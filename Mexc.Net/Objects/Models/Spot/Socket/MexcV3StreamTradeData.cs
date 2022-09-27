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
    /// Stream Trade data
    /// 逐比交易数据流返回的实际数据
    /// </summary>
    public class MexcV3StreamTradeData: IMexcV3StreamTradeData<MexcV3StreamTradeDeal>
    {
        /// <summary>
        /// The event timestamp of this trade 
        /// 事件时间eventTime
        /// </summary>
        [JsonProperty("E"), JsonConverter(typeof(DateTimeConverter))]
        public DateTime EventTime { get; set; }

        /// <summary>
        /// The deals of this trade data
        /// 交易明细
        /// </summary>
        [JsonProperty("deals")]
        public IEnumerable<MexcV3StreamTradeDeal> Deals { get; set; } = default!;

        /// <summary>
        /// The event type of this trade data
        /// 事件类型EventType
        /// </summary>
        [JsonProperty("e")]
        public string EventType { get; set; } = string.Empty;

        /// <summary>
        /// The symbol of this trade data
        /// 数据交易代码Symbol
        /// </summary>
        [JsonProperty("s")]
        public string Symbol { get; set; } = string.Empty;
    }
}
