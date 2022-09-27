using System;
using Mexc.Net.Interfaces;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Mexc.Net.Objects.Models.Spot.Socket
{
    /// <summary>
    /// Aggregated information about trades for a symbol
    /// </summary>
    public class MexcV3StreamPersonalDeals : MexcV3StreamEvent, IMexcV3AggregatedTrade
    {
        /// <summary>
        /// The symbol the trade was for
        /// </summary>
        [JsonProperty("s")]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// The id of this aggregated trade
        /// </summary>
        [JsonProperty("a")]
        public string OrderId { get; set; } = string.Empty;

        /// <summary>
        /// The first trade id in this aggregation
        /// </summary>
        [JsonProperty("f")]
        public string FirstTradeId { get; set; } = string.Empty;

        /// <summary>
        /// The last trade id in this aggregation
        /// </summary>
        [JsonProperty("l")]
        public string LastTradeId { get; set; } = string.Empty;

        /// <summary>
        /// The price of the trades
        /// </summary>
        [JsonProperty("p")]
        public decimal Price { get; set; }

        /// <summary>
        /// The combined quantity of the trades
        /// </summary>
        [JsonProperty("q")]
        public decimal Quantity { get; set; }
        
        /// <summary>
        /// The time of the trades
        /// </summary>
        [JsonProperty("T"), JsonConverter(typeof(DateTimeConverter))]
        public DateTime TradeTime { get; set; }

        /// <summary>
        /// Whether the buyer was the maker
        /// </summary>
        [JsonProperty("m")]
        public bool BuyerIsMaker { get; set; }
        
        /// <summary>
        /// Unused
        /// </summary>
        [JsonProperty("M")]
        public bool WasBestPriceMatch { get; set; }
    }
}
