using Newtonsoft.Json;
using Mexc.Net.Interfaces;
using System.Collections.Generic;
using CryptoExchange.Net.Converters;
using System;

namespace Mexc.Net.Objects.Models.Spot
{
    /// <summary>
    /// Etf Info
    /// </summary>
    public class MexcV3EtfInfoResponse
    {
        /// <summary>
        /// ETF symbol
        /// 杠杆ETF交易对
        /// </summary>
        [JsonProperty("symbol", NullValueHandling = NullValueHandling.Ignore)]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// Net Value
        /// 最新净值
        /// </summary>
        [JsonProperty("netValue", NullValueHandling = NullValueHandling.Ignore)]
        public string? NetValue { get; set; }

        /// <summary>
        /// Fund Fee
        /// 管理费率
        /// </summary>
        [JsonProperty("feeRate", NullValueHandling = NullValueHandling.Ignore)]
        public string? FeeRate { get; set; }

        /// <summary>
        /// The symbol the order is for
        /// 系统时间
        /// </summary>
        [JsonProperty("timestamp", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// 目标杠杆
        /// </summary>
        [JsonProperty("Leverage", NullValueHandling = NullValueHandling.Ignore)]
        public int? Leverage { get; set; }

        /// <summary>
        /// 当前杠杆
        /// </summary>
        [JsonProperty("realLeverage", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? RealLeverage { get; set; }

        /// <summary>
        /// 合并次数
        /// </summary>
        [JsonProperty("mergedTimes", NullValueHandling = NullValueHandling.Ignore)]
        public int? MergedTimes { get; set; }

        /// <summary>
        /// 最近合并时间
        /// </summary>
        [JsonProperty("lastMergedTime", NullValueHandling = NullValueHandling.Ignore)]
        public long? LastMergedTime { get; set; }

        /// <summary>
        /// 再平衡前篮子
        /// </summary>
        [JsonProperty("PreBasket", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? preBasket { get; set; }

        /// <summary>
        /// 再平衡前杠杆
        /// </summary>
        [JsonProperty("preLeverage", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? PreLeverage { get; set; }        

        /// <summary>
        /// 再平衡后篮子
        /// </summary>
        [JsonProperty("basket", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? Basket { get; set; }
    }
}
