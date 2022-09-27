using CryptoExchange.Net.Converters;
using Newtonsoft.Json;
using System;

namespace Mexc.Net.Objects.Models.Spot.Margin
{
    /// <summary>
    /// Stream Trade data
    /// </summary>
    public class MexcV3MarginPlacedOrderData
    {
        /// <summary>
        /// The symbol the data is for
        /// 交易代码
        /// </summary>
        [JsonProperty("symbol")]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// order Id
        /// 订单id
        /// </summary>
        [JsonProperty("orderId")]
        public string OrderId { get; set; } = string.Empty;

        /// <summary>
        /// client Order Id
        /// 客户自定义订单id
        /// </summary>
        [JsonProperty("clientOrderId")]
        public string ClientOrderId { get; set; } = string.Empty;

        /// <summary>
        /// is Isolated
        /// 是否是逐仓
        /// </summary>
        [JsonProperty("isIsolated")]
        public bool IsIsolated { get; set; } = true;

        /// <summary>
        /// Transact Time
        /// 下单时间
        /// </summary>
        [JsonProperty("transactTime"), JsonConverter(typeof(DateTimeConverter))]
        public DateTime TransactTime { get; set; } = default!;
    }
}
