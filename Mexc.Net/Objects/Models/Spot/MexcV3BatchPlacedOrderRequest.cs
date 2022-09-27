using System.Collections.Generic;
using Mexc.Net.Converters;
using Mexc.Net.Enums;
using Newtonsoft.Json;

namespace Mexc.Net.Objects.Models.Spot
{
    /// <summary>
    /// 批量下单的提交请求
    /// </summary>
    public class MexcV3BatchPlacedOrderRequest
    {
        /// <summary>
        /// 批量下单的列表
        /// </summary>
        public IEnumerable<PlacedOrder>? placedOrderList { get; set; }
    }

    /// <summary>
    /// 下单的订单参数
    /// </summary>
    public class PlacedOrder
    {
        /// <summary>
        /// 交易代码
        /// </summary>
        [JsonProperty("symbol")]
        public string? Symbol { get; set; }

        /// <summary>
        /// The side of the order
        /// 订单方向
        /// </summary>
        [JsonProperty("side"), JsonConverter(typeof(OrderSideConverter))]
        public OrderSide? Side { get; set; }

        /// <summary>
        /// The type of the order
        /// 订单类型
        /// </summary>
        [JsonProperty("type"), JsonConverter(typeof(SpotOrderTypeConverter))]
        public SpotOrderType? Type { get; set; }

        /// <summary>
        /// 委托数量
        /// </summary>
        [JsonProperty("quantity")]
        public decimal? Quantity { get; set; }

        /// <summary>
        /// 委托总额
        /// </summary>
        [JsonProperty("quoteOrderQty")]
        public decimal? QuoteQuantity { get; set; }

        /// <summary>
        /// 委托价格
        /// </summary>
        [JsonProperty("price")]
        public decimal? Price { get; set; }

        /// <summary>
        /// 客户自定义的唯一订单ID
        /// </summary>
        [JsonProperty("newClientOrderId")]
        public string? ClientOrderId { get; set; }
    }
}