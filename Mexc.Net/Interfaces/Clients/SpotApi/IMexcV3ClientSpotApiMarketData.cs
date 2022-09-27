using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mexc.Net.Enums;
using Mexc.Net.Objects.Models.Spot;
using CryptoExchange.Net.Objects;

namespace Mexc.Net.Interfaces.Clients.SpotApi
{
    /// <summary>
    /// Mexc Spot Market Data Endpoints.
    /// </summary>
    public interface IMexcV3ClientSpotApiMarketData
    {
        /// <summary>
        /// Test Connectivity
        /// 测试服务器连通性 Test Connectivity
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_en/#test-connectivity" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_cn/#16073bbcf1" /></para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>True if successful ping, false if no response</returns>
        Task<WebCallResult<long>> PingAsync(CancellationToken ct = default);

        /// <summary>
        /// Check Server Time Stamp
        /// 获取服务器时间戳
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_en/#check-server-time" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_cn/#3f1907847c" /></para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Server time</returns>
        Task<WebCallResult<long>> GetServerTimeStampAsync(CancellationToken ct = default);

        /// <summary>
        /// Check Server Time
        /// 获取服务器时间
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_en/#check-server-time" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_cn/#3f1907847c" /></para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Server time</returns>
        Task<WebCallResult<DateTime>> GetServerTimeAsync(CancellationToken ct = default);

        /// <summary>
        /// Exchange Information
        /// 交易规范信息(包含指定单一交易代码信息）
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_en/#exchange-information" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_cn/#e7746f7d60" /></para>
        /// </summary>
        /// <param name="symbol">Symbol to get data for token</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Exchange info</returns>
        Task<WebCallResult<MexcV3ExchangeInfo>> GetExchangeInfoAsync(string symbol, CancellationToken ct = default);

        /// <summary>
        /// Exchange Information
        /// 交易规范信息(包含指定多个交易代码信息）
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_en/#exchange-information" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_cn/#e7746f7d60" /></para>
        /// </summary>
        /// <param name="symbols">Symbols to get data for token</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Exchange info</returns>
        Task<WebCallResult<MexcV3ExchangeInfo>> GetExchangeInfoAsync(IEnumerable<string> symbols, CancellationToken ct = default);

        /// <summary>
        /// Exchange Information
        /// 交易规范信息(包含所有交易代码信息）
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_en/#exchange-information" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_cn/#e7746f7d60" /></para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Exchange info</returns>
        Task<WebCallResult<MexcV3ExchangeInfo>> GetExchangeInfoAsync(CancellationToken ct = default);

