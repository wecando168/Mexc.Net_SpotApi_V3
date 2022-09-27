using System;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Mexc.Net.Objects.Models.Spot
{
    /// <summary>
    /// Information about a trade
    /// </summary>
    public class MexcV3Trade
    {
        /// <summary>
        /// The symbol the trade is for
        /// </summary>
        [JsonProperty("symbol")] 
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// The id of the trade
        /// 成交id
        /// </summary>
        [JsonProperty("id")]
        public string TradeId { get; set; }

        /// <summary>
        /// The order id the trade belongs to
        /// 订单编号
        /// </summary>
        [JsonProperty("orderId")]
        public string OrderId { get; set; }

        /// <summary>
        /// Id of the order list this order belongs to
        /// 订单id
        /// </summary>
        [JsonProperty("orderListId")]
        public long? OrderListId { get; set; }

        /// <summary>
        /// The price of the trade
        /// 	价格
        /// </summary>
        [JsonProperty("price")]
        public decimal Price { get; set; }

        /// <summary>
        /// The quantity of the trade
        /// 	数量
        /// </summary>
        [JsonProperty("qty")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// The quote quantity of the trade
        /// 成交金额
        /// </summary>
        [JsonProperty("quoteQty")]
        public decimal QuoteQuantity { get; set; }

        /// <summary>
        /// The fee paid for the trade
        /// 手续费
        /// </summary>
        [JsonProperty("commission")]
        public decimal Fee { get; set; }

        /// <summary>
        /// The asset the fee is paid in
        /// 手续费币种
        /// </summary>
        [JsonProperty("commissionAsset")]
        public string FeeAsset { get; set; } = string.Empty;

        /// <summary>
        /// The time the trade was made
        /// 成交时间
        /// </summary>
        [JsonProperty("time"), JsonConverter(typeof(DateTimeConverter))]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Whether account was the buyer in the trade
        /// 是否为买方
        /// </summary>
        [JsonProperty("isBuyer")]
        public bool IsBuyer { get; set; }

        /// <summary>
        /// Whether account was the maker in the trade
        /// 是否为maker单
        /// </summary>
        [JsonProperty("isMaker")]
        public bool IsMaker { get; set; }

        /// <summary>
        /// Whether trade was made with the best match
        /// 是否为最佳匹配
        /// </summary>
        [JsonProperty("isBestMatch")]
        public bool IsBestMatch { get; set; }

        /// <summary>
        /// Is Self Trade
        /// 是否自成交
        /// </summary>
        [JsonProperty("isSelfTrade")]
        public bool? IsSelfTrade { get; set; }

        /// <summary>
        /// Client OrderId
        /// 用户自定义id
        /// </summary>
        [JsonProperty("clientOrderId")]
        public string? ClientOrderId { get; set; }
    }
}
