using Mexc.Net.Enums;
using Newtonsoft.Json;
using System;

namespace Mexc.Net.Interfaces
{
    /// <summary>
    /// Stream Trade data
    /// </summary>
    public interface IMexcV3StreamPrivateOrdersData
    {
        /// <summary>
        /// The remain amount
        /// 实际剩余金额
        /// </summary>
        [JsonProperty("A")]
        decimal RemainAmount { get; set; }

        /// <summary>
        /// The create time of the order
        /// 订单创建时间
        /// </summary>
        DateTime CreateTime { get; set; }

        /// <summary>
        /// The order type of the prvate orders
        /// 交易方向 1:买 2:卖
        /// </summary>
        MexcV3SpotSocketAccountOrderTradeType TradeType { get; set; }

        /// <summary>
        /// The remain quantity
        /// 实际剩余数量
        /// </summary>
        decimal RemainQuantity { get; set; }

        /// <summary>
        /// The amount
        /// 下单总金额
        /// </summary>
        decimal Amount { get; set; }

        /// <summary>
        /// The client order id
        /// 用户自定义订单id: clientOrderId
        /// </summary>
        string ClientOrderId { get; set; }

        /// <summary>
        /// The order id
        /// 订单id: orderId
        /// </summary>
        string OrderId { get; set; }

        /// <summary>
        /// The order is maker or taker?
        /// 是否是挂单: isMaker
        /// </summary>
        MexcV3SpotSocketAccountOrderMakerOrTaker MakerOrTaker { get; set; }

        /// <summary>
        /// The order type
        /// 订单类型LIMIT_ORDER(1),POST_ONLY(2),IMMEDIATE_OR_CANCEL(3),FILL_OR_KILL(4),MARKET_ORDER(5); 止盈止损（100）
        /// </summary>
        MexcV3SpotSocketAccountOrderType OrderType { get; set; }

        /// <summary>
        /// The price
        /// 下单价格
        /// </summary>
        decimal Price { get; set; }

        /// <summary>
        /// The order status
        /// 订单状态 1:未成交 2:已成交 3:部分成交 4:已撤单 5:部分撤单
        /// </summary>
        MexcV3SpotSocketAccountOrderStatus OrderStatus { get; set; }

        /// <summary>
        /// The quantity
        /// 下单数量
        /// </summary>
        decimal Quantity { get; set; }
    }
}
