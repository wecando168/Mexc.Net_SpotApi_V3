namespace Mexc.Net.Interfaces
{
    /// <summary>
    /// MiniTick info
    /// </summary>
    public interface IMexcV3MiniTick
    {
        /// <summary>
        /// Symbol
        /// 交易代码
        /// </summary>
        string Symbol { get; set; }

        /// <summary>
        /// Close Price
        /// 最新价（收盘价）
        /// </summary>
        decimal LastPrice { get; set; }

        /// <summary>
        /// Open Price
        /// 开盘价
        /// </summary>
        decimal OpenPrice { get; set; }

        /// <summary>
        /// High
        /// 最高价
        /// </summary>
        decimal HighPrice { get; set; }

        /// <summary>
        /// Low
        /// 最低价
        /// </summary>
        decimal LowPrice { get; set; }

        /// <summary>
        /// Total traded volume
        /// 成交量
        /// </summary>
        decimal Volume { get; set; }

        /// <summary>
        /// Total traded alternate asset volume
        /// 成交额
        /// </summary>
        decimal QuoteVolume { get; set; }
    }
}
