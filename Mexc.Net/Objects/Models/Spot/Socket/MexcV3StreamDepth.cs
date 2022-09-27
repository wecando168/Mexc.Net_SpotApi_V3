using Mexc.Net.Interfaces;
using Newtonsoft.Json;
using Mexc.Net.Converters;

namespace Mexc.Net.Objects.Models.Spot.Socket
{
    /// <summary>
    /// 增量深度信息(实时)
    /// Diff.Depth Stream
    /// If the quantity is 0, it means that the order of the price has been cancel or traded，remove the price level.
    /// 如果某个价格对应的挂单量(v)为0，表示该价位的挂单已经撤单或者被吃，应该移除这个价位。
    /// </summary>
    public class MexcV3StreamDepth : IMexcV3Stream<MexcV3StreamDepthData>
    {
        /// <summary>
        /// The Subscription request
        /// 订阅频道（可作为当前请求客户端标识）
        /// </summary>
        [JsonProperty("c")]
        public string Stream { get; set; } = string.Empty;

        /// <summary>
        /// 数据
        /// </summary>
        [JsonProperty("d")]
        public MexcV3StreamDepthData Data { get; set; } = default!;

        /// <summary>
        /// 数据流交易代码Symbol
        /// </summary>
        [JsonProperty("s")]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// The time the event happened
        /// 事件时间eventTime
        /// </summary>
        [JsonProperty("t")]
        public string? EventTimeStamp { get; set; }
    }
}

/*
{
    "c":"spot@public.increase.depth.v3.api@BTCUSDT",
    "d":
    {
        "E":1663550873343,
        "bids":
        [
            {
                "p":"19443.30",
                "v":"0.000000"
            }
        ],
        "e":"spot@public.increase.depth.v3.api",
        "s":"BTCUSDT",
        "version":"4179466072"
    },
    "s":"BTCUSDT",
    "t":1663550873343
}

{
    "c":"spot@public.increase.depth.v3.api@BTCUSDT",
    "d":
    {
        "E":1663550873390,
        "asks":
        [
            {
                "p":"19465.05",
                "v":"0.002637"
            }
        ],
        "e":"spot@public.increase.depth.v3.api",
        "s":"BTCUSDT",
        "version":"4179466077"
    },
    "s":"BTCUSDT",
    "t":1663550873390
}
*/