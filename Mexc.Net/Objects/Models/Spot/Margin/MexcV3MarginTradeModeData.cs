using Newtonsoft.Json;

namespace Mexc.Net.Objects.Models.Spot.Margin
{
    /// <summary>
    /// Stream Trade data
    /// </summary>
    public class MexcV3MarginTradeModeData
    {
        /// <summary>
        /// The trade mode
        /// </summary>
        [JsonProperty("tradeMode")]
        public int TradeMode { get; set; }

        /// <summary>
        /// The symbol the data is for
        /// </summary>
        [JsonProperty("symbol")]
        public string Symbol { get; set; } = string.Empty;
    }
}
