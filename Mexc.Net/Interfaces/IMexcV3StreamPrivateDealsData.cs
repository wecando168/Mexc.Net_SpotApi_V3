using Mexc.Net.Enums;
using System;

namespace Mexc.Net.Interfaces
{
    /// <summary>
    /// Stream Trade data
    /// </summary>
    public interface IMexcV3StreamPrivateDealsData
    {
        /// <summary>
        /// The trade type of the prvate deals
        /// 交易类型 1:买 2:卖
        /// </summary>
        MexcV3SpotSocketAccountOrderTradeType TradeType { get; set; }

        /// <summary>
        /// The time of the deal
        /// 成交时间
        /// </summary>
        DateTime DealTime { get; set; }

        /// <summary>
        /// The client order id of the deal
        /// 用户自定义订单id: clientOrderId
        /// </summary>
        string ClientOrderId { get; set; }

        /// <summary>
        /// The order id of the deal
        /// 订单id: orderId
        /// </summary>
        string OrderId { get; set; }

        /// <summary>
        /// The order is maker or taker?
        /// 是否是挂单: isMaker
        /// </summary>
        MexcV3SpotSocketAccountOrderMakerOrTaker MakerOrTaker { get; set; }

        /// <summary>
        /// The price of the deal
        /// 交易价格
        /// </summary>
        decimal Price { get; set; }

        /// <summary>
        /// The trade is self trade
        /// 是否自成交：isSelfTrade
        /// </summary>
        int IsSelfTrade { get; set; }

        /// <summary>
        /// The trade id of the deal
        /// 成交id: orderId
        /// </summary>
        string TradeId { get; set; }

        /// <summary>
        /// The quantity of the deal
        /// 交易数量
        /// </summary>
        decimal Quantity { get; set; }
    }
}
