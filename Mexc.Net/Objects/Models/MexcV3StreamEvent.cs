using Newtonsoft.Json;

namespace Mexc.Net.Objects.Models
{
    /// <summary>
    /// A event received by a Mexc websocket
    /// </summary>
    public class MexcV3StreamEvent
    {
        /// <summary>
        /// The Subscription request
        /// 订阅频道（可作为当前请求客户端标识）
        /// </summary>
        [JsonProperty("c")]
        public string Stream { get; set; } = string.Empty;

        /// <summary>
        /// The time the event happened
        /// 事件时间eventTime
        /// </summary>
        /// [JsonProperty("t"), JsonConverter(typeof(DateTimeConverter))]
        [JsonProperty("t")]
        public string? EventTime { get; set; }
    }
}
