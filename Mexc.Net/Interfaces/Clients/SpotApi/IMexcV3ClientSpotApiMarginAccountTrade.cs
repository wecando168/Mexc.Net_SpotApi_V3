using System.Threading;
using System.Threading.Tasks;
using Mexc.Net.Enums;
using Mexc.Net.Objects.Models.Spot.Margin;
using CryptoExchange.Net.Objects;

namespace Mexc.Net.Interfaces.Clients.SpotApi
{
    /// <summary>
    /// Margin Account and Trading Interface Endpoints
    /// </summary>
    public interface IMexcV3ClientSpotApiMarginAccountTrade
    {
        /// <summary>
        /// switch trade mode of margin account
        /// 切换杠杆账户模式
        /// <para><a href="POST https://api.mexc.com/api/v3/margin/tradeMode" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_en/#trademode" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_cn/#3294f5a23a" /></para>
        /// </summary>
        /// <param name="symbol">The symbol the order is for 订单的交易代码</param>
        /// <param name="tradeMode">交易模式 0: 手动模式 1:自动借还模式</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request 此请求处于活动状态的接收窗口。 当请求的完成时间超过此时间时，服务器将拒绝该请求</param>
        /// <param name="ct">Cancellation token 取消令牌</param>
        /// <returns></returns>
        Task<WebCallResult<MexcV3MarginTradeModeResponse>> MarginTradeModeAsync(
            string symbol,
            int tradeMode,
            int? receiveWindow = null,
            CancellationToken ct = default);

        /// <summary>
        /// New Margin Order
        /// 杠杆账户下单 (TRADE)
        /// <para><a href="POST https://api.mexc.com/api/v3/margin/order" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_en/#place-order" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_cn/#fd6ce2a756-2" /></para>
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
        Task<WebCallResult<MexcV3MarginPlacedOrderResponse>> MarginPlaceOrderAsync(
            string symbol,
            OrderSide side,
            SpotOrderType type,
            decimal? quantity = null,
            decimal? quoteQuantity = null,
            decimal? price = null,
            string? newClientOrderId = null,
            int? receiveWindow = null,
            CancellationToken ct = default);
    }
}