        /// <summary>
        /// Order Book
        /// 深度信息
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_en/#order-book" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_cn/#38a975b802" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to get the order book for</param>
        /// <param name="limit">Max number of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The order book for the symbol</returns>
        Task<WebCallResult<MexcV3OrderBook>> GetOrderBookAsync(string symbol, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Recent Trades List
        /// 近期成交列表 获取指定交易对的近期成交信息，默认返回最近500条成交信息(指定单一交易代码）
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_en/#recent-trades-list" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_cn/#2c5e424c25" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to get recent trades for</param>
        /// <param name="limit">Result limit</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of recent trades</returns>
        Task<WebCallResult<IEnumerable<IMexcV3RecentTrade>>> GetRecentTradesAsync(string symbol, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Old Trade Lookup
        /// 旧交易查询
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_en/#old-trade-lookup" /></para>
        /// <para><a href="无中文说明" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to get recent trades for</param>
        /// <param name="limit">Result limit</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of recent trades</returns>
        Task<WebCallResult<IEnumerable<IMexcV3RecentTrade>>> GetTradeHistoryAsync(string symbol, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Compressed/Aggregate Trades List
        /// Get compressed, aggregate trades. Trades that fill at the time, from the same order, with the same price will have the quantity aggregated.
        /// 近期成交(归集) 归集交易与逐笔交易的区别在于，同一价格、同一方向、同一时间的trade会被聚合为一条
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_en/#compressed-aggregate-trades-list" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_cn/#c59e471e81" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to get the trades for</param>
        /// <param name="startTime">Time to start getting trades from</param>
        /// <param name="endTime">Time to stop getting trades from</param>
        /// <param name="limit">Max number of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The aggregated trades list for the symbol</returns>
        Task<WebCallResult<IEnumerable<IMexcV3AggregatedTrade>>> GetAggregatedTradeHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Kline/Candlestick Data
        /// K线数据
        /// 获取指定交易对的k线数据，每根K线代表一个交易对。每根K线的开盘时间可视为唯一ID。
        /// Kline/candlestick bars for a symbol. Klines are uniquely identified by their open time.
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_en/#kline-candlestick-data" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_cn/#k" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to get the data for</param>
        /// <param name="interval">The candlestick timespan</param>
        /// <param name="startTime">Start time to get candlestick data</param>
        /// <param name="endTime">End time to get candlestick data</param>
        /// <param name="limit">Max number of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The candlestick data for the provided symbol</returns>
        Task<WebCallResult<IEnumerable<IMexcV3Kline>>> GetKlinesAsync(string symbol, MexcV3RestKlineInterval interval,
            DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Current Average Price
        /// 当前平均价格
        /// 获取指定交易对在一定时间范围内的平均价格。
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_en/#current-average-price" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_cn/#3b4f48cdbb" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to get the data for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<MexcV3AveragePrice>> GetCurrentAvgPriceAsync(string symbol, CancellationToken ct = default);

        /// <summary>
        /// 24hr Ticker Price Change Statistics
        /// 24hr 价格变动情况(指定单一交易代码）
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_en/#24hr-ticker-price-change-statistics" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_cn/#24" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to get the data for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Data over the last 24 hours</returns>
        Task<WebCallResult<IMexcV3Tick>> GetTickerAsync(string symbol,
            CancellationToken ct = default);

        /// <summary>
        /// 24hr Ticker Price Change Statistics
        /// 24hr 价格变动情况(指定多个交易代码）
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_en/#24hr-ticker-price-change-statistics" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_cn/#24" /></para>
        /// </summary>
        /// <param name="symbols">The symbols to get the data for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Data over the last 24 hours</returns>
        Task<WebCallResult<IEnumerable<IMexcV3Tick>>> GetTickersAsync(IEnumerable<string> symbols,
            CancellationToken ct = default);

        /// <summary>
        /// 24hr Ticker Price Change Statistics
        /// 24hr 价格变动情况(所有交易代码）
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_en/#24hr-ticker-price-change-statistics" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_cn/#24" /></para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of data over the last 24 hours</returns>
        Task<WebCallResult<IEnumerable<IMexcV3Tick>>> GetTickersAsync(CancellationToken ct = default);

        /// <summary>
        /// Symbol Price Ticker
        /// 最新价格(指定单一交易代码）
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_en/#symbol-price-ticker" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_cn/#8ff46b58de" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to get the price for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Price of symbol</returns>
        Task<WebCallResult<MexcV3Price>> GetPriceAsync(string symbol, CancellationToken ct = default);

        /// <summary>
        ///  Symbol Price Ticker
        ///  最新价格(指定多个交易代码）
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_en/#symbol-price-ticker" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_cn/#8ff46b58de" /></para>
        /// </summary>
        /// <param name="symbols">The symbols to get the price for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of prices</returns>
        Task<WebCallResult<IEnumerable<MexcV3Price>>> GetPricesAsync(IEnumerable<string> symbols, CancellationToken ct = default);

        /// <summary>
        /// Symbol Price Ticker
        /// 最新价格(所有交易代码）
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_en/#symbol-price-ticker" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_cn/#8ff46b58de" /></para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of prices</returns>
        Task<WebCallResult<IEnumerable<MexcV3Price>>> GetPricesAsync(CancellationToken ct = default);

        /// <summary>
        /// Symbol Order Book Ticker
        /// 当前最优挂单(指定单一交易代码）
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_en/#symbol-order-book-ticker" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_cn/#5393cd07b4" /></para>
        /// </summary>
        /// <param name="symbol">Symbol to get book price for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of book prices</returns>
        Task<WebCallResult<MexcV3BookPrice>> GetBookPriceAsync(string symbol, CancellationToken ct = default);

        /// <summary>
        /// Symbol Order Book Ticker
        /// 当前最优挂单(指定多个交易代码）
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_en/#symbol-order-book-ticker" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_cn/#5393cd07b4" /></para>
        /// </summary>
        /// <param name="symbol">Symbol to get book price for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of book prices</returns>
        Task<WebCallResult<IEnumerable<MexcV3BookPrice>>> GetBookPricesAsync(IEnumerable<string> symbol, CancellationToken ct = default);

        /// <summary>
        /// Symbol Order Book Ticker
        /// 当前最优挂单(所有交易代码）
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_en/#symbol-order-book-ticker" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_cn/#5393cd07b4" /></para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of book prices</returns>
        Task<WebCallResult<IEnumerable<MexcV3BookPrice>>> GetBookPricesAsync(CancellationToken ct = default);
    }
}
