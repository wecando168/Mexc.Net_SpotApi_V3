using Mexc.Net.Converters;
using Newtonsoft.Json;
using Mexc.Net.Interfaces;
using CryptoExchange.Net.Converters;
using System;

namespace Mexc.Net.Objects.Models.Spot.Socket
{
    /// <summary>
    /// Update data about an order
    /// </summary>
    public class MexcV3StreamPrivateOrders : IMexcV3Stream<MexcV3StreamPrivateOrdersData>
    {
        /// <summary>
        /// ListenKey
        /// 私有数据监听密钥
        /// </summary>
        public string ListenKey { get; set; } = string.Empty;

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
        public MexcV3StreamPrivateOrdersData Data { get; set; } = default!;

        /// <summary>
        /// 数据流交易代码Symbol
        /// </summary>
        [JsonProperty("s")]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// 交易代码显示名称(即将废弃）
        /// </summary>
        [JsonProperty("sd", NullValueHandling = NullValueHandling.Ignore)]
        public string? SymbolDisplay { get; set; } = string.Empty;

        /// <summary>
        /// The time the event happened
        /// 事件时间EventTime
        /// </summary>
        [JsonProperty("eventTime",NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime EventTime { get; set; }

        /// <summary>
        /// The time the event happened
        /// 事件的时间戳EventTime
        /// </summary>
        [JsonProperty("t")]
        public string? EventTimeStamp { get; set; }

    }
}
