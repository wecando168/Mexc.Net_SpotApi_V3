﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Logging;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using Microsoft.Extensions.Logging;

namespace CryptoExchange.Net.OrderBook
{
    /// <summary>
    /// Base for order book implementations
    /// </summary>
    public abstract class SymbolOrderBook : ISymbolOrderBook, IDisposable
    {
        private readonly object _bookLock = new object();

        private OrderBookStatus _status;
        private UpdateSubscription? _subscription;
        
        private bool _stopProcessing;
        private Task? _processTask;
        private CancellationTokenSource? _cts;

        private readonly AsyncResetEvent _queueEvent;
        private readonly ConcurrentQueue<object> _processQueue;
        private readonly bool _validateChecksum;

        private class EmptySymbolOrderBookEntry : ISymbolOrderBookEntry
        {
            public decimal Quantity { get => 0m;
                set { } }
            public decimal Price { get => 0m;
                set { } }
        }

        private static readonly ISymbolOrderBookEntry emptySymbolOrderBookEntry = new EmptySymbolOrderBookEntry();


        /// <summary>
        /// A buffer to store messages received before the initial book snapshot is processed. These messages
        /// will be processed after the book snapshot is set. Any messages in this buffer with sequence numbers lower
        /// than the snapshot sequence number will be discarded
        /// </summary>
        protected readonly List<ProcessBufferRangeSequenceEntry> processBuffer;

        /// <summary>
        /// The ask list, should only be accessed using the bookLock
        /// </summary>
        protected SortedList<decimal, ISymbolOrderBookEntry> asks;

        /// <summary>
        /// The bid list, should only be accessed using the bookLock
        /// </summary>
        protected SortedList<decimal, ISymbolOrderBookEntry> bids;

        /// <summary>
        /// The log
        /// </summary>
        protected Log log;

        /// <summary>
        /// Whether update numbers are consecutive. If set to true and an update comes in which isn't the previous sequences number + 1
        /// the book will resynchronize as it is deemed out of sync
        /// </summary>
        protected bool sequencesAreConsecutive;
        
        /// <summary>
        /// Whether levels should be strictly enforced. For example, when an order book has 25 levels and a new update comes in which pushes
        /// the current level 25 ask out of the top 25, should the curent the level 26 entry be removed from the book or does the 
        /// server handle this
        /// </summary>
        protected bool strictLevels;

        /// <summary>
        /// If the initial snapshot of the book has been set
        /// </summary>
        protected bool bookSet;

        /// <summary>
        /// The amount of levels for this book
        /// </summary>
        protected int? Levels { get; set; } = null;

        /// <inheritdoc/>
        public string Id { get; }

        /// <inheritdoc/>
        public OrderBookStatus Status 
        {
            get => _status;
            set
            {
                if (value == _status)
                    return;

                var old = _status;
                _status = value;
                log.Write(LogLevel.Information, $"{Id} order book {Symbol} status changed: {old} => {value}");
                OnStatusChange?.Invoke(old, _status);
            }
        }

        /// <inheritdoc/>
        public long LastSequenceNumber { get; private set; }

        /// <inheritdoc/>
        public string Symbol { get; }

        /// <inheritdoc/>
        public event Action<OrderBookStatus, OrderBookStatus>? OnStatusChange;

        /// <inheritdoc/>
        public event Action<(ISymbolOrderBookEntry BestBid, ISymbolOrderBookEntry BestAsk)>? OnBestOffersChanged;

        /// <inheritdoc/>
        public event Action<(IEnumerable<ISymbolOrderBookEntry> Bids, IEnumerable<ISymbolOrderBookEntry> Asks)>? OnOrderBookUpdate;

        /// <inheritdoc/>
        public DateTime UpdateTime { get; private set; }

        /// <inheritdoc/>
        public int AskCount { get; private set; }

        /// <inheritdoc/>
        public int BidCount { get; private set; }

        /// <inheritdoc/>
        public IEnumerable<ISymbolOrderBookEntry> Asks
        {
            get
            {
                lock (_bookLock)
                    return asks.Select(a => a.Value).ToList();
            }
        }

        /// <inheritdoc/>
        public IEnumerable<ISymbolOrderBookEntry> Bids 
        {
            get
            {
                lock (_bookLock)
                    return bids.Select(a => a.Value).ToList();
            }
        }

        /// <inheritdoc/>
        public (IEnumerable<ISymbolOrderBookEntry> bids, IEnumerable<ISymbolOrderBookEntry> asks) Book
        {
            get
            {
                lock (_bookLock)
                    return (Bids, Asks);
            }
        }

