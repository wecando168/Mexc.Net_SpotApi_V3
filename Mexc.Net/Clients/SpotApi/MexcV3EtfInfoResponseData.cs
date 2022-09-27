using System;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Mexc.Net.Interfaces
{
    /// <summary>
    /// Etf Info
    /// </summary>
    public class MexcV3EtfInfoResponseData : IMexcV3EtfInfoResponseData
    {
        /// <summary>
        /// ETF symbol
        /// 杠杆ETF交易对
        /// </summary>
        [JsonProperty("symbol")]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// Net Value
        /// 最新净值
        /// </summary>
        [JsonProperty("netValue")]
        public string? NetValue { get; set; }

        /// <summary>
        /// Fund Fee
        /// 管理费率
        /// </summary>
        [JsonProperty("feeRate")]
        public string? FeeRate { get; set; }

        /// <summary>
        /// The symbol the order is for
        /// 系统时间
        /// </summary>
        [JsonProperty("timestamp"),JsonConverter(typeof(DateTimeConverter))]
        public DateTime Timestamp { get; set; }
    }
}
