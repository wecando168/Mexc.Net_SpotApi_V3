using System;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Mexc.Net.Objects.Models
{
    /// <summary>
    /// A event received by a Mexc websocket
    /// </summary>
    public class MexcStreamEvent
    {
        /// <summary>
        /// The type of the event
        /// </summary>
        [JsonProperty("e")]
        public string Event { get; set; } = string.Empty;
        /// <summary>
        /// The time the event happened
        /// </summary>
        [JsonProperty("E"), JsonConverter(typeof(DateTimeConverter))]
        public DateTime EventTime { get; set; }
    }
}