        /// <inheritdoc/>
        public ISymbolOrderBookEntry BestBid
        {
            get
            {
                lock (_bookLock)
                    return bids.FirstOrDefault().Value ?? emptySymbolOrderBookEntry;
            }
        }

        /// <inheritdoc/>
        public ISymbolOrderBookEntry BestAsk 
        {
            get
            {
                lock (_bookLock)
                    return asks.FirstOrDefault().Value ?? emptySymbolOrderBookEntry;
            }
        }

        /// <inheritdoc/>
        public (ISymbolOrderBookEntry Bid, ISymbolOrderBookEntry Ask) BestOffers {
            get {
                lock (_bookLock)
                    return (BestBid,BestAsk);
            }
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="id">The id of the order book. Should be set to {Exchange}[{type}], for example: Kucoin[Spot]</param>
        /// <param name="symbol">The symbol the order book is for</param>
        /// <param name="options">The options for the order book</param>
        protected SymbolOrderBook(string id, string symbol, OrderBookOptions options)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            Id = id;
            processBuffer = new List<ProcessBufferRangeSequenceEntry>();
            _processQueue = new ConcurrentQueue<object>();
            _queueEvent = new AsyncResetEvent(false, true);

            _validateChecksum = options.ChecksumValidationEnabled;
            Symbol = symbol;
            Status = OrderBookStatus.Disconnected;

            asks = new SortedList<decimal, ISymbolOrderBookEntry>();
            bids = new SortedList<decimal, ISymbolOrderBookEntry>(new DescComparer<decimal>());

            log = new Log(id) { Level = options.LogLevel };
            var writers = options.LogWriters ?? new List<ILogger> { new DebugLogger() };
            log.UpdateWriters(writers.ToList());
        }

        /// <inheritdoc/>
        public async Task<CallResult<bool>> StartAsync(CancellationToken? ct = null)
        {
            if (Status != OrderBookStatus.Disconnected)
                throw new InvalidOperationException($"Can't start book unless state is {OrderBookStatus.Disconnected}. Current state: {Status}");

            log.Write(LogLevel.Debug, $"{Id} order book {Symbol} starting");
            _cts = new CancellationTokenSource();
            ct?.Register(async () =>
            {
                _cts.Cancel();
                await StopAsync().ConfigureAwait(false);
            }, false);

            // Clear any previous messages
            while (_processQueue.TryDequeue(out _)) { }
            processBuffer.Clear();
            bookSet = false;

            Status = OrderBookStatus.Connecting;
            _processTask = Task.Factory.StartNew(ProcessQueue, TaskCreationOptions.LongRunning);

            var startResult = await DoStartAsync(_cts.Token).ConfigureAwait(false);
            if (!startResult)
            {
                Status = OrderBookStatus.Disconnected;
                return new CallResult<bool>(startResult.Error!);
            }

            if (_cts.IsCancellationRequested)
            {
                log.Write(LogLevel.Debug, $"{Id} order book {Symbol} stopped while starting");
                await startResult.Data.CloseAsync().ConfigureAwait(false);
                Status = OrderBookStatus.Disconnected;
                return new CallResult<bool>(new CancellationRequestedError());
            }

            _subscription = startResult.Data;
            _subscription.ConnectionLost += HandleConnectionLost;
            _subscription.ConnectionClosed += HandleConnectionClosed;
            _subscription.ConnectionRestored += HandleConnectionRestored;

            Status = OrderBookStatus.Synced;
            return new CallResult<bool>(true);
        }

        private void HandleConnectionLost() {
             log.Write(LogLevel.Warning, $"{Id} order book {Symbol} connection lost");
             if (Status != OrderBookStatus.Disposed) {
                Status = OrderBookStatus.Reconnecting;
                Reset();
            }
        }

        private void HandleConnectionClosed() {
            log.Write(LogLevel.Warning, $"{Id} order book {Symbol} disconnected");
            Status = OrderBookStatus.Disconnected;
            _ = StopAsync();
        }

