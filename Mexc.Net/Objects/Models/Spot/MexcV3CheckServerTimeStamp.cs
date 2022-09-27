using System;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Mexc.Net.Objects.Models.Spot
{
    /// <summary>
    /// Server Time
    /// </summary>
    public class MexcV3CheckServerTimeStamp
    {
        /// <summary>
        /// Server Time
        /// </summary>
        [JsonProperty("serverTime")]
        public long ServerTime { get; set; }
    }
}