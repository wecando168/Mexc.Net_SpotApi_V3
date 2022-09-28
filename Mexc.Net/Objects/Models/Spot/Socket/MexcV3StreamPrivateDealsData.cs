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
    public class MexcV3StreamPrivateDealsData : IMexcV3StreamPrivateDealsData
    {
        /// <summary>
        /// The trade type of the prvate deals（Order Side）
        /// 交易类型 1:买 2:卖
        /// </summary>
        [JsonProperty("S")]
        [JsonConverter(typeof(MexcV3SpotSocketAccountOrderTradeTypeConverter))]
        public MexcV3SpotSocketAccountOrderTradeType TradeType { get; set; }

        /// <summary>
        /// The time of the deal
        /// 成交时间
        /// </summary>
        [JsonProperty("T")]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime DealTime { get; set; }

        /// <summary>
        /// The client order id of the deal
        /// 用户自定义订单id: clientOrderId
        /// </summary>
        [JsonProperty("c")]
        public string ClientOrderId { get; set; } = string.Empty;

        /// <summary>
        /// The order id of the deal
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
        /// The price of the deal
        /// 交易价格
        /// </summary>
        [JsonProperty("p")]
        public decimal Price { get; set; }

        /// <summary>
        /// The trade is self trade
        /// 是否自成交：isSelfTrade
        /// </summary>
        [JsonProperty("st")]
        public int IsSelfTrade { get; set; }

        /// <summary>
        /// The trade id of the deal
        /// 成交id: orderId
        /// </summary>
        [JsonProperty("t")]
        public string TradeId { get; set; } = string.Empty;

        /// <summary>
        /// The quantity of the deal
        /// 交易数量
        /// </summary>
        [JsonProperty("v")]
        public decimal Quantity { get; set; }
    }
}
