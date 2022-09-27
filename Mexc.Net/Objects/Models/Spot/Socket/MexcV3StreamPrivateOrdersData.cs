using CryptoExchange.Net.Converters;
using Mexc.Net.Converters;
using Mexc.Net.Enums;
using Newtonsoft.Json;
using System;

namespace Mexc.Net.Interfaces
{
    /// <summary>
    /// Stream deal data
    /// 账户成交
    /// </summary>
    public class MexcV3StreamPrivateOrdersData : IMexcV3StreamPrivateOrdersData
    {
        /// <summary>
        /// The remain amount
        /// 实际剩余金额
        /// </summary>
        [JsonProperty("A")]
        public decimal RemainAmount { get; set; }

        /// <summary>
        /// The create time of the order
        /// 订单创建时间
        /// </summary>
        [JsonProperty("O")]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// The order type of the prvate orders
        /// 交易方向 1:买 2:卖
        /// </summary>
        [JsonProperty("S"), JsonConverter(typeof(MexcV3SpotSocketAccountOrderTradeTypeConverter))]
        public MexcV3SpotSocketAccountOrderTradeType TradeType { get; set; }

        /// <summary>
        /// The remain quantity
        /// 实际剩余数量
        /// </summary>
        [JsonProperty("V")]
        public decimal RemainQuantity { get; set; }

        /// <summary>
        /// The amount
        /// 下单总金额
        /// </summary>
        [JsonProperty("a")]
        public decimal Amount { get; set; }

        /// <summary>
        /// The client order id
        /// 用户自定义订单id: clientOrderId
        /// </summary>
        [JsonProperty("c")]
        public string ClientOrderId { get; set; } = string.Empty;

        /// <summary>
        /// The order id
        /// 订单id: orderId
        /// </summary>
        [JsonProperty("i")]
        public string OrderId { get; set; } = string.Empty;

        /// <summary>
        /// The order is maker?
        /// 是否是挂单: isMaker
        /// </summary>
        [JsonProperty("m"), JsonConverter(typeof(MexcV3SpotSocketAccountOrderMakerOrTakerConverter))]
        public MexcV3SpotSocketAccountOrderMakerOrTaker MakerOrTaker { get; set; }

        /// <summary>
        /// The order type
        /// 订单类型 LIMIT_ORDER(1),POST_ONLY(2),IMMEDIATE_OR_CANCEL(3),FILL_OR_KILL(4),MARKET_ORDER(5); 止盈止损（100）
        /// </summary>
        [JsonProperty("o"), JsonConverter(typeof(MexcV3SpotSocketAccountOrderTypeConverter))]
        public MexcV3SpotSocketAccountOrderType OrderType { get; set; }

        /// <summary>
        /// The price
        /// 下单价格
        /// </summary>
        [JsonProperty("p")]
        public decimal Price { get; set; }

        /// <summary>
        /// The order status
        /// 订单状态 1:未成交 2:已成交 3:部分成交 4:已撤单 5:部分撤单
        /// </summary>
        [JsonProperty("s"), JsonConverter(typeof(MexcV3SpotSocketAccountOrderStatusConverter))]
        public MexcV3SpotSocketAccountOrderStatus OrderStatus { get; set; }

        /// <summary>
        /// The quantity
        /// 下单数量
        /// </summary>
        [JsonProperty("v")]
        public decimal Quantity { get; set; }
    }
}
