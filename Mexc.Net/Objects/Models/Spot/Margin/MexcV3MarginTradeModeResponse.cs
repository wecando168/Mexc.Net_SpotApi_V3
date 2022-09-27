using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mexc.Net.Objects.Models.Spot.Margin
{
    /// <summary>
    /// The result of margin trade mode
    /// </summary>
    public class MexcV3MarginTradeModeResponse
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
        /// trade Detail Array
        /// </summary>
        public IEnumerable<MexcV3MarginTradeModeData>? Data { get; set; }
    }
}


