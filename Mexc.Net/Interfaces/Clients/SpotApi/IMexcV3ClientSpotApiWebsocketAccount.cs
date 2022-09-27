using System.Threading;
using System.Threading.Tasks;
using CryptoExchange.Net.Objects;

namespace Mexc.Net.Interfaces.Clients.SpotApi
{
    /// <summary>
    /// Mexc spot websocket account endpoints. Account endpoints include listen key info
    /// </summary>
    public interface IMexcV3ClientSpotApiWebsocketAccount
    {
        /// <summary>
        /// Start a new user data stream. The stream will close after 60 minutes unless a keepalive is sent. If the account has an active listenKey, that listenKey will be returned and its validity will be extended for 60 minutes.
        /// 生成现货 Listen Key (USER_STREAM)
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_en/#listen-key-spot" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_cn/#listen-key" /></para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Listen key</returns>
        Task<WebCallResult<string>> StartUserStreamAsync(CancellationToken ct = default);

        /// <summary>
        /// Sends a keep alive for the current user stream listen key to keep the stream from closing. Stream auto closes after 60 minutes if no keep alive is send. 30 minute interval for keep alive is recommended.
        /// 延长现货 Listen Key 有效期 (USER_STREAM)
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_en/#listen-key-spot" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_cn/#listen-key" /></para>
        /// </summary>
        /// <param name="listenKey">The listen key to keep alive</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<string>> KeepAliveUserStreamAsync(string listenKey, CancellationToken ct = default);

        /// <summary>
        /// Stops the current user stream
        /// 关闭现货 Listen Key (USER_STREAM)
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_en/#listen-key-spot" /></para>
        /// <para><a href="https://mxcdevelop.github.io/apidocs/spot_v3_cn/#listen-key" /></para>
        /// </summary>
        /// <param name="listenKey">The listen key to keep alive</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<string>> StopUserStreamAsync(string listenKey, CancellationToken ct = default);
    }
}
