﻿using Newtonsoft.Json;

namespace Mexc.Net.Objects.Models
{
    /// <summary>
    /// Query result
    /// </summary>
    public class MexcV3Result
    {
        /// <summary>
        /// Result code
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// Message
        /// </summary>
        [JsonProperty("msg")]
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// Query result
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class MexcV3Result<T>: MexcV3Result
    {
        /// <summary>
        /// The data
        /// </summary>
        public T Data { get; set; } = default!;
    }
}
