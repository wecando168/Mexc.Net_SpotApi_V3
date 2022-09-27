using System;
using System.Collections.Generic;
using Mexc.Net.Interfaces;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Mexc.Net.Objects.Models.Spot
{
    /// <summary>
    /// Stream order book
    /// </summary>
    public class MexcV3EventOrderBook: MexcV3OrderBook, IMexcV3EventOrderBook
    {
        /// <summary>
        /// The id of this update, can be synced with MexcClient.Spot.GetOrderBook to update the order book
        /// </summary>
        [JsonProperty("U")]
        public long? FirstUpdateId { get; set; }

        /// <summary>
        /// Setter for last update id, need for Json.Net
        /// </summary>
        [JsonProperty("u")]
        internal long LastUpdateIdStream { set => LastUpdateId = value; }

        /// <summary>
        /// Event type
        /// </summary>
        [JsonProperty("e")]
        internal string EventType { get; set; } = string.Empty;

        /// <summary>
        /// Event time of the update
        /// </summary>
        [JsonProperty("E"), JsonConverter(typeof(DateTimeConverter))]
        public DateTime EventTime { get; set; }
        
        /// <summary>
        /// Setter for bids (needed forJson.Net)
        /// </summary>
        [JsonProperty("b")]
        internal IEnumerable<MexcV3OrderBookEntry> BidsStream { set => Bids = value; }

        /// <summary>
        /// Setter for asks (needed forJson.Net)
        /// </summary>
        [JsonProperty("a")]
        internal IEnumerable<MexcV3OrderBookEntry> AsksStream { set => Asks = value; }
    }
}
