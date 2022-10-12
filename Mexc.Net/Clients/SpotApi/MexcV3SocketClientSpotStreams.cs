using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mexc.Net.Converters;
using Mexc.Net.Enums;
using Mexc.Net.Interfaces;
using Mexc.Net.Interfaces.Clients.SpotApi;
using Mexc.Net.Objects;
using Mexc.Net.Objects.Models;
using Mexc.Net.Objects.Models.Spot.Socket;
using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Logging;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mexc.Net.Clients.SpotApi
{
    /// <inheritdoc />
    public class MexcV3SocketClientSpotStreams : SocketApiClient, IMexcV3SocketClientSpotStreams
    {
        private Log _log = new Log("MexcV3SocketClientSpotStreams");

        #region fields
        private readonly MexcV3SocketClient _baseClient;
        private readonly MexcV3SocketClientOptions _options;

        private const string tradesStreamEndpoint = "spot@public.deals.v3.api";             //逐笔交易(实时)
        private const string klineStreamEndpoint = "spot@public.kline.v3.api";              //K线 Streams
        private const string depthStreamEndpoint = "spot@public.increase.depth.v3.api";     //增量深度信息(实时)

        private const string privateDealsUpdateEvent = "spot@private.deals.v3.api";         //账户成交(实时)
        private const string privateOrdersUpdateEvent = "spot@private.orders.v3.api";       //账户订单(实时)

        private const string bookTickerStreamEndpoint = "@bookTicker";
        private const string allBookTickerStreamEndpoint = "!bookTicker";


        private const string aggregatedTradesStreamEndpoint = "@aggTrade";
        private const string symbolTickerStreamEndpoint = "@ticker";
        private const string allSymbolTickerStreamEndpoint = "!ticker@arr";
        private const string partialBookDepthStreamEndpoint = "@depth";
        private const string symbolMiniTickerStreamEndpoint = "@miniTicker";
        private const string allSymbolMiniTickerStreamEndpoint = "!miniTicker@arr";

        private const string bltvInfoEndpoint = "@tokenNav";
        private const string bltvKlineEndpoint = "@nav_kline";

        private const string executionUpdateEvent = "executionReport";
        private const string ocoOrderUpdateEvent = "listStatus";
        private const string accountPositionUpdateEvent = "outboundAccountPosition";
        private const string balanceUpdateEvent = "balanceUpdate";
        #endregion

        #region constructor/destructor

        /// <summary>
        /// Create a new public instance of MexcSocketClientSpot with default options
        /// </summary>
        public MexcV3SocketClientSpotStreams(Log log, MexcV3SocketClient baseClient, MexcV3SocketClientOptions options) :
            base(options, options.SpotStreamsOptions)
        {
            _options = options;
            _baseClient = baseClient;
            _log = log;
        }

        /// <inheritdoc />
        protected override AuthenticationProvider CreateAuthenticationProvider(ApiCredentials credentials)
            => new MexcV3AuthenticationProvider(credentials);
        #endregion

        #region methods

        #region Trade Streams

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToTradeUpdatesAsync(string symbol,
            Action<DataEvent<MexcV3StreamTrade>> onMessage, CancellationToken ct = default) =>
            await SubscribeToTradeUpdatesAsync(new[] { symbol }, onMessage, ct).ConfigureAwait(false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbols"></param>
        /// <param name="onMessage"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<CallResult<UpdateSubscription>> SubscribeToTradeUpdatesAsync(
            IEnumerable<string> symbols,
            Action<DataEvent<MexcV3StreamTrade>> onMessage,
            CancellationToken ct = default)
        {
            symbols.ValidateNotNull(nameof(symbols));
            foreach (var symbol in symbols)
            {
                symbol.ValidateMexcSymbol();
            }
            Action<DataEvent<MexcV3StreamTrade>>? handler = new Action<DataEvent<MexcV3StreamTrade>>(data => onMessage(data));
            symbols = symbols.Select(a =>
                $"{tradesStreamEndpoint}" +
                $"@{a.ToUpper(CultureInfo.InvariantCulture)}"
                ).ToArray();
            CallResult<UpdateSubscription>? response = await _baseClient.SubscribeInternal(this, BaseAddress, symbols, handler, ct).ConfigureAwait(false);
            return response;
        }
        #endregion

        #region Kline/Candlestick Streams

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToKlineUpdatesAsync(string symbol, MexcV3StreamsKlineInterval interval, 
            Action<DataEvent<MexcV3StreamKline>> onMessage, CancellationToken ct = default) =>
            await SubscribeToKlineUpdatesAsync(new[] { symbol }, interval, onMessage, ct).ConfigureAwait(false);

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToKlineUpdatesAsync(string symbol, IEnumerable<MexcV3StreamsKlineInterval> intervals, 
            Action<DataEvent<MexcV3StreamKline>> onMessage, CancellationToken ct = default) =>
            await SubscribeToKlineUpdatesAsync(new[] { symbol }, intervals, onMessage, ct).ConfigureAwait(false);

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToKlineUpdatesAsync(IEnumerable<string> symbols, MexcV3StreamsKlineInterval interval, 
            Action<DataEvent<MexcV3StreamKline>> onMessage, CancellationToken ct = default) =>
            await SubscribeToKlineUpdatesAsync(symbols, new[] { interval }, onMessage, ct).ConfigureAwait(false);

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToKlineUpdatesAsync(IEnumerable<string> symbols, IEnumerable<MexcV3StreamsKlineInterval> intervals, 
            Action<DataEvent<MexcV3StreamKline>> onMessage, CancellationToken ct = default)
        {
            symbols.ValidateNotNull(nameof(symbols));
            foreach (var symbol in symbols)
                symbol.ValidateMexcSymbol();
            Action<DataEvent<MexcV3StreamKline>>? handler = new Action<DataEvent<MexcV3StreamKline>>(data => onMessage(data));
            symbols = symbols.SelectMany(a =>
                intervals.Select(i =>
                    $"{klineStreamEndpoint}" +
                    $"@{a.ToUpper(CultureInfo.InvariantCulture)}" +
                    $"@{JsonConvert.SerializeObject(i, new MexcV3StreamsKlineIntervalConverter(false))}"
                    )
                ).ToArray();
            CallResult<UpdateSubscription>? response = await _baseClient.SubscribeInternal(this, BaseAddress, symbols, handler, ct).ConfigureAwait(false);
            return response;
        }

        #endregion

        #region Diff. Depth Stream

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToDiffDepthUpdatesAsync(string symbol,
            Action<DataEvent<MexcV3StreamDepth>> onMessage, CancellationToken ct = default) =>
            await SubscribeToDiffDepthUpdatesAsync(new[] { symbol }, onMessage, ct).ConfigureAwait(false);

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToDiffDepthUpdatesAsync(IEnumerable<string> symbols,
            Action<DataEvent<MexcV3StreamDepth>> onMessage, CancellationToken ct = default)
        {
            symbols.ValidateNotNull(nameof(symbols));
            foreach (var symbol in symbols)
                symbol.ValidateMexcSymbol();
            Action<DataEvent<MexcV3StreamDepth>>? handler = new Action<DataEvent<MexcV3StreamDepth>>(data => onMessage(data));
            symbols = symbols.Select(a =>
                $"{depthStreamEndpoint}" +
                $"@{a.ToUpper(CultureInfo.InvariantCulture)}"
                ).ToArray();
            CallResult<UpdateSubscription>? response = await _baseClient.SubscribeInternal(this, BaseAddress, symbols, handler, ct).ConfigureAwait(false);
            return response;
        }

        #endregion

        #region private deals Stream

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToPrivateDealsUpdatesAsync(
            string listenKey,
            Action<DataEvent<MexcV3StreamPrivateDeals>>? onMessage,
            CancellationToken ct = default
            )
        {
            listenKey.ValidateNotNull(nameof(listenKey));

            #region 方法一：
            Action<DataEvent<MexcV3StreamPrivateDeals>>? handler = new Action<DataEvent<MexcV3StreamPrivateDeals>>(data => onMessage(data));
            CallResult<UpdateSubscription>? response = await _baseClient.SubscribeInternal(this, BaseAddress, new[] { privateDealsUpdateEvent }, listenKey, handler, ct).ConfigureAwait(false);
            return response;
            #endregion

            #region 方法二(已经通过，备用误删）：
            //Action<DataEvent<MexcV3StreamPrivateDeals>>? handler = new Action<DataEvent<MexcV3StreamPrivateDeals>>(data =>
            //{
            //    JToken? combinedToken = JToken.Parse(data.Data.ToString());
            //    JToken? token = combinedToken["d"];
            //    if (token == null)
            //        return;

            //    string? evnt = token.Root["c"]?.ToString();
            //    if (evnt == null)
            //        return;

            //    CallResult<MexcV3StreamPrivateDeals>? result = _baseClient.DeserializeInternal<MexcV3StreamPrivateDeals>(combinedToken);
            //    if (result)
            //    {
            //        result.Data.Stream = combinedToken["c"]!.Value<string>()!;
            //        result.Data.ListenKey = listenKey;
            //        onMessage?.Invoke(data);
            //    }
            //    else
            //        _log.Write(LogLevel.Warning, "Couldn't deserialize data received from private deals stream: " + result.Error);
            //});
            //return await _baseClient.SubscribeInternal(this, BaseAddress, new[] { privateDealsUpdateEvent }, listenKey, handler, ct).ConfigureAwait(false);
            #endregion
        }
        #endregion

        #region private orders Stream

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToPrivateOrdersUpdatesAsync(
            string listenKey,
            Action<DataEvent<MexcV3StreamPrivateOrders>>? onMessage,
            CancellationToken ct = default)
        {
            listenKey.ValidateNotNull(nameof(listenKey));

            #region 方法一：
            Action<DataEvent<MexcV3StreamPrivateOrders>>? handler = new Action<DataEvent<MexcV3StreamPrivateOrders>>(data => onMessage(data));
            CallResult<UpdateSubscription>? response = await _baseClient.SubscribeInternal(this, BaseAddress, new[] { privateOrdersUpdateEvent }, listenKey, handler, ct).ConfigureAwait(false);
            return response;
            #endregion

            #region 方法二(已经通过，备用误删）：
            //Action<DataEvent<MexcV3StreamPrivateOrders>>? handler = new Action<DataEvent<MexcV3StreamPrivateOrders>>(data =>
            //{
            //    JToken? combinedToken = JToken.Parse(data.Data.ToString());
            //    JToken? token = combinedToken["d"];
            //    if (token == null)
            //        return;

            //    string? evnt = token.Root["c"]?.ToString();
            //    if (evnt == null)
            //        return;

            //    CallResult<MexcV3StreamPrivateOrders>? result = _baseClient.DeserializeInternal<MexcV3StreamPrivateOrders>(combinedToken);
            //    if (result)
            //    {
            //        result.Data.Stream = combinedToken["c"]!.Value<string>()!;
            //        result.Data.ListenKey = listenKey;
            //        onMessage?.Invoke(data);
            //    }
            //    else
            //        _log.Write(LogLevel.Warning, "Couldn't deserialize data received from private orders stream: " + result.Error);
            //});
            //return await _baseClient.SubscribeInternal(this, BaseAddress, new[] { privateOrdersUpdateEvent }, listenKey, handler, ct).ConfigureAwait(false);
            #endregion

        }
        #endregion
        #endregion
    }
}
