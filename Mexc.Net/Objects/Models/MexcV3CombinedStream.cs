using Newtonsoft.Json;

namespace Mexc.Net.Objects.Models
{
    /// <summary>
    /// Represents the mexc result for combined data on a single socket connection
    /// See on https://github.com/mxcdevelop/APIDoc
    /// Combined streams
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MexcV3CombinedStream<T>
    {
        /// <summary>
        /// The stream combined
        /// </summary>
        [JsonProperty("c")]
        public string Stream { get; set; } = string.Empty;

        /// <summary>
        /// 交易代码显示名称(即将废弃）
        /// </summary>
        [JsonProperty("sd")]
        public string SymbolDisplay { get; set; } = string.Empty;

        /// <summary>
        /// 交易代码
        /// </summary>
        [JsonProperty("s")]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// The data of stream
        /// </summary>
        [JsonProperty("d")]
        public T Data { get; set; } = default!;
    }
}