        private async void HandleConnectionRestored(TimeSpan _) {
            await ResyncAsync().ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task StopAsync()
        {
            log.Write(LogLevel.Debug, $"{Id} order book {Symbol} stopping");
            Status = OrderBookStatus.Disconnected;
            _cts?.Cancel();
            _queueEvent.Set();
            if (_processTask != null)
                await _processTask.ConfigureAwait(false);

            if (_subscription != null) {
                await _subscription.CloseAsync().ConfigureAwait(false);
                _subscription.ConnectionLost -= HandleConnectionLost;
                _subscription.ConnectionClosed -= HandleConnectionClosed;
                _subscription.ConnectionRestored -= HandleConnectionRestored;
            }
            log.Write(LogLevel.Trace, $"{Id} order book {Symbol} stopped");
        }

        /// <inheritdoc/>
        public CallResult<decimal> CalculateAverageFillPrice(decimal quantity, OrderBookEntryType type)
        {
            if (Status != OrderBookStatus.Synced)
                return new CallResult<decimal>(new InvalidOperationError($"{nameof(CalculateAverageFillPrice)} is not available when book is not in Synced state"));

            var totalCost = 0m;
            var totalAmount = 0m;
            var amountLeft = quantity;
            lock (_bookLock)
            {
                var list = type == OrderBookEntryType.Ask ? asks : bids;
                
                var step = 0;
                while (amountLeft > 0)
                {
                    if (step == list.Count)
                        return new CallResult<decimal>(new InvalidOperationError("Quantity is larger than order in the order book"));

                    var element = list.ElementAt(step);
                    var stepAmount = Math.Min(element.Value.Quantity, amountLeft);
                    totalCost += stepAmount * element.Value.Price;
                    totalAmount += stepAmount;
                    amountLeft -= stepAmount;
                    step++;
                }
            }

            return new CallResult<decimal>(Math.Round(totalCost / totalAmount, 8));
        }

        /// <summary>
        /// Implementation for starting the order book. Should typically have logic for subscribing to the update stream and retrieving
        /// and setting the initial order book
        /// </summary>
        /// <returns></returns>
        protected abstract Task<CallResult<UpdateSubscription>> DoStartAsync(CancellationToken ct);

        /// <summary>
        /// Reset the order book
        /// </summary>
        protected virtual void DoReset() { }

        /// <summary>
        /// Resync the order book
        /// </summary>
        /// <returns></returns>
        protected abstract Task<CallResult<bool>> DoResyncAsync(CancellationToken ct);

        /// <summary>
        /// Implementation for validating a checksum value with the current order book. If checksum validation fails (returns false)
        /// the order book will be resynchronized
        /// </summary>
        /// <param name="checksum"></param>
        /// <returns></returns>
        protected virtual bool DoChecksum(int checksum) => true;
                
        /// <summary>
        /// Set the initial data for the order book. Typically the snapshot which was requested from the Rest API, or the first snapshot
        /// received from a socket subcription
        /// </summary>
        /// <param name="orderBookSequenceNumber">The last update sequence number until which the snapshot is in sync</param>
        /// <param name="askList">List of asks</param>
        /// <param name="bidList">List of bids</param>
        protected void SetInitialOrderBook(long orderBookSequenceNumber, IEnumerable<ISymbolOrderBookEntry> bidList, IEnumerable<ISymbolOrderBookEntry> askList)
        {
            _processQueue.Enqueue(new InitialOrderBookItem { StartUpdateId = orderBookSequenceNumber, EndUpdateId = orderBookSequenceNumber, Asks = askList, Bids = bidList });
            _queueEvent.Set();
        }

        /// <summary>
        /// Add an update to the process queue. Updates the book by providing changed bids and asks, along with an update number which should be higher than the previous update numbers
        /// </summary>
        /// <param name="updateId">The sequence number</param>
        /// <param name="bids">List of updated/new bids</param>
        /// <param name="asks">List of updated/new asks</param>
        protected void UpdateOrderBook(long updateId, IEnumerable<ISymbolOrderBookEntry> bids, IEnumerable<ISymbolOrderBookEntry> asks)
        {
            _processQueue.Enqueue(new ProcessQueueItem { StartUpdateId = updateId, EndUpdateId = updateId, Asks = asks, Bids = bids });
            _queueEvent.Set();
        }

        /// <summary>
        /// Add an update to the process queue. Updates the book by providing changed bids and asks, along with the first and last sequence number in the update
        /// </summary>
        /// <param name="firstUpdateId">The sequence number of the first update</param>
        /// <param name="lastUpdateId">The sequence number of the last update</param>
        /// <param name="bids">List of updated/new bids</param>
        /// <param name="asks">List of updated/new asks</param>
        protected void UpdateOrderBook(long firstUpdateId, long lastUpdateId, IEnumerable<ISymbolOrderBookEntry> bids, IEnumerable<ISymbolOrderBookEntry> asks)
        {
            _processQueue.Enqueue(new ProcessQueueItem { StartUpdateId = firstUpdateId, EndUpdateId = lastUpdateId, Asks = asks, Bids = bids });
            _queueEvent.Set();
        }

        /// <summary>
        /// Add an update to the process queue. Updates the book by providing changed bids and asks, each with its own sequence number
        /// </summary>
        /// <param name="bids">List of updated/new bids</param>
        /// <param name="asks">List of updated/new asks</param>
        protected void UpdateOrderBook(IEnumerable<ISymbolOrderSequencedBookEntry> bids, IEnumerable<ISymbolOrderSequencedBookEntry> asks)
        {
            var highest = Math.Max(bids.Any() ? bids.Max(b => b.Sequence) : 0, asks.Any() ? asks.Max(a => a.Sequence) : 0);
            var lowest = Math.Min(bids.Any() ? bids.Min(b => b.Sequence) : long.MaxValue, asks.Any() ? asks.Min(a => a.Sequence) : long.MaxValue);

            _processQueue.Enqueue(new ProcessQueueItem { StartUpdateId = lowest, EndUpdateId = highest, Asks = asks, Bids = bids });
            _queueEvent.Set();
        }

        /// <summary>
        /// Add a checksum value to the process queue
        /// </summary>
        /// <param name="checksum">The checksum value</param>
        protected void AddChecksum(int checksum)
        {
            _processQueue.Enqueue(new ChecksumItem() { Checksum = checksum });
            _queueEvent.Set();
        }

        /// <summary>
        /// Check and empty the process buffer; see what entries to update the book with
        /// </summary>
        protected void CheckProcessBuffer()
        {
            var pbList = processBuffer.ToList();
            if (pbList.Count > 0)
                log.Write(LogLevel.Debug, $"Processing {pbList.Count} buffered updates");

            foreach (var bufferEntry in pbList)
            {
                ProcessRangeUpdates(bufferEntry.FirstUpdateId, bufferEntry.LastUpdateId, bufferEntry.Bids, bufferEntry.Asks);
                processBuffer.Remove(bufferEntry);
            }
        }

        /// <summary>
        /// Update order book with an entry
        /// </summary>
        /// <param name="sequence">Sequence number of the update</param>
        /// <param name="type">Type of entry</param>
        /// <param name="entry">The entry</param>
        protected virtual bool ProcessUpdate(long sequence, OrderBookEntryType type, ISymbolOrderBookEntry entry)
        {
            if (sequence <= LastSequenceNumber)
            {
                log.Write(LogLevel.Debug, $"{Id} order book {Symbol} update skipped #{sequence}, currently at #{LastSequenceNumber}");
                return false;
            }

            if (sequencesAreConsecutive && sequence > LastSequenceNumber + 1)
            {
                // Out of sync
                log.Write(LogLevel.Warning, $"{Id} order book {Symbol} out of sync (expected { LastSequenceNumber + 1}, was {sequence}), reconnecting");
                _stopProcessing = true;
                Resubscribe();
                return false;
            }

            UpdateTime = DateTime.UtcNow;
            var listToChange = type == OrderBookEntryType.Ask ? asks : bids;
            if (entry.Quantity == 0)
            {
                if (!listToChange.ContainsKey(entry.Price))
                    return true;

                listToChange.Remove(entry.Price);
                if (type == OrderBookEntryType.Ask) AskCount--;
                else BidCount--;
            }
            else
            {
                if (!listToChange.ContainsKey(entry.Price))
                {
                    listToChange.Add(entry.Price, entry);
                    if (type == OrderBookEntryType.Ask) AskCount++;
                    else BidCount++;
                }
                else
                {
                    listToChange[entry.Price] = entry;
                }
            }

            return true;
        }

        /// <summary>
        /// Wait until the order book snapshot has been set
        /// </summary>
        /// <param name="timeout">Max wait time</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        protected async Task<CallResult<bool>> WaitForSetOrderBookAsync(TimeSpan timeout, CancellationToken ct)
        {
            var startWait = DateTime.UtcNow;
            while (!bookSet && Status == OrderBookStatus.Syncing)
            {
                if(ct.IsCancellationRequested)
                    return new CallResult<bool>(new CancellationRequestedError());

                if (DateTime.UtcNow - startWait > timeout)
                    return new CallResult<bool>(new ServerError("Timeout while waiting for data"));

                try
                {
                    await Task.Delay(50, ct).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                { }
            }

            return new CallResult<bool>(true);
        }

        /// <summary>
        /// IDisposable implementation for the order book
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose method
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            Status = OrderBookStatus.Disposing;

            _cts?.Cancel();
            _queueEvent.Set();

            // Clear queue
            while (_processQueue.TryDequeue(out _)) { }

            processBuffer.Clear();
            asks.Clear();
            bids.Clear();
            AskCount = 0;
            BidCount = 0;

            Status = OrderBookStatus.Disposed;
        }

        /// <summary>
        /// String representation of the top 3 entries
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToString(3);
        }

