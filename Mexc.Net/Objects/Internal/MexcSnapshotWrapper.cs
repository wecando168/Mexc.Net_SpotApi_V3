using Newtonsoft.Json;

namespace Mexc.Net.Objects.Internal
{
    internal class MexcSnapshotWrapper<T>
    {
        public int Code { get; set; }
        [JsonProperty("msg")] 
        public string Message { get; set; } = string.Empty;
        [JsonProperty("snapshotVos")]
        public T SnapshotData { get; set; } = default!;
    }
}
