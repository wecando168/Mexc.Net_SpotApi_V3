using Newtonsoft.Json;

namespace Mexc.Net.Objects.Models
{
    /// <summary>
    /// Represents the mexc result for combined data on a single socket connection
    /// See on https://github.com/mxcdevelop/APIDoc
    /// Combined streams
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MexcCombinedStream<T>
    {
        /// <summary>
        /// The stream combined
        /// </summary>
        [JsonProperty("stream")]
        public string Stream { get; set; } = string.Empty;

        /// <summary>
        /// The data of stream
        /// </summary>
        [JsonProperty("data")]
        public T Data { get; set; } = default!;
    }
}
