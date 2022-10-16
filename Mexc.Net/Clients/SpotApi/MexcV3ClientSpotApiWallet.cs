using System.Collections.Generic;
using System.Globalization;
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
    public class MexcV3ClientSpotApiWallet : IMexcV3ClientSpotApiWallet
    {
        private const string userCoinsEndpoint = "capital/config/getall";

        private const string api = "api";
        private const string publicVersion = "3";
        private const string signedVersion = "3";

        private readonly Log _log;

        private readonly MexcV3ClientSpotApi _baseClient;

        internal MexcV3ClientSpotApiWallet(Log log, MexcV3ClientSpotApi baseClient)
        {
            _log = log;
            _baseClient = baseClient;
        }

        #region 1.Query the currency information
        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<MexcV3UserAsset>>> GetUserAssetsAsync(int? receiveWindow = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.Options.ReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            var response = await _baseClient.MexcV3SendRequestInternal<IEnumerable<MexcV3UserAsset>>(
                uri: _baseClient.GetUrl(userCoinsEndpoint, "api", "3"), 
                method: HttpMethod.Get, 
                cancellationToken: ct, 
                parameters: parameters, 
                signed: true, 
                weight: 10).ConfigureAwait(false);
            return response;
        }
        #endregion
    }
}
