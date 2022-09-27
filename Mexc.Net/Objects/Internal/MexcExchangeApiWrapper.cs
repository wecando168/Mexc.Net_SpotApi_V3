namespace Mexc.Net.Objects.Internal
{
    internal class MexcExchangeApiWrapper<T>
    {
        public int Code { get; set; }
        public string Message { get; set; } = string.Empty;
        public string MessageDetail { get; set; } = string.Empty;

        public T Data { get; set; } = default!;

        public bool Success { get; set; }
    }
}
