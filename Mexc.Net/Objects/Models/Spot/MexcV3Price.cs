﻿using System;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Mexc.Net.Objects.Models.Spot
{
    /// <summary>
    /// The price of a symbol
    /// </summary>
    public class MexcV3Price
    {
        /// <summary>
        /// The symbol the price is for
        /// </summary>
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// The price of the symbol
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// Timestamp
        /// </summary>
        [JsonProperty("time"), JsonConverter(typeof(DateTimeConverter))]
        public DateTime? Timestamp { get; set; }
    }

    /// <summary>
    /// Futures-Coin price
    /// </summary>
    public class MexcV3FuturesCoinPrice: MexcV3Price
    {
        /// <summary>
        /// Name of the pair
        /// </summary>
        [JsonProperty("ps")]
        public string Pair { get; set; } = string.Empty;
    }
}
