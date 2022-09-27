using System;
using Newtonsoft.Json;

namespace Mexc.Net.Objects.Internal
{
    internal class MexcSocketRequest
    {
        [JsonProperty("method")]
        public string Method { get; set; } = "";
        [JsonProperty("params")]
        public string[] Params { get; set; } = Array.Empty<string>();
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}
