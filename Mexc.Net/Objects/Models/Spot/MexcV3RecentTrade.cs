using System;
using Mexc.Net.Interfaces;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Mexc.Net.Objects.Models.Spot
{
    /// <summary>
    /// Recent trade info
    /// </summary>
    public abstract class MexcV3RecentTrade : IMexcV3RecentTrade
    {
        /// <summary>
        /// The id of the trade
        /// </summary>
        [JsonProperty("id")]
        public string? OrderId { get; set; } = string.Empty;

        /// <summary>
        /// The price of the trade
        /// </summary>
        [JsonProperty("price")]
        public decimal Price { get; set; }

        /// <inheritdoc />
        [JsonProperty("qty")]
        public abstract decimal BaseQuantity { get; set; }

        /// <inheritdoc />
        [JsonProperty("quoteQty")]
        public abstract decimal QuoteQuantity { get; set; }

        /// <summary>
        /// The timestamp of the trade
        /// </summary>
        [JsonProperty("time"), JsonConverter(typeof(DateTimeConverter))]
        public DateTime TradeTime { get; set; }

        /// <summary>
        /// Whether the buyer is maker
        /// </summary>
        [JsonProperty("isBuyerMaker")]
        public bool BuyerIsMaker { get; set; }

        /// <summary>
        /// Whether the trade was made at the best match
        /// </summary>
        [JsonProperty("isBestMatch")]
        public bool IsBestMatch { get; set; }
    }

    /// <summary>
    /// Recent trade with quote quantity
    /// </summary>
    public class MexcV3RecentTradeQuote : MexcV3RecentTrade
    {
        /// <inheritdoc />
        [JsonProperty("quoteQty")]
        public override decimal QuoteQuantity { get; set; }

        /// <inheritdoc />
        [JsonProperty("qty")]
        public override decimal BaseQuantity { get; set; }
    }

    /// <summary>
    /// Recent trade with base quantity
    /// </summary>
    public class MexcV3RecentTradeBase : MexcV3RecentTrade
    {
        /// <inheritdoc />
        [JsonProperty("qty")]
        public override decimal QuoteQuantity { get; set; }

        /// <inheritdoc />
        [JsonProperty("baseQty")]
        public override decimal BaseQuantity { get; set; }
    }
}
