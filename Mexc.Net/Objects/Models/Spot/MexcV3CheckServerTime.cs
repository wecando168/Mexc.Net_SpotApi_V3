using System;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Mexc.Net.Objects.Models.Spot
{
    /// <summary>
    /// Server Time
    /// </summary>
    public class MexcV3CheckServerTime
    {
        /// <summary>
        /// Server Time
        /// </summary>
        [JsonProperty("serverTime"), JsonConverter(typeof(DateTimeConverter))]
        public DateTime ServerTime { get; set; }
    }
}
