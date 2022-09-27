using System;

namespace Mexc.Net.Interfaces
{
    /// <summary>
    /// Etf Info
    /// </summary>
    public interface IMexcV3EtfInfoResponseData
    {
        /// <summary>
        /// ETF symbol
        /// 杠杆ETF交易对
        /// </summary>
        string Symbol { get; set; }

        /// <summary>
        /// Net Value
        /// 最新净值
        /// </summary>
        string? NetValue { get; set; }

        /// <summary>
        /// Fund Fee
        /// 管理费率
        /// </summary>
        string? FeeRate { get; set; }

        /// <summary>
        /// The symbol the order is for
        /// 系统时间
        /// </summary>
        DateTime Timestamp { get; set; }
    }
}
