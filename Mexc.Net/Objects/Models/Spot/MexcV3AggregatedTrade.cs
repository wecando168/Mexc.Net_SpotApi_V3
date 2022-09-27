using System;
using Mexc.Net.Interfaces;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Mexc.Net.Objects.Models.Spot
{
    /// <summary>
    /// Compressed aggregated trade information. Trades that fill at the time, from the same order, with the same price will have the quantity aggregated.
    /// </summary>
    public class MexcV3AggregatedTrade : IMexcV3AggregatedTrade
    {
        /// <summary>
        /// The id of this aggregation
        /// 归集成交ID
        /// </summary>
        [JsonProperty("a")]
        public string OrderId { get; set; }
        /// <summary>
        /// The price of trades in this aggregation
        /// 成交价
        /// </summary>
        [JsonProperty("p")]
        public decimal Price { get; set; }
        /// <summary>
        /// The total quantity of trades in the aggregation
        /// 成交量
        /// </summary>
        [JsonProperty("q")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// The first trade id in this aggregation
        /// 被归集的首个成交ID
        /// </summary>
        [JsonProperty("f")]
        public string FirstTradeId { get; set; }
        /// <summary>
        /// The last trade id in this aggregation
        /// 被归集的末个成交ID
        /// </summary>
        [JsonProperty("l")]
        public string LastTradeId { get; set; }
        /// <summary>
        /// The timestamp of the trades
        /// 成交时间
        /// </summary>
        [JsonProperty("T"), JsonConverter(typeof(DateTimeConverter))]
        public DateTime TradeTime { get; set; }
        /// <summary>
        /// Whether the buyer was the maker
        /// 是否为主动卖出单
        /// </summary>
        [JsonProperty("m")]
        public bool BuyerIsMaker { get; set; }
        /// <summary>
        /// Whether the trade was matched at the best price
        /// 是否为最优撮合单(可忽略，目前总为最优撮合)
        /// </summary>
        [JsonProperty("M")]
        public bool WasBestPriceMatch { get; set; }
    }
}
