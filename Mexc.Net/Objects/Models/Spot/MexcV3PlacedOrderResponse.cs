using System;
using CryptoExchange.Net.Converters;
using Mexc.Net.Converters;
using Mexc.Net.Enums;
using Newtonsoft.Json;

namespace Mexc.Net.Objects.Models.Spot
{
    /// <summary>
    /// The result of placing a new order
    /// </summary>
    public class MexcV3PlacedOrderResponse
    {
        /// <summary>
        /// 错误返回的消息
        /// </summary>
        [JsonProperty("msg")]
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// 错误编码
        /// </summary>
        [JsonProperty("code")]
        public string? ErrorCode { get; set; }

        /// <summary>
        /// 错误扩展消息
        /// </summary>
        public _Extend? _extend { get; set; }

        /// <summary>
        /// symbol
        /// </summary>
        [JsonProperty("symbol")]
        public string? Symbol { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        [JsonProperty("orderId")]
        public string? OrderId { get; set; }

        /// <summary>
        /// 用户自定义单号（仅当用户提交订单有提供时返回）
        /// </summary>
        [JsonProperty("clientOrderId")]
        public string? ClientOrderId { get; set; }

        /// <summary>
        /// 新的用户自定义单号（仅当该提交状态为失败的时候才有这个参数返回）
        /// </summary>
        [JsonProperty("newClientOrderId")]
        public string? NewClientOrderId { get; set; }

        /// <summary>
        /// 客户端订单列表
        /// </summary>
        [JsonProperty("orderListId")]
        public int? OrderListId { get; set; }

        /// <summary>
        /// 下单价格
        /// </summary>
        [JsonProperty("price")]
        public string? Price { get; set; }

        /// <summary>
        /// The original quantity of the order, as specified in the order parameters by the user
        /// 原始下单数量
        /// </summary>
        [JsonProperty("origQty")]
        public decimal? Quantity { get; set; }

        /// <summary>
        /// The type of the order
        /// 订单类型
        /// </summary>
        [JsonProperty("type"), JsonConverter(typeof(SpotOrderTypeConverter))]
        public SpotOrderType? Type { get; set; }

        /// <summary>
        /// The side of the order
        /// 订单方向
        /// </summary>
        [JsonProperty("side"), JsonConverter(typeof(OrderSideConverter))]
        public OrderSide? Side { get; set; }

        /// <summary>
        /// The time the order was placed
        /// 下单时间
        /// </summary>
        [JsonProperty("transactTime"), JsonConverter(typeof(DateTimeConverter))]
        public DateTime? CreateTime { get; set; }
    }

    public class _Extend
    {
        /// <summary>
        /// 错误下单金额
        /// </summary>
        [JsonProperty("quantity")]
        public string? Quantity { get; set; }
    }
}


