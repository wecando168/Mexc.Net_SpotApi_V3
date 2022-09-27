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
    /// Depth ask or bid details
    /// </summary>
    public class MexcV3StreamDepthAskOrBidDetails : IMexcV3StreamDepthAskOrBidDetails
    {
        /// <summary>
        /// The price of this depth ask or bid details
        /// </summary>
        [JsonProperty("p")]
        public decimal Price { get; set; }

        /// <summary>
        /// The quantity of this depth ask or bid details
        /// </summary>
        [JsonProperty("v")]
        public decimal Quantity { get; set; }
    }
}
