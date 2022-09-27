using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mexc.Net.Objects.Models.Spot;
using CryptoExchange.Net.Objects;

namespace Mexc.Net.Interfaces.Clients.SpotApi
{
    /// <summary>
    /// Wallet Endpoints
    /// </summary>
    public interface IMexcV3ClientSpotApiWallet
    {
        /// <summary>
        /// Query the currency information Query currency details and the smart contract address
        /// 所有币种信息 返回币种详细信息以及智能合约地址
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_en/#query-the-currency-information" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_cn/#2a7110133d" /></para>
        /// </summary>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Assets info</returns>
        Task<WebCallResult<IEnumerable<MexcV3UserAsset>>> GetUserAssetsAsync(int? receiveWindow = null, CancellationToken ct = default);
    }
}
