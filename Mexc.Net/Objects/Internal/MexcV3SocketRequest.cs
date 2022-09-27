using System;
using Newtonsoft.Json;

namespace Mexc.Net.Objects.Internal
{
    internal class MexcV3SocketRequest
    {
        /// <summary>
        /// 请求方法
        /// </summary>
        [JsonProperty("method")]
        public string Method { get; set; } = "";

        /// <summary>
        /// 请求参数
        /// </summary>
        [JsonProperty("params")]
        public string[] Params { get; set; } = Array.Empty<string>();
    }
}
