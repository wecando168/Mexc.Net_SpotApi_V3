namespace Mexc.Net.Interfaces
{
    /// <summary>
    /// Stream Trade data
    /// </summary>
    public interface IMexcV3StreamPrivateDeals
    {
        /// <summary>
        /// 交易代码
        /// </summary>
        string Symbol { get; set; }

        /// <summary>
        /// 交易代码显示名称(即将废弃）
        /// </summary>
        string SymbolDisplay { get; set; }

        /// <summary>
        /// The data
        /// </summary>
        IMexcV3StreamPrivateDealsData Data { get; set; }
    }
}
