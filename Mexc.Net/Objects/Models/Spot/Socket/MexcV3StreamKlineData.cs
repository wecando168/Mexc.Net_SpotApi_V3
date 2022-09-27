using CryptoExchange.Net.Converters;
using Mexc.Net.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Mexc.Net.Interfaces
{
    /// <summary>
    /// Stream kline data
    /// </summary>
    public class MexcV3StreamKlineData: IMexcV3StreamKlineData<MexcV3StreamKlineDetail>
    {
        /// <summary>
        /// The event timestamp of candlestick trade 
        /// 事件时间eventTime
        /// </summary>
        [JsonProperty("E"), JsonConverter(typeof(DateTimeConverter))]
        public DateTime EventTime { get; set; }

        /// <summary>
        /// The k line of this candlestick data
        /// K线数据
        /// </summary>
        [JsonProperty("k")]
        public MexcV3StreamKlineDetail Details { get; set; } = default!;

        /// <summary>
        /// The event type of this candlestick data
        /// 事件类型EventType
        /// </summary>
        [JsonProperty("e")]
        public string EventType { get; set; } = string.Empty;

        /// <summary>
        /// The symbol of this candlestick data
        /// 数据交易代码Symbol
        /// </summary>
        [JsonProperty("s")]
        public string Symbol { get; set; } = string.Empty;
    }
}