        /// <summary>
        /// String representation of the top x entries
        /// </summary>
        /// <returns></returns>
        public string ToString(int numberOfEntries)
        {
            var stringBuilder = new StringBuilder();
            var book = Book;
            stringBuilder.AppendLine($"   Ask quantity       Ask price | Bid price       Bid quantity");
            for(var i = 0; i < numberOfEntries; i++)
            {
                var ask = book.asks.Count() > i ? book.asks.ElementAt(i): null;
                var bid = book.bids.Count() > i ? book.bids.ElementAt(i): null;
                stringBuilder.AppendLine($"[{ask?.Quantity.ToString(CultureInfo.InvariantCulture),14}] {ask?.Price.ToString(CultureInfo.InvariantCulture),14} | {bid?.Price.ToString(CultureInfo.InvariantCulture),-14} [{bid?.Quantity.ToString(CultureInfo.InvariantCulture),-14}]");
            }
            return stringBuilder.ToString();
        }

        private void CheckBestOffersChanged(ISymbolOrderBookEntry prevBestBid, ISymbolOrderBookEntry prevBestAsk)
        {
            var (bestBid, bestAsk) = BestOffers;
            if (bestBid.Price != prevBestBid.Price || bestBid.Quantity != prevBestBid.Quantity ||
                   bestAsk.Price != prevBestAsk.Price || bestAsk.Quantity != prevBestAsk.Quantity)
                OnBestOffersChanged?.Invoke((bestBid, bestAsk));
        }

