using Mexc.Net.Converters;
using Mexc.Net.Enums;
using Newtonsoft.Json;

namespace Mexc.Net.Objects.Models
{
    /// <summary>
    /// Order info
    /// </summary>
    public class MexcTestOrderBase
    {
        /// <summary>
        /// The symbol the order is for
        /// 交易对
        /// </summary>
        [JsonProperty("symbol")]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// The side of the order
        /// 订单方向
        /// </summary>
        [JsonProperty("side"),JsonConverter(typeof(OrderSideConverter))]
        public OrderSide Side { get; set; }

        /// <summary>
        /// The type of the order
        /// 订单类型
        /// </summary>
        [JsonProperty("type"), JsonConverter(typeof(SpotOrderTypeConverter))]
        public SpotOrderType Type { get; set; }

        /// <summary>
        /// Quantity
        /// 委托数量
        /// </summary>
        [JsonProperty("quantity")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// QuoteOrderQty
        /// 委托总额
        /// </summary>
        [JsonProperty("quoteOrderQty")]
        public decimal QuoteOrderQty { get; set; }

        /// <summary>
        /// 委托价格
        /// </summary>
        [JsonProperty("price")]
        private decimal Price;

        /// <summary>
        /// The order id as assigned by the client
        /// 客户自定义的唯一订单ID
        /// </summary>
        [JsonProperty("newClientOrderId")]
        public string ClientOrderId { get; set; } = string.Empty;


    }
}