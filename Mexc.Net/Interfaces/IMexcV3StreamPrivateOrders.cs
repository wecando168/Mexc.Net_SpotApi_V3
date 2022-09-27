namespace Mexc.Net.Interfaces
{
    /// <summary>
    /// Stream Order data
    /// </summary>
    public interface IMexcV3StreamPrivateOrders
    {
        /// <summary>
        /// 交易代码
        /// </summary>
        string Symbol { get; set; }

        /// <summary>
        /// The data
        /// </summary>
        IMexcV3StreamPrivateOrdersData Data { get; set; }
    }
}