        private void Reset()
        {
            _queueEvent.Set();
            // Clear queue
            while (_processQueue.TryDequeue(out _)) { }
            processBuffer.Clear();
            bookSet = false;
            DoReset();
        }

        private async Task ResyncAsync()
        {
            Status = OrderBookStatus.Syncing;
            var success = false;
            while (!success)
            {
                if (Status != OrderBookStatus.Syncing)
                    return;

                var resyncResult = await DoResyncAsync(_cts!.Token).ConfigureAwait(false);
                success = resyncResult;
            }

            log.Write(LogLevel.Information, $"{Id} order book {Symbol} successfully resynchronized");
            Status = OrderBookStatus.Synced;
        }

        private async Task ProcessQueue()
        {
            while (Status != OrderBookStatus.Disconnected && Status != OrderBookStatus.Disposed)
            {
                await _queueEvent.WaitAsync().ConfigureAwait(false);

                while (_processQueue.TryDequeue(out var item))
                {
                    if (Status == OrderBookStatus.Disconnected || Status == OrderBookStatus.Disposed)
                        break;

                    if (_stopProcessing)
                    {
                        log.Write(LogLevel.Trace, "Skipping message because of resubscribing");
                        continue;
                    }

                    if (item is InitialOrderBookItem iobi)
                        ProcessInitialOrderBookItem(iobi);
                    if (item is ProcessQueueItem pqi)
                        ProcessQueueItem(pqi);
                    else if (item is ChecksumItem ci)
                        ProcessChecksum(ci);
                }
            }
        }

        private void ProcessInitialOrderBookItem(InitialOrderBookItem item)
        {
            lock (_bookLock)
            {
                bookSet = true;
                asks.Clear();
                foreach (var ask in item.Asks)
                    asks.Add(ask.Price, ask);
                bids.Clear();
                foreach (var bid in item.Bids)
                    bids.Add(bid.Price, bid);

                LastSequenceNumber = item.EndUpdateId;

                AskCount = asks.Count;
                BidCount = bids.Count;

                UpdateTime = DateTime.UtcNow;
                log.Write(LogLevel.Debug, $"{Id} order book {Symbol} data set: {BidCount} bids, {AskCount} asks. #{item.EndUpdateId}");
                CheckProcessBuffer();
                OnOrderBookUpdate?.Invoke((item.Bids, item.Asks));
                OnBestOffersChanged?.Invoke((BestBid, BestAsk));
            }
        }

