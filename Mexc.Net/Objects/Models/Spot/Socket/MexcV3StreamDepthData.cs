using CryptoExchange.Net.Converters;
using Mexc.Net.Objects.Models.Spot.Socket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Mexc.Net.Interfaces
{
    /// <summary>
    /// Stream Depth data
    /// </summary>
    public class MexcV3StreamDepthData : IMexcV3StreamDepthData<MexcV3StreamDepthAskOrBidDetails>
    {
        /// <summary>
        /// The event timestamp of this depth 
        /// 事件时间eventTime
        /// </summary>
        [JsonProperty("E"), JsonConverter(typeof(DateTimeConverter))]
        public DateTime EventTime { get; set; }

        /// <summary>
        /// The event type of this depth data
        /// 事件类型EventType
        /// </summary>
        [JsonProperty("e")]
        public string EventType { get; set; } = string.Empty;

        /// <summary>
        /// The symbol of this depth data
        /// 深度交易代码Symbol
        /// </summary>
        [JsonProperty("s")]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// The version the data is for
        /// 版本信息
        /// </summary>
        [JsonProperty("version")]
        public string Version { get; set; } = string.Empty;

        /// <summary>
        /// Depth Detail Array
        /// </summary>
        [JsonProperty("asks", NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<MexcV3StreamDepthAskOrBidDetails>? Asks { get; set; }

        /// <summary>
        /// Depth Detail Array
        /// </summary>
        [JsonProperty("bids", NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<MexcV3StreamDepthAskOrBidDetails>? Bids { get; set; }
    }
}
