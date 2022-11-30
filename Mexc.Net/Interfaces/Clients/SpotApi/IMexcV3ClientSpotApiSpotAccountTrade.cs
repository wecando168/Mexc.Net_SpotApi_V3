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
    /// Spot Account/Trade Endpoints
    /// </summary>
    public interface IMexcV3ClientSpotApiSpotAccountTrade
    {
        /// <summary>
        /// Test New Order
        /// 测试下单 (TRADE) 用于测试订单请求，但不会提交到撮合引擎
        /// <para><a href="POST https://api.mexc.com/api/v3/order/test" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_en/#test-new-order" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_cn/#711c4ca31b" /></para>
        /// </summary>
        /// <param name="symbol">The symbol the order is for 订单的交易代码</param>
        /// <param name="side">The order side (buy/sell) 订单方（买/卖）</param>
        /// <param name="type">The order type (limit/market) 订单类型（限价/市价）</param>
        /// <param name="quantity">The quantity of the symbol 基础币数量</param>
        /// <param name="quoteQuantity">The quantity of the quote symbol. Only valid for market orders 报价币的数量。 仅对市价单有效</param>
        /// <param name="price">The price to use 使用价格</param>
        /// <param name="newClientOrderId">Unique id for order 订单的唯一 ID</param>        
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request 此请求处于活动状态的接收窗口。 当请求的完成时间超过此时间时，服务器将拒绝该请求</param>
        /// <param name="ct">Cancellation token 取消令牌</param>
        /// <returns>成功则返回一个没有任何内容的结构体</returns>
        Task<WebCallResult<MexcV3PlacedTestOrderResponse>> PlaceTestOrderAsync(
            string symbol,
            OrderSide side,
            SpotOrderType type,
            decimal? quantity = null,
            decimal? quoteQuantity = null,
            decimal? price = null,
            string? newClientOrderId = null,
            int? receiveWindow = null,
            CancellationToken ct = default);

        /// <summary>
        /// New Order
        /// 下单 (TRADE)
        /// <para><a href="POST https://api.mexc.com/api/v3/order" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_en/#new-order" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_cn/#fd6ce2a756" /></para>
        /// </summary>
        /// <param name="symbol">The symbol the order is for 订单的交易代码</param>
        /// <param name="side">The order side (buy/sell) 订单方（买/卖）</param>
        /// <param name="type">The order type (limit/market) 订单类型（限价/市价）</param>
        /// <param name="quantity">The quantity of the symbol 基础币数量</param>
        /// <param name="quoteQuantity">The quantity of the quote symbol. Only valid for market orders 报价币的数量。 仅对市价单有效</param>
        /// <param name="price">The price to use 使用价格</param>
        /// <param name="newClientOrderId">Unique id for order 订单的唯一ID</param>        
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request 此请求处于活动状态的接收窗口。 当请求的完成时间超过此时间时，服务器将拒绝该请求</param>
        /// <param name="ct">Cancellation token 取消令牌</param>
        /// <returns>成功则返回交易代码跟订单编号等新订单信息</returns>
        Task<WebCallResult<MexcV3PlacedOrderResponse>> PlaceOrderAsync(
            string symbol,
            OrderSide side,
            SpotOrderType type,
            decimal? quantity = null,
            decimal? quoteQuantity = null,
            decimal? price = null,
            string? newClientOrderId = null,
            int? receiveWindow = null,
            CancellationToken ct = default);

        /// <summary>
        /// Batch Orders
        /// 批量下单 (TRADE)
        /// <para><a href="POST https://api.mexc.com/api/v3/batchOrders" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_en/#batch-orders" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_cn/#de93fae07b" /></para>
        /// </summary>
        /// <param name="mexcV3BatchPlacedOrderRequestList">订单列表，最多支持20个订单(list of JSON格式填写订单参数,参考请求示例)</param>
        /// <param name="receiveWindow">时间窗</param>
        /// <param name="ct">CancellationToken</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<MexcV3BatchPlacedOrderResponse>>> BatchPlaceOrderAsync(
            IEnumerable<MexcV3SubmitOrder> mexcV3BatchPlacedOrderRequestList,
            int? receiveWindow = null,
            CancellationToken ct = default);

        /// <summary>
        /// Cancel Orde
        /// 撤销订单 (TRADE)
        /// <para><a href="DELETE https://api.mexc.com/api/v3/order" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_en/#cancel-orde" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_cn/#4e53c0fccd" /></para>
        /// </summary>
        /// <param name="symbol">The symbol the order is for</param>
        /// <param name="orderId">The order id of the order</param>
        /// <param name="origClientOrderId">The client order id of the order</param>
        /// <param name="newClientOrderId">Unique identifier for this cancel</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Id's for canceled order</returns>
        Task<WebCallResult<MexcV3CancelOrderResponse>> CancelOrderAsync(
            string symbol,
            string? orderId = null,
            string? origClientOrderId = null,
            string? newClientOrderId = null,
            int? receiveWindow = null,
            CancellationToken ct = default);

        /// <summary>
        /// Cancel all Open Orders on a Symbol
        /// 撤销单一交易对的所有挂单 (TRADE) 
        /// 撤销单一交易对下所有挂单, 包括OCO的挂单。
        /// Cancel all pending orders for a single symbol, including OCO pending orders.
        /// <para><a href="DELETE https://api.mexc.com/api/v3/openOrders" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_en/#cancel-all-open-orders-on-a-symbol" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_cn/#59db31bea9" /></para>
        /// </summary>
        /// <param name="symbol">The symbol the order is for</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Id's for canceled order</returns>
        Task<WebCallResult<IEnumerable<MexcV3CancelOrderResponse>>> CancelOpenOrdersAsync(
            string symbol,
            int? receiveWindow = null,
            CancellationToken ct = default);

        /// <summary>
        /// Query Order
        /// 查询订单 (USER_DATA) Check an order's status. 
        /// 至少需要发送 orderId 与 origClientOrderId中的一个
        /// 查询指定交易对订单状态，最多查询7天内的订单记录，超过7天的可在web客户端查看和导出。
        /// <para><a href="GET https://api.mexc.com/api/v3/order" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_en/#query-order" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_cn/#90376e83a0" /></para>
        /// </summary>
        /// <param name="symbol">The symbol the order is for</param>
        /// <param name="orderId">The order id of the order</param>
        /// <param name="origClientOrderId">The client order id of the order</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The specific order</returns>
        Task<WebCallResult<MexcV3GetOrderResponse>> GetOrderAsync(string symbol, string? orderId = null, string? origClientOrderId = null, long? receiveWindow = null, CancellationToken ct = default);

        /// <summary>
        /// Current Open Orders
        /// 当前挂单 (USER_DATA) 获取交易对的当前挂单
        /// 获取当前挂单支持查询多交易对，每次最多可以传5个symbol
        /// 若批量查5个交易对，最多也只返回1000条挂单
        /// <para><a href="GET https://api.mexc.com/api/v3/openOrders" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_en/#current-open-orders" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_cn/#066ca582c9" /></para>
        /// </summary>
        /// <param name="symbols">The symbolList to get open orders for</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of open orders</returns>
        Task<WebCallResult<IEnumerable<IMexcV3GetOrderResponse>>> GetOpenOrdersAsync(IEnumerable<string> symbols, int? receiveWindow = null, CancellationToken ct = default);

        /// <summary>
        /// All Orders
        /// 查询所有订单 (USER_DATA)（指定单一交易代码）
        /// 状态； 有效，已取消或已完成，最多查询最近7天数据。
        /// <para><a href="GET https://api.mexc.com/api/v3/allOrders" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_en/#all-orders" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_cn/#881e865963" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to get orders for</param>
        /// <param name="startTime">If set, only orders placed after this time will be returned</param>
        /// <param name="endTime">If set, only orders placed before this time will be returned</param>
        /// <param name="limit">Max number of results</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of orders</returns>
        Task<WebCallResult<IEnumerable<MexcV3GetOrderResponse>>> GetOrdersAsync(
            string symbol, 
            DateTime? startTime = null, 
            DateTime? endTime = null, 
            int? limit = null, int? 
            receiveWindow = null, 
            CancellationToken ct = default);

        /// <summary>
        /// Account Information
        /// 账户信息 (USER_DATA) Account Information
        /// 获取当前账户信息，限速2次每秒。
        /// <para><a href="GET https://api.mexc.com/api/v3/account" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_en/#account-information" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_cn/#bd9157656f" /></para>
        /// </summary>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The account information</returns>
        Task<WebCallResult<MexcV3AccountInfo>> GetAccountInfoAsync(long? receiveWindow = null, CancellationToken ct = default);

        /// <summary>
        /// Account Trade List
        /// 账户成交历史 (USER_DATA) 获取账户指定交易对的成交历史
        /// <para><a href="GET https://api.mexc.com/api/v3/myTrades" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_en/#account-trade-list" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_cn/#1c077e2313" /></para>
        /// </summary>
        /// <param name="symbol">Symbol to get trades for</param>
        /// <param name="orderId">Get trades for this order id</param>
        /// <param name="limit">The max number of results</param>
        /// <param name="fromId">TradeId to fetch from. Default gets most recent trades</param>
        /// <param name="startTime">Orders newer than this date will be retrieved</param>
        /// <param name="endTime">Orders older than this date will be retrieved</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of trades</returns>
        Task<WebCallResult<IEnumerable<MexcV3Trade>>> GetUserTradesAsync(string symbol, string? orderId = null, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, long? fromId = null, long? receiveWindow = null, CancellationToken ct = default);
    }
}
