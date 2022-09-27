using Mexc.Net.Converters;
using Mexc.Net.Interfaces;
using Newtonsoft.Json;
using System;

namespace Mexc.Net.Objects.Models.Spot.Socket
{
    /// <summary>
    /// Stream kline data
    /// </summary>
    public class MexcV3StreamKline : IMexcV3Stream<MexcV3StreamKlineData>
    {
        /// <summary>
        /// The Subscription request
        /// 订阅频道（可作为当前请求客户端标识）
        /// </summary>
        [JsonProperty("c")]
        public string Stream { get; set; } = string.Empty;

        /// <summary>
        /// 数据
        /// </summary>
        [JsonProperty("d")]
        public MexcV3StreamKlineData Data { get; set; } = default!;

        /// <summary>
        /// 数据流交易代码Symbol
        /// </summary>
        [JsonProperty("s")]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// 交易代码显示名称(即将废弃）
        /// </summary>
        [JsonProperty("sd",NullValueHandling = NullValueHandling.Ignore)]
        public string? SymbolDisplay { get; set; } = string.Empty;

        /// <summary>
        /// The time the event happened
        /// 事件时间eventTime
        /// </summary>
        [JsonProperty("t")]
        public string? EventTimeStamp { get; set; }
    }
}
