﻿using System;
using Mexc.Net.Converters;
using Mexc.Net.Enums;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;
using Mexc.Net.Interfaces;

namespace Mexc.Net.Objects.Models.Spot
{
    /// <summary>
    /// Order info
    /// </summary>
    public class MexcV3GetOrderResponse: IMexcV3GetOrderResponse
    {
        /// <summary>
        /// 错误提示消息
        /// </summary>
        [JsonProperty("msg")]
        public string? ErrorMessage { get; set; } = string.Empty;

        /// <summary>
        /// 错误编码
        /// </summary>
        [JsonProperty("code")]
        public long? ErrorCode { get; set; }

        /// <summary>
        /// 错误扩展信息
        /// </summary>
        [JsonProperty("_extend")]
        public string? Extend { get; set; }

        /// <summary>
        /// The symbol the order is for
        /// 交易代码
        /// </summary>
        [JsonProperty("symbol")]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// The order id generated by Mexc
        /// 订单编号
        /// </summary>
        [JsonProperty("orderId")]
        public string OrderId { get; set; } = string.Empty;

        /// <summary>
        /// The order List Id by Mexc
        /// OCO订单ID，否则为 -1
        /// </summary>
        [JsonProperty("orderListId")]
        public string OrderListId { get; set; } = string.Empty;

        /// <summary>
        /// The clientOrderId id generated by Mexc
        /// 订单的用户自定义唯一单号
        /// </summary>
        [JsonProperty("clientOrderId")]
        public string? ClientOrderId { get; set; }

        private decimal _price;

        /// <summary>
        /// The price of the order
        /// 下单价格
        /// </summary>
        [JsonProperty("price")]
        public decimal Price
        {
            get
            {
                if (_price == 0 && Type == SpotOrderType.Market && QuantityFilled != 0)
                    return QuoteQuantityFilled / QuantityFilled;
                return _price;
            }
            set => _price = value;
        }

        /// <summary>
        /// The original quantity of the order, as specified in the order parameters by the user
        /// 下单数量
        /// </summary>
        [JsonProperty("origQty")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// The currently executed quantity of the order
        /// 已成交数量
        /// </summary>
        [JsonProperty("executedQty")]
        public decimal QuantityFilled { get; set; }

        /// <summary>
        /// The currently executed amount of quote asset. Amounts to Sum(quantity * price) of executed trades for this order
        /// 未成交数量
        /// </summary>
        [JsonProperty("cummulativeQuoteQty")]
        public decimal QuoteQuantityFilled { get; set; }

        /// <summary>
        /// The status of the order
        /// 订单状态
        /// </summary>
        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore), JsonConverter(typeof(OrderStatusConverter))]
        public OrderStatus? Status { get; set; } = default!;

        /// <summary>
        /// 订单的时效方式
        /// </summary>
        [JsonProperty("timeInForce"), JsonConverter(typeof(TimeInForceConverter))]
        public TimeInForce? TimeInForce { get; set; }

        /// <summary>
        /// The type of the order
        /// 订单类型
        /// </summary>
        [JsonProperty("type"), JsonConverter(typeof(SpotOrderTypeConverter))]
        public SpotOrderType Type { get; set; }

        /// <summary>
        /// The side of the order
        /// 订单方向
        /// </summary>
        [JsonProperty("side"), JsonConverter(typeof(OrderSideConverter))]
        public OrderSide Side { get; set; }

        /// <summary>
        /// stop price
        /// 止损价格
        /// </summary>
        [JsonProperty("stopPrice")]
        public string? StopPrice { get; set; } = string.Empty;

        /// <summary>
        /// stop price
        /// 冰山数量
        /// </summary>
        [JsonProperty("icebergQty")]
        public string? IcebergQty { get; set; }

        /// <summary>
        /// Order created time
        /// 订单创建时间
        /// </summary>
        [JsonProperty("time"), JsonConverter(typeof(DateTimeConverter))]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// Order update time
        /// 订单最后更新时间
        /// </summary>
        [JsonProperty("updateTime"), JsonConverter(typeof(DateTimeConverter))]
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// is order book
        /// 是否在orderbook中
        /// </summary>
        [JsonProperty("isWorking")]
        public bool IsWorking { get; set; }

        /// <summary>
        /// is order book
        /// 原始的交易金额
        /// </summary>
        [JsonProperty("origQuoteOrderQty")]
        public decimal? QuoteQuantity { get; set; }
    }
}

