using Mexc.Net.Objects;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Objects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CryptoExchange.Net;
using CryptoExchange.Net.Converters;
using Mexc.Net.Interfaces.Clients.GeneralApi;
using Mexc.Net.Clients.SpotApi;
using CryptoExchange.Net.Logging;
using Microsoft.Extensions.Logging;

namespace Mexc.Net.Clients.GeneralApi
{
    /// <inheritdoc cref="IMexcV3ClientGeneralApi" />
    public class MexcV3ClientGeneralApi : RestApiClient, IMexcV3ClientGeneralApi
    {
        #region fields 
        private readonly MexcV3RestClient _baseClient;
        internal new readonly MexcV3ClientOptions Options;
        private readonly Log _log;
        #endregion

        #region Api clients
        /// <inheritdoc />
        public IMexcV3ClientGeneralApiSubAccount SubAccount { get; }
        #endregion

        #region constructor/destructor

        internal MexcV3ClientGeneralApi(Log log, MexcV3RestClient baseClient, MexcV3ClientOptions options) : base(log, options, options.SpotApiOptions)
        {
            Options = options;
            _baseClient = baseClient;
            _log = log;

            SubAccount = new MexcV3ClientGeneralApiSubAccount(this);

            requestBodyEmptyContent = "";
            requestBodyFormat = RequestBodyFormat.FormData;
            arraySerialization = ArrayParametersSerialization.MultipleValues;
        }

        #endregion

        /// <inheritdoc />
        protected override AuthenticationProvider CreateAuthenticationProvider(ApiCredentials credentials)
            => new MexcV3AuthenticationProvider(credentials);

        internal Uri GetUrl(string endpoint, string api, string? version = null)
        {
            var result = BaseAddress.AppendPath(api);

            if (!string.IsNullOrEmpty(version))
                result = result.AppendPath($"v{version}");

            return new Uri(result.AppendPath(endpoint));
        }

        internal async Task<WebCallResult<T>> MexcV3SendRequest<T>(Uri uri, HttpMethod method, CancellationToken cancellationToken,
            Dictionary<string, object>? parameters = null, bool signed = false, HttpMethodParameterPosition? postPosition = null,
            ArrayParametersSerialization? arraySerialization = null, int weight = 1, bool ignoreRateLimit = false) where T : class
        {
            var result = await MexcV3SendRequestAsync<T>(
                uri: uri,
                method: method,
                cancellationToken: cancellationToken,
                parameters: parameters,
                signed: signed,
                parameterPosition: postPosition,
                arraySerialization: arraySerialization,
                requestWeight: weight,
                deserializer: null,
                additionalHeaders: null,
                ignoreRatelimit: ignoreRateLimit
                ).ConfigureAwait(false);
            if (!result && result.Error!.Code == -1021 && Options.SpotApiOptions.AutoTimestamp)
            {
                _log.Write(LogLevel.Debug, "Received Invalid Timestamp error, triggering new time sync");
                MexcV3ClientSpotApi.TimeSyncState.LastSyncTime = DateTime.MinValue;
            }
            return result;
        }



        /// <inheritdoc />
        protected override Task<WebCallResult<DateTime>> GetServerTimestampAsync()
            => _baseClient.SpotApi.MarketData.GetServerTimeAsync();

        /// <inheritdoc />
        public override TimeSyncInfo GetTimeSyncInfo()
            => new TimeSyncInfo(_log, Options.SpotApiOptions.AutoTimestamp, Options.SpotApiOptions.TimestampRecalculationInterval, MexcV3ClientSpotApi.TimeSyncState);

        /// <inheritdoc />
        public override TimeSpan GetTimeOffset()
            => MexcV3ClientSpotApi.TimeSyncState.TimeOffset;
    }
}
