using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Interfaces;
using Newtonsoft.Json;

namespace Mexc.Net.Objects.Models
{
    /// <summary>
    /// An entry in the order book
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public class MexcOrderBookEntry : ISymbolOrderBookEntry
    {
        /// <summary>
        /// The price of this order book entry
        /// 价格
        /// </summary>
        [ArrayProperty(0)]
        public decimal Price { get; set; }
        /// <summary>
        /// The quantity of this price in the order book
        /// 数量
        /// </summary>
        [ArrayProperty(1)]
        public decimal Quantity { get; set; }
    }
}
