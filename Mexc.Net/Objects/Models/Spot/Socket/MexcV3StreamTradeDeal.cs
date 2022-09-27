using CryptoExchange.Net.Converters;
using Mexc.Net.Converters;
using Mexc.Net.Enums;
using Mexc.Net.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Mexc.Net.Objects.Models.Spot.Socket
{
    /// <summary>
    /// Stream Trade data deals
    /// 逐比交易数据流返回的实际数据交易明细列表
    /// </summary>
    public class MexcV3StreamTradeDeal : IMexcV3StreamTradeDeal
    {
        /// <summary>
        /// The trade type of the deal
        /// 交易类型TradeType
        /// </summary>
        [JsonProperty("S"), JsonConverter(typeof(MexcV3SpotSocketAccountOrderTradeTypeConverter))]
        public MexcV3SpotSocketAccountOrderTradeType TradeType { get; set; } = default!;

        /// <summary>
        /// The price of the deal
        /// 成交价格Price
        /// </summary>
        [JsonProperty("p")]
        public decimal Price { get; set; }

        /// <summary>
        /// The time of the deal
        /// 成交时间DealTime
        /// </summary>
        [JsonProperty("t")]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime DealTime { get; set; }

        /// <summary>
        /// The quantity of the deal
        /// 成交数量Quantity
        /// </summary>
        [JsonProperty("v")]
        public decimal Quantity { get; set; }
    }
}