        private void ProcessQueueItem(ProcessQueueItem item)
        {
            lock (_bookLock)
            {
                if (!bookSet)
                {
                    processBuffer.Add(new ProcessBufferRangeSequenceEntry()
                    {
                        Asks = item.Asks,
                        Bids = item.Bids,
                        FirstUpdateId = item.StartUpdateId,
                        LastUpdateId = item.EndUpdateId,
                    });
                    log.Write(LogLevel.Trace, $"{Id} order book {Symbol} update buffered #{item.StartUpdateId}-#{item.EndUpdateId} [{item.Asks.Count()} asks, {item.Bids.Count()} bids]");
                }
                else
                {
                    CheckProcessBuffer();
                    var (prevBestBid, prevBestAsk) = BestOffers;
                    ProcessRangeUpdates(item.StartUpdateId, item.EndUpdateId, item.Bids, item.Asks);

                    if (!asks.Any() || !bids.Any())
                        return;

                    if (asks.First().Key < bids.First().Key)
                    {
                        log.Write(LogLevel.Warning, $"{Id} order book {Symbol} detected out of sync order book. First ask: {asks.First().Key}, first bid: {bids.First().Key}. Resyncing");
                        _stopProcessing = true;
                        Resubscribe();
                        return;
                    }

                    OnOrderBookUpdate?.Invoke((item.Bids, item.Asks));
                    CheckBestOffersChanged(prevBestBid, prevBestAsk);
                }
            }
        }

        private void ProcessChecksum(ChecksumItem ci)
        {
            lock (_bookLock)
            {
                if (!_validateChecksum)
                    return;

                bool checksumResult = false;
                try
                {
                    checksumResult = DoChecksum(ci.Checksum);
                }
                catch (Exception)
                {
                    // If the status is not synced it can be expected a checksum is failing

                    if (Status == OrderBookStatus.Synced)
                        throw;
                }

                if (!checksumResult)
                {
                    log.Write(LogLevel.Warning, $"{Id} order book {Symbol} out of sync. Resyncing");
                    _stopProcessing = true;
                    Resubscribe();
                }
            }
        }

        private void Resubscribe()
        {
            Status = OrderBookStatus.Syncing;
            _ = Task.Run(async () =>
            {
                if(_subscription == null)
                {
                    Status = OrderBookStatus.Disconnected;
                    return;
                }

                await _subscription!.UnsubscribeAsync().ConfigureAwait(false);
                Reset();
                _stopProcessing = false;
                if (!await _subscription!.ResubscribeAsync().ConfigureAwait(false))
                {
                    // Resubscribing failed, reconnect the socket
                    log.Write(LogLevel.Warning, $"{Id} order book {Symbol} resync failed, reconnecting socket");
                    Status = OrderBookStatus.Reconnecting;
                    _ = _subscription!.ReconnectAsync();
                }
                else
                    await ResyncAsync().ConfigureAwait(false);
            });
        }

        private void ProcessRangeUpdates(long firstUpdateId, long lastUpdateId, IEnumerable<ISymbolOrderBookEntry> bids, IEnumerable<ISymbolOrderBookEntry> asks)
        {
            if (lastUpdateId <= LastSequenceNumber)
            {
                log.Write(LogLevel.Trace, $"{Id} order book {Symbol} update skipped #{firstUpdateId}-{lastUpdateId}");
                return;
            }

            foreach (var entry in bids)
                ProcessUpdate(LastSequenceNumber + 1, OrderBookEntryType.Bid, entry);

            foreach (var entry in asks)
                ProcessUpdate(LastSequenceNumber + 1, OrderBookEntryType.Ask, entry);

            if (Levels.HasValue && strictLevels)
            {
                while (this.bids.Count > Levels.Value)
                {
                    BidCount--;
                    this.bids.Remove(this.bids.Last().Key);
                }

                while (this.asks.Count > Levels.Value)
                {
                    AskCount--;
                    this.asks.Remove(this.asks.Last().Key);
                }
            }

            LastSequenceNumber = lastUpdateId;
            log.Write(LogLevel.Trace, $"{Id} order book {Symbol} update processed #{firstUpdateId}-{lastUpdateId}");
        }        
    }

    internal class DescComparer<T> : IComparer<T>
    {
        public int Compare(T x, T y)
        {
            return Comparer<T>.Default.Compare(y, x);
        }
    }
}
