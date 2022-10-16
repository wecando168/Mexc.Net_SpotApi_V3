using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Mexc.Net.Interfaces.Clients.SpotApi;
using Mexc.Net.Objects.Models.Spot;
using CryptoExchange.Net;
using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Logging;
using CryptoExchange.Net.Objects;

namespace Mexc.Net.Clients.SpotApi
{
    /// <inheritdoc />
    public class MexcV3ClientSpotApiETF : IMexcV3ClientSpotApiETF
    {
        private const string allOrdersEndpoint = "allOrders";
        private const string etfInfoEndpoint = "etf/info";

        private const string api = "api";
        private const string publicVersion = "3";
        private const string signedVersion = "3";

        private readonly Log _log;

        private readonly MexcV3ClientSpotApi _baseClient;

        internal MexcV3ClientSpotApiETF(Log log, MexcV3ClientSpotApi baseClient)
        {
            _log = log;
            _baseClient = baseClient;
        }

        #region 8.All Orders 

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<MexcV3GetOrderResponse>>> GetOrdersAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, int? receiveWindow = null, CancellationToken ct = default)
        {
            symbol.ValidateMexcSymbol();
            limit?.ValidateIntBetween(nameof(limit), 1, 1000);

            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol }
            };
            parameters.AddOptionalParameter("startTime", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("endTime", DateTimeConverter.ConvertToMilliseconds(endTime));
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.Options.ReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            WebCallResult<IEnumerable<MexcV3GetOrderResponse>>? response = await _baseClient.MexcV3SendRequestInternal<IEnumerable<MexcV3GetOrderResponse>>(
                uri: _baseClient.GetUrl(allOrdersEndpoint, api, signedVersion),
                method: HttpMethod.Get,
                cancellationToken: ct,
                parameters: parameters,
                signed: true,
                postPosition: HttpMethodParameterPosition.InUri,
                weight: 10).ConfigureAwait(false);
            return response;
        }

        #endregion

        #region 1.Test Connectivity

        /// <inheritdoc />
        public async Task<WebCallResult<MexcV3EtfInfoResponse>> GetETFInfoAsync(
            string? symbol = null,
            int? receiveWindow = null,
            CancellationToken ct = default)
        {
            if(!string.IsNullOrWhiteSpace(symbol))
                symbol.ValidateMexcSymbol();

            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("symbol", symbol);
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.Options.ReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            WebCallResult<MexcV3EtfInfoResponse> ? response = await _baseClient.MexcV3SendRequestInternal<MexcV3EtfInfoResponse>(
                uri: _baseClient.GetUrl(etfInfoEndpoint, api, signedVersion),
                method: HttpMethod.Get,
                cancellationToken: ct,
                parameters: parameters,
                signed: true,
                postPosition: HttpMethodParameterPosition.InUri,
                weight: 10).ConfigureAwait(false);
            return response;
        }

        #endregion
    }
}
