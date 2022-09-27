using System;
using System.Collections.Generic;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Mexc.Net.Objects.Models.Spot
{
    /// <summary>
    /// Exchange info
    /// </summary>
    public class MexcV3ExchangeInfo
    {
        /// <summary>
        /// The timezone the server uses
        /// </summary>
        [JsonProperty("timezone")]
        public string TimeZone { get; set; } = string.Empty;

        /// <summary>
        /// The current server time
        /// </summary>
        [JsonProperty("serverTime"), JsonConverter(typeof(DateTimeConverter))]
        public DateTime ServerTime { get; set; }

        /// <summary>
        /// The rate limits used
        /// </summary>
        [JsonProperty("rateLimits")]
        public IEnumerable<MexcRateLimit> RateLimits { get; set; } = Array.Empty<MexcRateLimit>();

        /// <summary>
        /// Filters
        /// </summary>
        [JsonProperty("exchangeFilters")]
        public IEnumerable<object> ExchangeFilters { get; set; } = Array.Empty<object>();

        /// <summary>
        /// All symbols supported
        /// </summary>
        [JsonProperty("symbols")]
        public IEnumerable<MexcV3Symbol> Symbols { get; set; } = Array.Empty<MexcV3Symbol>();


    }
}