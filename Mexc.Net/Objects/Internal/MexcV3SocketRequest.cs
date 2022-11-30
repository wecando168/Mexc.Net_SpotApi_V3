using System;
using Newtonsoft.Json;

namespace Mexc.Net.Objects.Internal
{
    internal class MexcV3Request
    {
        [JsonIgnore]
        public string? Id { get; set; }
    }

    internal class MexcV3SocketRequest: MexcV3Request
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

    internal class MexcV3AuthenticatedSubscribeRequest
    {
        [JsonProperty("action")]
        public string Action { get; set; }
        [JsonProperty("ch")]
        public string Channel { get; set; }

        public MexcV3AuthenticatedSubscribeRequest(string channel, string action = "sub")
        {
            Action = action;
            Channel = channel;
        }
    }

    internal class MexcV3SubscribeRequest : MexcV3Request
    {
        [JsonProperty("sub")]
        public string Topic { get; set; }
        [JsonProperty("id")]
        public new string Id { get; set; }

        public MexcV3SubscribeRequest(string id, string topic)
        {
            Id = id;
            Topic = topic;
        }
    }

    internal class MexcV3IncrementalOrderBookSubscribeRequest : MexcV3SubscribeRequest
    {
        [JsonProperty("data_type")]
        public string DataType { get; set; }
        public MexcV3IncrementalOrderBookSubscribeRequest(string id, string topic, string dataType) : base(id, topic)
        {
            DataType = dataType;
        }
    }
}
