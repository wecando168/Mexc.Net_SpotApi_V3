using System;
using Mexc.Net.Interfaces;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Mexc.Net.Objects.Models.Spot
{
    /// <summary>
    /// Information about the best price/quantity available for a symbol
    /// </summary>
    public class MexcV3BookPrice : IMexcV3BookPrice
    {
        /// <summary>
        /// The symbol the information is about
        /// 交易代码
        /// </summary>
        [JsonProperty("symbol")] 
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// The highest bid price for the symbol
        /// 最高买盘价
        /// </summary>
        [JsonProperty("bidPrice")]
        public decimal? BestBidPrice { get; set; }

        /// <summary>
        /// The quantity of the highest bid price currently in the order book
        /// 最高买盘数量
        /// </summary>
        [JsonProperty("bidQty")]
        public decimal? BestBidQuantity { get; set; }

        /// <summary>
        /// The lowest ask price for the symbol
        /// 最低卖盘价
        /// </summary>
        [JsonProperty("askPrice")]
        public decimal? BestAskPrice { get; set; }

        /// <summary>
        /// The quantity of the lowest ask price currently in the order book
        /// 最低卖盘数量
        /// </summary>
        [JsonProperty("askQty")]
        public decimal? BestAskQuantity { get; set; }

        /// <summary>
        /// Timestamp
        /// </summary>
        [JsonProperty("time"), JsonConverter(typeof(DateTimeConverter))]
        public DateTime? Timestamp { get; set; }
    }
}
