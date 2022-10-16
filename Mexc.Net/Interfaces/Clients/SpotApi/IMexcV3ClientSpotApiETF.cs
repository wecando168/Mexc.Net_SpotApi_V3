using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mexc.Net.Objects.Models.Spot;
using CryptoExchange.Net.Objects;

namespace Mexc.Net.Interfaces.Clients.SpotApi
{
    /// <summary>
    /// Exchange Traded Funds Endpoints(ETF Endpoints)
    /// </summary>
    public interface IMexcV3ClientSpotApiETF
    {
        /// <summary>
        /// All Orders
        /// 查询所有订单 (USER_DATA)（指定单一交易代码）
        /// 状态； 有效，已取消或已完成，最多查询最近7天数据。
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_en/#all-orders" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_cn/#881e865963" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to get orders for</param>
        /// <param name="startTime">If set, only orders placed after this time will be returned</param>
        /// <param name="endTime">If set, only orders placed before this time will be returned</param>
        /// <param name="limit">Max number of results</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of orders</returns>
        Task<WebCallResult<IEnumerable<MexcV3GetOrderResponse>>> GetOrdersAsync(
            string symbol,
            DateTime? startTime = null,
            DateTime? endTime = null,
            int? limit = null, int?
            receiveWindow = null,
            CancellationToken ct = default);

        /// <summary>
        /// Get ETF info
        /// 获取杠杆ETF基础信息
        /// Get information on ETFs, such as symbol, netValue and fund fee.
        /// 获取ETF的基础信息，如可交易币对、最新净值和管理费率。
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_en/#get-etf-info" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_cn/#etf-2" /></para>
        /// </summary>
        /// <param name="symbol">ETF symbol</param>
        /// <param name="receiveWindow">Receive window</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>True if successful ping, false if no response</returns>
        Task<WebCallResult<MexcV3EtfInfoResponse>> GetETFInfoAsync( 
            string? symbol = null,
            int? receiveWindow = null,
            CancellationToken ct = default);
    }
}
