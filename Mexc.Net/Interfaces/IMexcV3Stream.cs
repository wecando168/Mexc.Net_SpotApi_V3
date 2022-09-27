using Mexc.Net.Interfaces;
using Newtonsoft.Json;
using Mexc.Net.Converters;
using CryptoExchange.Net.Converters;
using System;

namespace Mexc.Net.Interfaces
{

    /// <summary>
    /// Websocket订阅返回数据接口
    /// </summary>
    public interface IMexcV3Stream<T>
    {
        /// <summary>
        /// The Subscription request
        /// 订阅频道（可作为当前请求客户端标识）
        /// </summary>
        string Stream { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        T Data { get; set; }

        /// <summary>
        /// 数据流交易代码Symbol
        /// </summary>
        string Symbol { get; set; }

        /// <summary>
        /// The time the event happened
        /// 事件时间eventTime
        /// </summary>
        string? EventTimeStamp { get; set; }
    }
}
