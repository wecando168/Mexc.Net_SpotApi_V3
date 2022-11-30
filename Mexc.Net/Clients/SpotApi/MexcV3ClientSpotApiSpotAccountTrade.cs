using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Mexc.Net.Enums;
using Mexc.Net.Interfaces.Clients.SpotApi;
using Mexc.Net.Objects.Models.Spot;
using CryptoExchange.Net;
using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.CommonObjects;
using Mexc.Net.Interfaces;
using CryptoExchange.Net.Logging;
using System.Linq;
using System.Web;

namespace Mexc.Net.Clients.SpotApi
{
    /// <inheritdoc />
    public class MexcV3ClientSpotApiSpotAccountTrade : IMexcV3ClientSpotApiSpotAccountTrade
    {
        private const string newTestOrderEndpoint = "order/test";
        private const string newOrderEndpoint = "order";
        private const string batchOrdersEndpoint = "batchOrders";
        private const string cancelOrderEndpoint = "order";
        private const string cancelAllOpenOrderEndpoint = "openOrders";
        private const string openOrdersEndpoint = "openOrders";
        private const string queryOrderEndpoint = "order";
        private const string allOrdersEndpoint = "allOrders";
        private const string accountInfoEndpoint = "account";
        private const string myTradesEndpoint = "myTrades";

        private const string api = "api";
        private const string publicVersion = "3";
        private const string signedVersion = "3";

        private readonly Log _log;

        private readonly MexcV3ClientSpotApi _baseClient;

        internal MexcV3ClientSpotApiSpotAccountTrade(Log log, MexcV3ClientSpotApi baseClient)
        {
            _log = log;
            _baseClient = baseClient;
        }

        #region 1.Test New Order

        /// <inheritdoc />
        public async Task<WebCallResult<MexcV3PlacedTestOrderResponse>> PlaceTestOrderAsync(string symbol,
            OrderSide side,
            SpotOrderType type,
            decimal? quantity = null,
            decimal? quoteQuantity = null,
            decimal? price = null,
            string? newClientOrderId = null,
            int? receiveWindow = null,
            CancellationToken ct = default)
        {
            WebCallResult<MexcV3PlacedTestOrderResponse>? respance = await _baseClient.PlaceTestOrderInternal(
                uri: _baseClient.GetUrl(newTestOrderEndpoint, api, signedVersion),
                symbol: symbol,
                side: side,
                type: type,
                quantity: quantity,
                quoteQuantity: quoteQuantity,
                price: price,
                newClientOrderId: newClientOrderId,
                receiveWindow: receiveWindow,
                weight: 1,
                ct: ct).ConfigureAwait(false);
            return respance;
        }
        
        #endregion

        #region 2.New Order 

        /// <inheritdoc />
        public async Task<WebCallResult<MexcV3PlacedOrderResponse>> PlaceOrderAsync(string symbol,
            OrderSide side,
            SpotOrderType type,
            decimal? quantity = null,
            decimal? quoteQuantity = null,
            decimal? price = null,
            string? newClientOrderId = null,
            int? receiveWindow = null,
            CancellationToken ct = default)
        {
            WebCallResult<MexcV3PlacedOrderResponse>? result = await _baseClient.PlaceOrderInternal(
                uri: _baseClient.GetUrl(newOrderEndpoint, api, signedVersion),
                symbol: symbol,
                side: side,
                type: type,
                quantity: quantity,
                quoteQuantity: quoteQuantity,
                price: price,
                newClientOrderId: newClientOrderId,
                receiveWindow: receiveWindow,
                weight: 1,
                ct: ct).ConfigureAwait(false);
            if (result)
                _baseClient.InvokeOrderPlaced(new OrderId() { SourceObject = result.Data, Id = result.Data.OrderId.ToString(CultureInfo.InvariantCulture) });
            return result;
        }

        #endregion

        #region 3.Batch Orders 
        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<MexcV3BatchPlacedOrderResponse>>> BatchPlaceOrderAsync(
            IEnumerable<MexcV3SubmitOrder> mexcV3BatchPlacedOrderRequestTestList,
            int? receiveWindow = null,
            CancellationToken ct = default)
        {
            WebCallResult<IEnumerable<MexcV3BatchPlacedOrderResponse>>? result = await _baseClient.BatchPlaceOrderInternal(
                uri: _baseClient.GetUrl(batchOrdersEndpoint, api, signedVersion),
                mexcV3BatchPlacedOrderRequestTestList,
                receiveWindow: receiveWindow,
                weight: 1,
                ct: ct).ConfigureAwait(false);
            //if (result)
                //_baseClient.InvokeOrderPlaced(new OrderId() { SourceObject = result.Data, Id = result.Data.OrderId.ToString(CultureInfo.InvariantCulture) });
            return result;
        }
        #endregion

        #region 4.Cancel Order

        /// <inheritdoc />
        public async Task<WebCallResult<MexcV3CancelOrderResponse>> CancelOrderAsync(
            string symbol,
            string? orderId = null,
            string? origClientOrderId = null,
            string? newClientOrderId = null,
            int? receiveWindow = null,
            CancellationToken ct = default)
        {
            WebCallResult<MexcV3CancelOrderResponse>? response = await _baseClient.CancelOrderInternal(
                uri: _baseClient.GetUrl(cancelOrderEndpoint, api, signedVersion),
                symbol: symbol,
                orderId: orderId,
                origClientOrderId: origClientOrderId,
                newClientOrderId: newClientOrderId,
                receiveWindow: receiveWindow,
                weight: 1,
                ct: ct).ConfigureAwait(false);
            if (response)
                _baseClient.InvokeOrderPlaced(new OrderId() { SourceObject = response.Data, Id = response.Data.OrderId.ToString(CultureInfo.InvariantCulture) });
            return response;
        }

