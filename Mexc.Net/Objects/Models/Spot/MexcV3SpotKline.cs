using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Mexc.Net.Objects.Models.Spot
{
    /// <summary>
    /// Candlestick information for symbol
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public class MexcV3SpotKline: MexcV3KlineBase 
    {
        /// <summary>
        /// The volume traded during this candlestick
        /// </summary>
        [ArrayProperty(5)]
        public override decimal Volume { get; set; }
        /// <summary>
        /// The volume traded during this candlestick in the asset form
        /// </summary>
        [ArrayProperty(7)]
        public override decimal QuoteVolume { get; set; }        
    }
}
