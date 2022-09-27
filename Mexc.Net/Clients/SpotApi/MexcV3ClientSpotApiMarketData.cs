using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Mexc.Net.Converters;
using Mexc.Net.Enums;
using Mexc.Net.Interfaces;
using Mexc.Net.Interfaces.Clients.SpotApi;
using Mexc.Net.Objects.Models.Spot;
using CryptoExchange.Net;
using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Logging;
using CryptoExchange.Net.Objects;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Mexc.Net.Clients.SpotApi
{
    /// <inheritdoc />
    public class MexcV3ClientSpotApiMarketData : IMexcV3ClientSpotApiMarketData
    {
        private const string pingEndpoint = "ping";
        private const string checkTimeEndpoint = "time";
        private const string exchangeInfoEndpoint = "exchangeInfo";
        private const string orderBookEndpoint = "depth";
        private const string recentTradesEndpoint = "trades";
        private const string historicalTradesEndpoint = "historicalTrades";
        private const string aggregatedTradesEndpoint = "aggTrades";
        private const string klinesEndpoint = "klines";
        private const string averagePriceEndpoint = "avgPrice";
        private const string price24HEndpoint = "ticker/24hr";
        private const string allPricesEndpoint = "ticker/price";
        private const string bookPricesEndpoint = "ticker/bookTicker";

        private const string api = "api";
        private const string publicVersion = "3";

        private readonly Log _log;

        private readonly MexcV3ClientSpotApi _baseClient;

        internal MexcV3ClientSpotApiMarketData(Log log, MexcV3ClientSpotApi baseClient)
        {
            _log = log;
            _baseClient = baseClient;
        }

        #region 1.Test Connectivity

        /// <inheritdoc />
        public async Task<WebCallResult<long>> PingAsync(CancellationToken ct = default)
        {
            Stopwatch? sw = Stopwatch.StartNew();
            WebCallResult<object>? result = await _baseClient.MexcV3SendRequestInternal<object>(
                uri: _baseClient.GetUrl(pingEndpoint, api, publicVersion), 
                method: HttpMethod.Get, 
                cancellationToken: ct).ConfigureAwait(false);
            sw.Stop();
            return result ? result.As(sw.ElapsedMilliseconds) : result.As<long>(default!);
        }

        #endregion

        #region 2.Check Server Time

        /// <inheritdoc />
        public async Task<WebCallResult<long>> GetServerTimeStampAsync(CancellationToken ct = default)
        {
            var result = await _baseClient.MexcV3SendRequestInternal<MexcV3CheckServerTimeStamp>(
                uri: _baseClient.GetUrl(checkTimeEndpoint, api, publicVersion), 
                method: HttpMethod.Get, 
                cancellationToken: ct, 
                ignoreRateLimit: true).ConfigureAwait(false);
            return result.As(result.Data?.ServerTime ?? default);            
        }

        /// <inheritdoc />
        public async Task<WebCallResult<DateTime>> GetServerTimeAsync(CancellationToken ct = default)
        {
            var result = await _baseClient.MexcV3SendRequestInternal<MexcV3CheckServerTime>(
                uri: _baseClient.GetUrl(checkTimeEndpoint, api, publicVersion),
                method: HttpMethod.Get,
                cancellationToken: ct, 
                ignoreRateLimit: true).ConfigureAwait(false);
            return result.As(result.Data?.ServerTime ?? default);            
        }

        #endregion

        #region 3.Exchange Information
        /// <inheritdoc />
        public Task<WebCallResult<MexcV3ExchangeInfo>> GetExchangeInfoAsync(CancellationToken ct = default)
             => GetExchangeInfoAsync(Array.Empty<string>(), ct);

        /// <inheritdoc />
        public Task<WebCallResult<MexcV3ExchangeInfo>> GetExchangeInfoAsync(string symbol, CancellationToken ct = default)
             => GetExchangeInfoAsync(new string[] { symbol }, ct);

        /// <inheritdoc />
        public async Task<WebCallResult<MexcV3ExchangeInfo>> GetExchangeInfoAsync(IEnumerable<string> symbols, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();
            if (symbols.Count() > 1)
                parameters.Add("symbols", JsonConvert.SerializeObject(symbols));
            else if (symbols.Any())
                parameters.Add("symbol", symbols.First());

            var exchangeInfoResult = await _baseClient.MexcV3SendRequestInternal<MexcV3ExchangeInfo>(
                uri: _baseClient.GetUrl(exchangeInfoEndpoint, api, publicVersion), 
                method: HttpMethod.Get, 
                cancellationToken: ct, 
                parameters: parameters, 
                arraySerialization: ArrayParametersSerialization.Array, 
                weight: 10).ConfigureAwait(false);
            if (!exchangeInfoResult)
                return exchangeInfoResult;

            _baseClient.ExchangeInfo = exchangeInfoResult.Data;
            _baseClient.LastExchangeInfoUpdate = DateTime.UtcNow;
            _log.Write(LogLevel.Information, "Trade rules updated");
            return exchangeInfoResult;
        }
        #endregion

        #region 4.Order Book

        /// <inheritdoc />
        public async Task<WebCallResult<MexcV3OrderBook>> GetOrderBookAsync(string symbol, int? limit = null, CancellationToken ct = default)
        {
            symbol.ValidateMexcSymbol();
            limit?.ValidateIntBetween(nameof(limit), 1, 5000);
            var parameters = new Dictionary<string, object> { { "symbol", symbol } };
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            var requestWeight = limit == null ? 1 : limit <= 100 ? 1 : limit <= 500 ? 5 : limit <= 1000 ? 10 : 50;
            WebCallResult<MexcV3OrderBook>? result = await _baseClient.MexcV3SendRequestInternal<MexcV3OrderBook>(
                uri: _baseClient.GetUrl(orderBookEndpoint, api, publicVersion),
                method: HttpMethod.Get,
                cancellationToken: ct,
                parameters: parameters,
                weight: requestWeight).ConfigureAwait(false);
            if (result)
                result.Data.Symbol = symbol;
            return result;
        }

        #endregion

        #region 5.Recent Trades List

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<IMexcV3RecentTrade>>> GetRecentTradesAsync(string symbol, int? limit = null, CancellationToken ct = default)
        {
            symbol.ValidateMexcSymbol();
            limit?.ValidateIntBetween(nameof(limit), 1, 1000);

            var parameters = new Dictionary<string, object> { { "symbol", symbol } };
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));

            var result = await _baseClient.MexcV3SendRequestInternal<IEnumerable<MexcV3RecentTradeQuote>>(
                uri: _baseClient.GetUrl(recentTradesEndpoint, api, publicVersion),
                method:HttpMethod.Get,
                cancellationToken:ct,
                parameters:parameters).ConfigureAwait(false);
            return result.As<IEnumerable<IMexcV3RecentTrade>>(result.Data);
        }

        #endregion

        #region 6.Old Trade Lookup

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<IMexcV3RecentTrade>>> GetTradeHistoryAsync(string symbol, int? limit = null, CancellationToken ct = default)
        {
            symbol.ValidateMexcSymbol();
            limit?.ValidateIntBetween(nameof(limit), 1, 1000);
            var parameters = new Dictionary<string, object> { { "symbol", symbol } };
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));

            WebCallResult<IEnumerable<MexcV3RecentTradeQuote>>? result = await _baseClient.MexcV3SendRequestInternal<IEnumerable<MexcV3RecentTradeQuote>>(
                uri: _baseClient.GetUrl(historicalTradesEndpoint, api, publicVersion),
                method: HttpMethod.Get,
                cancellationToken: ct,
                parameters: parameters,
                weight: 5).ConfigureAwait(false);
            return result.As<IEnumerable<IMexcV3RecentTrade>>(result.Data);
        }

        #endregion

        #region 7.Compressed/Aggregate Trades List

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<IMexcV3AggregatedTrade>>> GetAggregatedTradeHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default)
        {
            symbol.ValidateMexcSymbol();
            limit?.ValidateIntBetween(nameof(limit), 1, 1000);

            var parameters = new Dictionary<string, object> { { "symbol", symbol } };
            parameters.AddOptionalParameter("startTime", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("endTime", DateTimeConverter.ConvertToMilliseconds(endTime));
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));

            WebCallResult<IEnumerable<MexcV3AggregatedTrade>>? response = await _baseClient.MexcV3SendRequestInternal<IEnumerable<MexcV3AggregatedTrade>>(
                uri: _baseClient.GetUrl(aggregatedTradesEndpoint, api, publicVersion),
                method: HttpMethod.Get,
                cancellationToken: ct,
                parameters: parameters).ConfigureAwait(false);
            return response.As<IEnumerable<IMexcV3AggregatedTrade>>(response.Data);
        }

        #endregion

        #region 8.Kline/Candlestick Data

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<IMexcV3Kline>>> GetKlinesAsync(string symbol, MexcV3RestKlineInterval interval, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default)
        {
            symbol.ValidateMexcSymbol();
            limit?.ValidateIntBetween(nameof(limit), 1, 1500);
            var parameters = new Dictionary<string, object> {
                { "symbol", symbol },
                { "interval", JsonConvert.SerializeObject(interval, new MexcV3RestKlineIntervalConverter(false)) }
            };
            parameters.AddOptionalParameter("startTime", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("endTime", DateTimeConverter.ConvertToMilliseconds(endTime));
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));

            WebCallResult<IEnumerable<MexcV3SpotKline>>? result = await _baseClient.MexcV3SendRequestInternal<IEnumerable<MexcV3SpotKline>>(
                uri: _baseClient.GetUrl(klinesEndpoint, api, publicVersion),
                method: HttpMethod.Get,
                cancellationToken: ct,
                parameters: parameters).ConfigureAwait(false);
            return result.As<IEnumerable<IMexcV3Kline>>(result.Data);
        }

        #endregion

        #region 9.Current Average Price

        /// <inheritdoc />
        public async Task<WebCallResult<MexcV3AveragePrice>> GetCurrentAvgPriceAsync(string symbol, CancellationToken ct = default)
        {
            symbol.ValidateMexcSymbol();
            var parameters = new Dictionary<string, object> { { "symbol", symbol } };

            WebCallResult<MexcV3AveragePrice>? response = await _baseClient.MexcV3SendRequestInternal<MexcV3AveragePrice>(
                uri: _baseClient.GetUrl(averagePriceEndpoint, api, publicVersion),
                method: HttpMethod.Get,
                cancellationToken: ct,
                parameters: parameters).ConfigureAwait(false);
            return response;
        }

        #endregion

        #region 10.24hr Ticker Price Change Statistics

        /// <inheritdoc />
        public async Task<WebCallResult<IMexcV3Tick>> GetTickerAsync(string symbol, CancellationToken ct = default)
        {
            symbol.ValidateMexcSymbol();
            var parameters = new Dictionary<string, object> { { "symbol", symbol } };

            WebCallResult<MexcV324HPrice>? result = await _baseClient.MexcV3SendRequestInternal<MexcV324HPrice>(
                uri: _baseClient.GetUrl(price24HEndpoint, api, publicVersion),
                method: HttpMethod.Get,
                cancellationToken: ct,
                parameters: parameters,
                weight: 1).ConfigureAwait(false);
            return result.As<IMexcV3Tick>(result.Data);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<IMexcV3Tick>>> GetTickersAsync(IEnumerable<string> symbols, CancellationToken ct = default)
        {
            foreach (var symbol in symbols)
                symbol.ValidateMexcSymbol();

            var parameters = new Dictionary<string, object> { { "symbols", $"[{string.Join(",", symbols.Select(s => $"\"{s}\""))}]" } };
            var symbolCount = symbols.Count();
            var weight = symbolCount <= 20 ? 1 : symbolCount <= 100 ? 20 : 40;
            WebCallResult<IEnumerable<MexcV324HPrice>>? result = await _baseClient.MexcV3SendRequestInternal<IEnumerable<MexcV324HPrice>>(
                uri: _baseClient.GetUrl(price24HEndpoint, api, publicVersion),
                method: HttpMethod.Get,
                cancellationToken: ct,
                parameters: parameters,
                weight: weight).ConfigureAwait(false);
            return result.As<IEnumerable<IMexcV3Tick>>(result.Data);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<IMexcV3Tick>>> GetTickersAsync(CancellationToken ct = default)
        {
            WebCallResult<IEnumerable<MexcV324HPrice>>? result = await _baseClient.MexcV3SendRequestInternal<IEnumerable<MexcV324HPrice>>(
                uri: _baseClient.GetUrl(price24HEndpoint, api, publicVersion),
                method: HttpMethod.Get,
                cancellationToken: ct,
                weight: 40).ConfigureAwait(false);
            return result.As<IEnumerable<IMexcV3Tick>>(result.Data);
        }

        #endregion

        #region 11.Symbol Price Ticker

        /// <inheritdoc />
        public async Task<WebCallResult<MexcV3Price>> GetPriceAsync(string symbol, CancellationToken ct = default)
        {
            symbol.ValidateMexcSymbol();
            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol }
            };

            WebCallResult<MexcV3Price>? response = await _baseClient.MexcV3SendRequestInternal<MexcV3Price>(
                uri: _baseClient.GetUrl(allPricesEndpoint, api, publicVersion),
                method: HttpMethod.Get,
                cancellationToken: ct,
                parameters: parameters).ConfigureAwait(false);
            return response;
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<MexcV3Price>>> GetPricesAsync(IEnumerable<string> symbols, CancellationToken ct = default)
        {
            foreach (var symbol in symbols)
                symbol.ValidateMexcSymbol();

            var parameters = new Dictionary<string, object> { { "symbols", $"[{string.Join(",", symbols.Select(s => $"\"{s}\""))}]" } };
            WebCallResult<IEnumerable<MexcV3Price>>? response = await _baseClient.MexcV3SendRequestInternal<IEnumerable<MexcV3Price>>(
                uri: _baseClient.GetUrl(allPricesEndpoint, api, publicVersion),
                method: HttpMethod.Get,
                cancellationToken: ct,
                parameters: parameters,
                weight: 2).ConfigureAwait(false);
            return response;
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<MexcV3Price>>> GetPricesAsync(CancellationToken ct = default)
        {
            WebCallResult<IEnumerable<MexcV3Price>>? response = await _baseClient.MexcV3SendRequestInternal<IEnumerable<MexcV3Price>>(
                uri: _baseClient.GetUrl(allPricesEndpoint, api, publicVersion),
                method: HttpMethod.Get,
                cancellationToken: ct,
                weight: 2).ConfigureAwait(false);
            return response;
        }

        #endregion

        #region 12.Symbol Order Book Ticker

        /// <inheritdoc />
        public async Task<WebCallResult<MexcV3BookPrice>> GetBookPriceAsync(string symbol, CancellationToken ct = default)
        {
            symbol.ValidateMexcSymbol();
            var parameters = new Dictionary<string, object> { { "symbol", symbol } };

            WebCallResult<MexcV3BookPrice>? response = await _baseClient.MexcV3SendRequestInternal<MexcV3BookPrice>(
                uri: _baseClient.GetUrl(bookPricesEndpoint, api, publicVersion),
                method: HttpMethod.Get,
                cancellationToken: ct,
                parameters: parameters).ConfigureAwait(false);
            return response;
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<MexcV3BookPrice>>> GetBookPricesAsync(IEnumerable<string> symbols, CancellationToken ct = default)
        {
            foreach (var symbol in symbols)
                symbol.ValidateMexcSymbol();
            var parameters = new Dictionary<string, object> { { "symbols", $"[{string.Join(",", symbols.Select(s => $"\"{s}\""))}]" } };

            WebCallResult<IEnumerable<MexcV3BookPrice>>? response = await _baseClient.MexcV3SendRequestInternal<IEnumerable<MexcV3BookPrice>>(
                uri: _baseClient.GetUrl(bookPricesEndpoint, api, publicVersion),
                method: HttpMethod.Get,
                cancellationToken: ct,
                parameters: parameters,
                weight: 2).ConfigureAwait(false);
            return response;
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<MexcV3BookPrice>>> GetBookPricesAsync(CancellationToken ct = default)
        {
            WebCallResult<IEnumerable<MexcV3BookPrice>>? response = await _baseClient.MexcV3SendRequestInternal<IEnumerable<MexcV3BookPrice>>(
                uri: _baseClient.GetUrl(bookPricesEndpoint, api, publicVersion),
                method: HttpMethod.Get,
                cancellationToken: ct,
                weight: 2).ConfigureAwait(false);
            return response;
        }

        #endregion
    }
}
