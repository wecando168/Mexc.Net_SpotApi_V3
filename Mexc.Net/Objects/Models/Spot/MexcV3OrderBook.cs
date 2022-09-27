using System;
using System.Collections.Generic;
using Mexc.Net.Interfaces;
using Newtonsoft.Json;

namespace Mexc.Net.Objects.Models.Spot
{
    /// <summary>
    /// The order book for a asset
    /// </summary>
    public class MexcV3OrderBook : IMexcV3OrderBook
    {
        /// <summary>
        /// The symbol of the order book 
        /// </summary>
        [JsonProperty("s")]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// The ID of the last update
        /// </summary>
        [JsonProperty("lastUpdateId")]
        public long LastUpdateId { get; set; }
        
        /// <summary>
        /// The list of bids
        /// </summary>
        public IEnumerable<MexcV3OrderBookEntry> Bids { get; set; } = Array.Empty<MexcV3OrderBookEntry>();

        /// <summary>
        /// The list of asks
        /// </summary>
        public IEnumerable<MexcV3OrderBookEntry> Asks { get; set; } = Array.Empty<MexcV3OrderBookEntry>();
    }
}
