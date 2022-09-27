using System.Collections.Generic;

namespace Mexc.Net.Interfaces
{
    /// <summary>
    /// Etf Info
    /// </summary>
    public interface IMexcV3EtfInfoResponse
    {
        /// <summary>
        /// 错误提示消息
        /// </summary>
        string? ErrorMessage { get; set; }

        /// <summary>
        /// 错误编码
        /// </summary>
        long? ErrorCode { get; set; }

        /// <summary>
        /// 错误扩展信息
        /// </summary>
        string? Extend { get; set; }

        /// <summary>
        /// The data
        /// </summary>
        IEnumerable<MexcV3EtfInfoResponseData> Data { get; set; }        
    }
}
