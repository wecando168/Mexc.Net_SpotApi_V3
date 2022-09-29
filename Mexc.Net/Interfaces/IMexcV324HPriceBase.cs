using System;

namespace Mexc.Net.Interfaces
{
    /// <summary>
    /// 24 hour price stats
    /// </summary>
    public interface IMexcV324HPrice: IMexcV3MiniTick
    {
        /// <summary>
        /// The actual price change in the last 24 hours
        /// 价格变化
        /// </summary>
        decimal PriceChange { get; set; }

        /// <summary>
        /// The price change in percentage in the last 24 hours
        /// 价格变化比
        /// </summary>
        decimal PriceChangePercent { get; set; }

        /// <summary>
        /// Time at which this 24 hours opened
        /// 开始时间
        /// </summary>
        DateTime OpenTime { get; set; }

        /// <summary>
        /// Time at which this 24 hours closed
        /// 结束时间
        /// </summary>
        DateTime CloseTime { get; set; }
    }
}