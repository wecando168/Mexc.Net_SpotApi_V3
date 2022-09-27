using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Mexc.Net.Interfaces.Clients.SpotApi;
using Mexc.Net.Objects.Models.Spot;
using CryptoExchange.Net;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Logging;

namespace Mexc.Net.Clients.SpotApi
{
    /// <inheritdoc />
    public class MexcV3ClientSpotApiWebsocketAccount : IMexcV3ClientSpotApiWebsocketAccount
    {
        private const string getListenKeyEndpoint = "userDataStream";
        private const string keepListenKeyAliveEndpoint = "userDataStream";
        private const string closeListenKeyEndpoint = "userDataStream";

        private const string api = "api";
        private const string publicVersion = "3";
        private const string signedVersion = "3";

        private readonly Log _log;

        private readonly MexcV3ClientSpotApi _baseClient;

        internal MexcV3ClientSpotApiWebsocketAccount(Log log, MexcV3ClientSpotApi baseClient)
        {
            _log = log;
            _baseClient = baseClient;
        }


        #region Create a ListenKey
        /// <inheritdoc />
        public async Task<WebCallResult<string>> StartUserStreamAsync(CancellationToken ct = default)
        {
            WebCallResult<MexcV3ListenKey>? result = await _baseClient.MexcV3SendRequestInternal<MexcV3ListenKey>(
                uri: _baseClient.GetUrl(getListenKeyEndpoint, "api", "3"), 
                method: HttpMethod.Post, 
                cancellationToken: ct,
                signed: true
                ).ConfigureAwait(false);
            return result.As(result.Data?.ListenKey!);
        }

        #endregion

        #region Ping/Keep-alive a ListenKey

        /// <inheritdoc />
        public async Task<WebCallResult<string>> KeepAliveUserStreamAsync(string listenKey, CancellationToken ct = default)
        {
            listenKey.ValidateNotNull(nameof(listenKey));

            Dictionary<string, object>? parameters = new Dictionary<string, object>
            {
                { "listenKey", listenKey }
            };

            WebCallResult<MexcV3ListenKey>? result = await _baseClient.MexcV3SendRequestInternal<MexcV3ListenKey>(
                uri: _baseClient.GetUrl(keepListenKeyAliveEndpoint, "api", "3"),
                method: HttpMethod.Put,
                cancellationToken: ct,
                parameters: parameters,
                signed: true,
                postPosition: HttpMethodParameterPosition.InUri
                ).ConfigureAwait(false);
            return result.As(result.Data?.ListenKey!);
        }

        #endregion

        #region Invalidate a ListenKey
        /// <inheritdoc />
        public async Task<WebCallResult<string>> StopUserStreamAsync(string listenKey, CancellationToken ct = default)
        {
            listenKey.ValidateNotNull(nameof(listenKey));

            var parameters = new Dictionary<string, object>
            {
                { "listenKey", listenKey }
            };

            WebCallResult<MexcV3ListenKey>? result = await _baseClient.MexcV3SendRequestInternal<MexcV3ListenKey>(
                uri: _baseClient.GetUrl(closeListenKeyEndpoint, "api", "3"), 
                method: HttpMethod.Delete,
                cancellationToken: ct,
                parameters: parameters,
                signed: true,
                postPosition: HttpMethodParameterPosition.InUri
                ).ConfigureAwait(false);
            return result.As(result.Data?.ListenKey!);
        }

        #endregion
    }
}
