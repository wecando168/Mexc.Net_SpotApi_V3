using Mexc.Net.Enums;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mexc.Net.Objects.Models.Spot.Socket;

namespace Mexc.Net.Interfaces.Clients.SpotApi
{
    /// <summary>
    /// Mexc Spot streams
    /// </summary>
    public interface IMexcV3SocketClientSpotStreams : IDisposable
    {        
        /// <summary>
        /// 逐笔交易(实时) Trade Streams
        /// The Trade Streams push raw trade information; each trade has a unique buyer and seller.
        /// 逐笔交易推送每一笔成交的信息。成交，或者说交易的定义是仅有一个吃单者与一个挂单者相互交易
        /// 逐笔交易（指定单一交易代码） 逐笔交易推送每一笔成交的信息。成交，或者说交易的定义是仅有一个吃单者与一个挂单者相互交易
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_en/#trade-streams" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_cn/#2947d06b59" /></para>
        /// </summary>
        /// <param name="symbol">The symbol</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToTradeUpdatesAsync(string symbol,
            Action<DataEvent<MexcV3StreamTrade>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// 逐笔交易(实时) Trade Streams
        /// The Trade Streams push raw trade information; each trade has a unique buyer and seller.
        /// 逐笔交易推送每一笔成交的信息。成交，或者说交易的定义是仅有一个吃单者与一个挂单者相互交易
        /// 逐笔交易（指定多个交易代码） 逐笔交易推送每一笔成交的信息。成交，或者说交易的定义是仅有一个吃单者与一个挂单者相互交易
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_en/#trade-streams" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_cn/#2947d06b59" /></para>
        /// </summary>
        /// <param name="symbols">The symbol array</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToTradeUpdatesAsync(IEnumerable<string> symbols,
            Action<DataEvent<MexcV3StreamTrade>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// K线 Streams Kline Streams
        /// K线逐秒推送所请求的K线种类(最新一根K线)的更新。
        /// The Kline/Candlestick Stream push updates to the current klines/candlestick every second.
        /// K线 Streams（指定单一交易代码，指定单一时间间隔）  K线stream逐秒推送所请求的K线种类(最新一根K线)的更新。
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_en/#kline-streams" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_cn/#k-streams" /></para>
        /// </summary>
        /// <param name="symbol">The symbol 交易代码</param>
        /// <param name="interval">The interval of the candlesticks K线的时间间隔</param>
        /// <param name="onMessage">The event handler for the received data 接收数据的事件处理程序</param>
        /// <param name="ct">Cancellation token for closing this subscription 用于关闭此订阅的取消令牌</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToKlineUpdatesAsync(string symbol, MexcV3StreamsKlineInterval interval, 
            Action<DataEvent<MexcV3StreamKline>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// K线 Streams Kline Streams
        /// K线逐秒推送所请求的K线种类(最新一根K线)的更新。
        /// The Kline/Candlestick Stream push updates to the current klines/candlestick every second.
        /// K线 Streams（指定单一交易代码，指定多个时间间隔）  K线stream逐秒推送所请求的K线种类(最新一根K线)的更新。
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_en/#kline-streams" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_cn/#k-streams" /></para>
        /// </summary>
        /// <param name="symbol">The symbol 交易代码</param>
        /// <param name="intervals">The interval of the candlesticks K线的时间间隔</param>
        /// <param name="onMessage">The event handler for the received data 接收数据的事件处理程序</param>
        /// <param name="ct">Cancellation token for closing this subscription 用于关闭此订阅的取消令牌</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToKlineUpdatesAsync(string symbol, IEnumerable<MexcV3StreamsKlineInterval> intervals, 
            Action<DataEvent<MexcV3StreamKline>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// K线 Streams Kline Streams
        /// K线逐秒推送所请求的K线种类(最新一根K线)的更新。
        /// The Kline/Candlestick Stream push updates to the current klines/candlestick every second.
        /// K线 Streams（指定多个交易代码，指定单一时间间隔）  K线stream逐秒推送所请求的K线种类(最新一根K线)的更新。
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_en/#kline-streams" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_cn/#k-streams" /></para>
        /// </summary>
        /// <param name="symbols">The symbol 交易代码</param>
        /// <param name="interval">The interval of the candlesticks K线的时间间隔</param>
        /// <param name="onMessage">The event handler for the received data 接收数据的事件处理程序</param>
        /// <param name="ct">Cancellation token for closing this subscription 用于关闭此订阅的取消令牌</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToKlineUpdatesAsync(IEnumerable<string> symbols, MexcV3StreamsKlineInterval interval, 
            Action<DataEvent<MexcV3StreamKline>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// K线 Streams Kline Streams
        /// K线逐秒推送所请求的K线种类(最新一根K线)的更新。
        /// The Kline/Candlestick Stream push updates to the current klines/candlestick every second.
        /// K线 Streams（指定多个交易代码，指定多个时间间隔）  K线stream逐秒推送所请求的K线种类(最新一根K线)的更新。
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_en/#kline-streams" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_cn/#k-streams" /></para>
        /// </summary>
        /// <param name="symbols">The symbol 交易代码</param>
        /// <param name="intervals">The interval of the candlesticks K线的时间间隔</param>
        /// <param name="onMessage">The event handler for the received data 接收数据的事件处理程序</param>
        /// <param name="ct">Cancellation token for closing this subscription 用于关闭此订阅的取消令牌</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToKlineUpdatesAsync(IEnumerable<string> symbols, IEnumerable<MexcV3StreamsKlineInterval> intervals, 
            Action<DataEvent<MexcV3StreamKline>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// 增量深度信息(实时)
        /// Subscribes to the order book updates for the provided symbol
        /// 如果某个价格对应的挂单量(v)为0，表示该价位的挂单已经撤单或者被吃，应该移除这个价位。
        /// 增量深度信息（指定单一交易代码） 每秒或每100毫秒推送orderbook的变化部分(如果有)
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_en/#diff-depth-stream" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_cn/#efff33c875" /></para>
        /// </summary>
        /// <param name="symbol">The symbol</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToDiffDepthUpdatesAsync(string symbol, 
            Action<DataEvent<MexcV3StreamDepth>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// 增量深度信息(实时)
        /// Subscribes to the depth update stream for the provided symbols
        /// 如果某个价格对应的挂单量(v)为0，表示该价位的挂单已经撤单或者被吃，应该移除这个价位。
        /// 增量深度信息（指定多个交易代码） 每秒或每100毫秒推送orderbook的变化部分(如果有)
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_en/#diff-depth-stream" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_cn/#efff33c875" /></para>
        /// </summary>
        /// <param name="symbols">The symbols</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToDiffDepthUpdatesAsync(IEnumerable<string> symbols, 
            Action<DataEvent<MexcV3StreamDepth>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribes to the private deals update stream. Prior to using this, the MexcClient.SpotApi.WebsocketAccount.StartUserStreamAsync method should be called.
        /// 订阅帐户成交更新流。 在使用它之前，应该调用 MexcClient.SpotApi.WebsocketAccount.StartUserStreamAsync 方法(先生成 Listen Key (USER_STREAM) 再开始一个新的数据流）
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_en/#account-deals" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_cn/#444af6a5fa" /></para>
        /// </summary>
        /// <param name="listenKey">Listen key retrieved by the StartUserStream method</param>
        /// <param name="onMessage">The event handler for whenever an private deals update is received</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToPrivateDealsUpdatesAsync(string listenKey, 
            Action<DataEvent<MexcV3StreamPrivateDeals>>? onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribes to the private orders update stream. Prior to using this, the MexcClient.SpotApi.WebsocketAccount.StartUserStreamAsync method should be called.
        /// 订阅帐户订单更新流。 在使用它之前，应该调用 MexcClient.SpotApi.WebsocketAccount.StartUserStreamAsync 方法(先生成 Listen Key (USER_STREAM) 再开始一个新的数据流）
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_cn/#ef28329b2a" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_en/#account-orders" /></para>
        /// </summary>
        /// <param name="listenKey">Listen key retrieved by the StartUserStream method</param>
        /// <param name="onMessage">The event handler for whenever an private orders update is received</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToPrivateOrdersUpdatesAsync(string listenKey, 
            Action<DataEvent<MexcV3StreamPrivateOrders>>? onMessage, CancellationToken ct = default);
    }
}
