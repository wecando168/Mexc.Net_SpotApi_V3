using Newtonsoft.Json;
using Mexc.Net.Interfaces;
using System.Collections.Generic;

namespace Mexc.Net.Objects.Models.Spot
{
    /// <summary>
    /// Etf Info
    /// </summary>
    public class MexcV3EtfInfoResponse : IMexcV3EtfInfoResponse
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
        /// ETF Info Response Data
        /// 杠杆ETF交易对基本信息列表
        /// </summary>
        public IEnumerable<MexcV3EtfInfoResponseData>? Data { get; set; }
    }
}