        #endregion

        #region 5.Cancel all Open Orders on a Symbol

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<MexcV3CancelOrderResponse>>> CancelOpenOrdersAsync(
            string symbol,
            int? receiveWindow = null,
            CancellationToken ct = default)
        {
            symbol.ValidateMexcSymbol();

            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol }
            };
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.Options.ReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            WebCallResult<IEnumerable<MexcV3CancelOrderResponse>>? response = await _baseClient.MexcV3SendRequest<IEnumerable<MexcV3CancelOrderResponse>>(
                uri: _baseClient.GetUrl(cancelAllOpenOrderEndpoint, api, signedVersion),
                method: HttpMethod.Delete,
                cancellationToken: ct,
                parameters: parameters,
                signed: true,
                postPosition: HttpMethodParameterPosition.InUri,
                arraySerialization: null).ConfigureAwait(false);
            return response;
        }
        #endregion

        #region 6.Query Order

        /// <inheritdoc />
        public async Task<WebCallResult<MexcV3GetOrderResponse>> GetOrderAsync(string symbol, string? orderId = null, string? origClientOrderId = null, long? receiveWindow = null, CancellationToken ct = default)
        {
            symbol.ValidateMexcSymbol();
            if (orderId == null && origClientOrderId == null)
                throw new ArgumentException("Either orderId or origClientOrderId must be sent");

            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol }
            };
            parameters.AddOptionalParameter("orderId", orderId?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("origClientOrderId", origClientOrderId);
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.Options.ReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            WebCallResult<MexcV3GetOrderResponse>? response = await _baseClient.MexcV3SendRequest<MexcV3GetOrderResponse>(
                uri: _baseClient.GetUrl(queryOrderEndpoint, api, signedVersion),
                method: HttpMethod.Get,
                cancellationToken: ct,
                parameters: parameters,
                signed: true,
                postPosition: HttpMethodParameterPosition.InUri,
                weight: 2).ConfigureAwait(false);
            return response;
        }

        #endregion

        #region 7.Current Open Orders

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<IMexcV3GetOrderResponse>>> GetOpenOrdersAsync(
            IEnumerable<string> symbolList, 
            int? receiveWindow = null, 
            CancellationToken ct = default)
        {
            string symbolString = string.Empty;
            if (object.Equals(symbolList,null))
                throw new ArgumentException("Either symbol must be sent");
            if (!object.Equals(symbolList, null) && Enumerable.Count<string>(symbolList) > 5)
                throw new ArgumentException("The number of transaction symbols cannot be greater than 5");
            
            int i = Enumerable.Count<string>(symbolList);
            foreach (var item in symbolList)
            {
                i--;
                item?.ValidateMexcSymbol();
                symbolString += item;
                if (i != 0)
                    symbolString += ",";
            }

            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("symbol", symbolString);
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.Options.ReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));
            
            WebCallResult<IEnumerable<MexcV3GetOrderResponse>>? response = await _baseClient.MexcV3SendRequest<IEnumerable<MexcV3GetOrderResponse>>(
                uri: _baseClient.GetUrl(openOrdersEndpoint, api, signedVersion),
                method: HttpMethod.Get,
                cancellationToken: ct,
                parameters: parameters,
                signed: true,
                postPosition: HttpMethodParameterPosition.InUri,
                weight: Enumerable.Count<string>(symbolList) > 5 ? 40 : 3).ConfigureAwait(false);
            return response.As<IEnumerable<IMexcV3GetOrderResponse>>(response.Data);
        }

        #endregion

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

            WebCallResult<IEnumerable<MexcV3GetOrderResponse>>? response = await _baseClient.MexcV3SendRequest<IEnumerable<MexcV3GetOrderResponse>>(
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

        #region 9.Account Information

        /// <inheritdoc />
        public async Task<WebCallResult<MexcV3AccountInfo>> GetAccountInfoAsync(long? receiveWindow = null, CancellationToken ct = default)
        {
            Dictionary<string, object>? parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.Options.ReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            WebCallResult<MexcV3AccountInfo>? response = await _baseClient.MexcV3SendRequest<MexcV3AccountInfo>(
                uri: _baseClient.GetUrl(accountInfoEndpoint, "api", "3"),
                method: HttpMethod.Get,
                cancellationToken: ct,
                parameters: parameters,
                signed: true,
                weight: 10).ConfigureAwait(false);
            return response;
        }

        #endregion

        #region 10.Account Trade List

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<MexcV3Trade>>> GetUserTradesAsync(
            string symbol, 
            string? orderId = null, 
            DateTime? startTime = null, 
            DateTime? endTime = null, 
            int? limit = null, 
            long? fromId = null, 
            long? receiveWindow = null, CancellationToken ct = default)
        {
            symbol.ValidateMexcSymbol();
            limit?.ValidateIntBetween(nameof(limit), 1, 1000);

            Dictionary<string, object>? parameters = new Dictionary<string, object>
            {
                { "symbol", symbol }
            };
            parameters.AddOptionalParameter("orderId", orderId?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("fromId", fromId?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("startTime", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("endTime", DateTimeConverter.ConvertToMilliseconds(endTime));
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.Options.ReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            WebCallResult<IEnumerable<MexcV3Trade>>? response = await _baseClient.MexcV3SendRequest<IEnumerable<MexcV3Trade>>(
                uri: _baseClient.GetUrl(myTradesEndpoint, api, signedVersion),
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
