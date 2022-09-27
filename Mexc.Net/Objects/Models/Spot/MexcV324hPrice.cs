using Mexc.Net.Interfaces;
using Newtonsoft.Json;

namespace Mexc.Net.Objects.Models.Spot
{
    /// <summary>
    /// Price statistics of the last 24 hours
    /// </summary>
    public class MexcV324HPrice : MexcV324HPriceBase, IMexcV3Tick
    {
        /// <summary>
        /// The close price 24 hours ago
        /// 前一收盘价
        /// </summary>
        [JsonProperty("prevClosePrice")]
        public decimal PrevDayClosePrice { get; set; }

        /// <summary>
        /// The best bid price in the order book
        /// 买盘价格
        /// </summary>
        [JsonProperty("bidPrice")]
        public decimal BestBidPrice { get; set; }

        /// <summary>
        /// The quantity of the best bid price in the order book
        /// 买盘数量
        /// </summary>
        [JsonProperty("bidQty")]
        public decimal BestBidQuantity { get; set; }

        /// <summary>
        /// The best ask price in the order book
        /// 卖盘价格
        /// </summary>
        [JsonProperty("askPrice")]
        public decimal BestAskPrice { get; set; }

        /// <summary>
        /// The quantity of the best ask price in the order book
        /// 卖盘数量
        /// </summary>
        [JsonProperty("askQty")]
        public decimal BestAskQuantity { get; set; }

        /// <inheritdoc />
        [JsonProperty("volume")]
        public override decimal Volume { get; set; }

        /// <inheritdoc />
        [JsonProperty("quoteVolume")]
        public override decimal QuoteVolume { get; set; }
    }
}
