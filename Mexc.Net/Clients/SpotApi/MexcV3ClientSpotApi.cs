using Mexc.Net.Converters;
using Mexc.Net.Enums;
using Mexc.Net.Objects;
using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Objects;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Mexc.Net.Objects.Internal;
using Mexc.Net.Objects.Models.Spot;
using CryptoExchange.Net.Logging;
using Mexc.Net.Interfaces.Clients.SpotApi;
using CryptoExchange.Net.CommonObjects;
using CryptoExchange.Net.Interfaces.CommonClients;
using Mexc.Net.Objects.Models.Spot.Margin;

namespace Mexc.Net.Clients.SpotApi
{
    /// <inheritdoc cref="IMexcV3ClientSpotApi" />
    public class MexcV3ClientSpotApi : RestApiClient, IMexcV3ClientSpotApi, ISpotClient
    {
        #region fields 
        private readonly MexcV3RestClient _baseClient;
        internal new readonly MexcV3ClientOptions Options;

        internal MexcV3ExchangeInfo? ExchangeInfo;
        internal DateTime? LastExchangeInfoUpdate;

        internal static TimeSyncState TimeSyncState = new TimeSyncState("Spot Api");

        private readonly Log _log;
        #endregion

        #region Api clients
        /// <inheritdoc />
        public IMexcV3ClientSpotApiMarketData MarketData { get; }

        /// <inheritdoc />
        public IMexcV3ClientSpotApiSubAccount SubAccount { get; }

        /// <inheritdoc />
        public IMexcV3ClientSpotApiSpotAccountTrade SpotAccountTrade { get; }

        /// <inheritdoc />
        public IMexcV3ClientSpotApiWallet Wallet { get; }

        /// <inheritdoc />
        public IMexcV3ClientSpotApiETF ExchangeTradedFunds { get; }

        /// <inheritdoc />
        public IMexcV3ClientSpotApiMarginAccountTrade MarginAccountTrade { get; }

        /// <inheritdoc />
        public IMexcV3ClientSpotApiWebsocketAccount WebsocketAccount { get; }

        /// <inheritdoc />
        public IMexcV3ClientSpotApiTrading Trading { get; }

        /// <inheritdoc />
        public string ExchangeName => "Mexc";
        #endregion

        /// <summary>
        /// Event triggered when an order is placed via this client. Only available for Spot orders
        /// </summary>
        public event Action<OrderId>? OnOrderPlaced;
        /// <summary>
        /// Event triggered when an order is canceled via this client. Note that this does not trigger when using CancelAllOrdersAsync. Only available for Spot orders
        /// </summary>
        public event Action<OrderId>? OnOrderCanceled;

        #region constructor/destructor
        internal MexcV3ClientSpotApi(Log log, MexcV3RestClient baseClient, MexcV3ClientOptions options) : base(options, options.SpotApiOptions)
        {
            Options = options;
            _log = log;
            _baseClient = baseClient;
            
            MarketData = new MexcV3ClientSpotApiMarketData(log, this);
            SubAccount = new MexcV3ClientSpotApiSubAccount(log, this);
            SpotAccountTrade = new MexcV3ClientSpotApiSpotAccountTrade(log, this);
            Wallet = new MexcV3ClientSpotApiWallet(log, this);
            ExchangeTradedFunds = new MexcV3ClientSpotApiETF(log, this);
            MarginAccountTrade = new MexcV3ClientSpotApiMarginAccountTrade(log, this);
            WebsocketAccount = new MexcV3ClientSpotApiWebsocketAccount(log, this);

            requestBodyEmptyContent = "";
            requestBodyFormat = RequestBodyFormat.FormData;
            arraySerialization = ArrayParametersSerialization.MultipleValues;
        }
        #endregion

        /// <inheritdoc />
        protected override AuthenticationProvider CreateAuthenticationProvider(ApiCredentials credentials)
            => new MexcV3AuthenticationProvider(credentials);

        #region helpers
        internal async Task<WebCallResult<MexcV3PlacedTestOrderResponse>> PlaceTestOrderInternal(
            Uri uri,
            string symbol,
            Enums.OrderSide side,
            SpotOrderType type,
            decimal? quantity = null,
            decimal? quoteQuantity = null,
            decimal? price = null,
            string? newClientOrderId = null,
            int? receiveWindow = null,
            int weight = 1,
            CancellationToken ct = default)
        {
            symbol.ValidateMexcSymbol();

            if (quoteQuantity != null && type != SpotOrderType.Market)
                throw new ArgumentException("quoteQuantity is only valid for market orders");

            if (quantity == null && quoteQuantity == null || quantity != null && quoteQuantity != null)
                throw new ArgumentException("1 of either should be specified, quantity or quoteOrderQuantity");

            var rulesCheck = await CheckTestTradeRules(symbol, quantity, quoteQuantity, price, type, ct).ConfigureAwait(false);
            if (!rulesCheck.Passed)
            {
                _log.Write(LogLevel.Warning, rulesCheck.ErrorMessage!);
                return new WebCallResult<MexcV3PlacedTestOrderResponse>(new ArgumentError(rulesCheck.ErrorMessage!));
            }

            quantity = rulesCheck.Quantity;
            price = rulesCheck.Price;
            quoteQuantity = rulesCheck.QuoteQuantity;

            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol },
                { "side", JsonConvert.SerializeObject(side, new OrderSideConverter(false)) },
                { "type", JsonConvert.SerializeObject(type, new SpotOrderTypeConverter(false)) }
            };
            parameters.AddOptionalParameter("quantity", quantity?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("quoteOrderQty", quoteQuantity?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("price", price?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("newClientOrderId", newClientOrderId);            
            parameters.AddOptionalParameter("receiveWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? Options.ReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await MexcV3SendRequestInternal<MexcV3PlacedTestOrderResponse>(uri, HttpMethod.Post, ct, parameters, true, HttpMethodParameterPosition.InUri, null, weight: weight).ConfigureAwait(false);
        }

        internal async Task<WebCallResult<MexcV3PlacedOrderResponse>> PlaceOrderInternal(
            Uri uri,
            string symbol,
            OrderSide side,
            SpotOrderType type,
            decimal? quantity = null,
            decimal? quoteQuantity = null,
            decimal? price = null,
            string? newClientOrderId = null,
            int? receiveWindow = null,
            int weight = 1,
            CancellationToken ct = default)
        {
            symbol.ValidateMexcSymbol();

            if (quoteQuantity != null && type != SpotOrderType.Market)
                throw new ArgumentException("quoteQuantity is only valid for market orders");

            if (quantity == null && quoteQuantity == null || quantity != null && quoteQuantity != null)
                throw new ArgumentException("1 of either should be specified, quantity or quoteOrderQuantity");

            var rulesCheck = await CheckTradeRules(symbol, quantity, quoteQuantity, price, type, ct).ConfigureAwait(false);
            if (!rulesCheck.Passed)
            {
                _log.Write(LogLevel.Warning, rulesCheck.ErrorMessage!);
                return new WebCallResult<MexcV3PlacedOrderResponse>(new ArgumentError(rulesCheck.ErrorMessage!));
            }

            quantity = rulesCheck.Quantity;
            price = rulesCheck.Price;
            quoteQuantity = rulesCheck.QuoteQuantity;

            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol },
                { "side", JsonConvert.SerializeObject(side, new OrderSideConverter(false)) },
                { "type", JsonConvert.SerializeObject(type, new SpotOrderTypeConverter(false)) }
            };
            parameters.AddOptionalParameter("quantity", quantity?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("quoteOrderQty", quoteQuantity?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("newClientOrderId", newClientOrderId);
            parameters.AddOptionalParameter("price", price?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("receiveWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? Options.ReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await MexcV3SendRequestInternal<MexcV3PlacedOrderResponse>(uri, HttpMethod.Post, ct, parameters, true, HttpMethodParameterPosition.InUri, weight: weight).ConfigureAwait(false);
        }

        internal async Task<WebCallResult<MexcV3PlacedOrderResponse>> BatchPlaceOrderInternal(
            Uri uri, 
            MexcV3BatchPlacedOrderRequest mexcV3BatchPlacedOrderRequest,
            int? receiveWindow = null,
            int weight = 1,
            CancellationToken ct = default
            )
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            List<PlacedOrder> placedOrderList = new List<PlacedOrder>();
            foreach (PlacedOrder? item in mexcV3BatchPlacedOrderRequest.placedOrderList)
            {
                item.Symbol.ValidateMexcSymbol();
                if (item.QuoteQuantity != null && item.Type != SpotOrderType.Market)
                    throw new ArgumentException("quoteQuantity is only valid for market orders");
                if (item.Quantity == null && item.QuoteQuantity == null || item.Quantity != null && item.QuoteQuantity != null)
                    throw new ArgumentException("1 of either should be specified, quantity or quoteOrderQuantity");

                var rulesCheck = await CheckTradeRules(item.Symbol, item.Quantity, item.QuoteQuantity, item.Price, item.Type, CancellationToken.None).ConfigureAwait(false);
                if (!rulesCheck.Passed)
                {
                    _log.Write(LogLevel.Warning, rulesCheck.ErrorMessage!);
                    return new WebCallResult<MexcV3PlacedOrderResponse>(new ArgumentError(rulesCheck.ErrorMessage!));
                }
                item.Quantity = rulesCheck.Quantity;
                item.Price = rulesCheck.Price;
                item.QuoteQuantity = rulesCheck.QuoteQuantity;

                Dictionary<string, object>? parameter = new Dictionary<string, object>
                {
                    { "symbol", item.Symbol },
                    { "side", JsonConvert.SerializeObject(item.Side, new OrderSideConverter(false)) },
                    { "type", JsonConvert.SerializeObject(item.Type, new SpotOrderTypeConverter(false)) }
                };
                parameter.AddOptionalParameter("quantity", item.Quantity?.ToString(CultureInfo.InvariantCulture));
                parameter.AddOptionalParameter("quoteOrderQty", item.QuoteQuantity?.ToString(CultureInfo.InvariantCulture));
                parameter.AddOptionalParameter("newClientOrderId", item.ClientOrderId);
                parameter.AddOptionalParameter("price", item.Price?.ToString(CultureInfo.InvariantCulture));
                parameter.AddOptionalParameter("receiveWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? Options.ReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));
                placedOrderList.Add((PlacedOrder)item);
                //placedOrderList.Append((PlacedOrder)item);
                //placedOrderList.Append(parameter);                
            }
            parameters.AddOptionalParameter("batchOrders", placedOrderList);
            return await MexcV3SendRequestInternal<MexcV3PlacedOrderResponse>(uri, HttpMethod.Post, ct, parameters, true, HttpMethodParameterPosition.InUri,ArrayParametersSerialization.Array, weight: weight).ConfigureAwait(false);
        }

        internal async Task<WebCallResult<MexcV3PlacedOrderResponse>> BatchPlaceOrderTestInternal(
            Uri uri,
            string mexcV3BatchPlacedOrderRequestTestList,
            int? receiveWindow = null,
            int weight = 1,
            CancellationToken ct = default
            )
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();            
            parameters.AddOptionalParameter("batchOrders", mexcV3BatchPlacedOrderRequestTestList);
            var response = await MexcV3SendRequestInternal<MexcV3PlacedOrderResponse>(
                uri: uri, 
                method: HttpMethod.Post, 
                cancellationToken: ct, 
                parameters: parameters, 
                signed: true, 
                postPosition: HttpMethodParameterPosition.InUri, 
                arraySerialization: ArrayParametersSerialization.MultipleValues, 
                weight: weight).ConfigureAwait(false);
            return response;
        }

        internal async Task<WebCallResult<MexcV3CancelOrderResponse>> CancelOrderInternal(
            Uri uri,
            string symbol,
            string? orderId = null,
            string? origClientOrderId = null,
            string? newClientOrderId = null,
            int? receiveWindow = null,
            int weight = 1,
            CancellationToken ct = default
            )
        {
            symbol.ValidateMexcSymbol();
            if (string.IsNullOrEmpty(orderId) && string.IsNullOrEmpty(origClientOrderId))
                throw new ArgumentException("Either orderId or origClientOrderId must be sent");

            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol }
            };
            parameters.AddOptionalParameter("orderId", orderId?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("origClientOrderId", origClientOrderId);
            parameters.AddOptionalParameter("newClientOrderId", newClientOrderId);
            parameters.AddOptionalParameter("receiveWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? Options.ReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await MexcV3SendRequestInternal<MexcV3CancelOrderResponse>(uri, HttpMethod.Delete, ct, parameters, true, HttpMethodParameterPosition.InUri, null, weight: weight).ConfigureAwait(false);
        }

        internal async Task<WebCallResult<MexcV3GetOrderResponse>> GetOrderInternal(
            Uri uri,
            string symbol,
            string? orderId = null,
            string? origClientOrderId = null,
            int? receiveWindow = null,
            int weight = 1,
            CancellationToken ct = default
            )
        {
            symbol.ValidateMexcSymbol();
            if (string.IsNullOrEmpty(orderId) && string.IsNullOrEmpty(origClientOrderId))
                throw new ArgumentException("Either orderId or origClientOrderId must be sent");

            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol }
            };
            parameters.AddOptionalParameter("orderId", orderId?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("origClientOrderId", origClientOrderId);
            parameters.AddOptionalParameter("receiveWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? Options.ReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await MexcV3SendRequestInternal<MexcV3GetOrderResponse>(uri, HttpMethod.Delete, ct, parameters, true, HttpMethodParameterPosition.InUri, null, weight: weight).ConfigureAwait(false);
        }

        internal async Task<WebCallResult<MexcV3MarginTradeModeResponse>> MarginTradeModeInternal(
            Uri uri,
            string symbol,
            int? tradeMode = null,
            int? receiveWindow = null,
            int weight = 1,
            CancellationToken ct = default
            )
        {
            symbol.ValidateMexcSymbol();

            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol }
            };
            parameters.AddOptionalParameter("tradeMode", tradeMode);
            parameters.AddOptionalParameter("receiveWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? Options.ReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await MexcV3SendRequestInternal<MexcV3MarginTradeModeResponse>(uri, HttpMethod.Delete, ct, parameters, true, HttpMethodParameterPosition.InUri, null, weight: weight).ConfigureAwait(false);
        }

        internal async Task<WebCallResult<MexcV3MarginPlacedOrderResponse>> MarginPlacedOrderInternal(
            Uri uri,
            string symbol,
            bool isIsolated,
            OrderSide side,
            SpotOrderType spotOrderType,
            decimal ? quantity,
            decimal? quoteOrderQty,
            decimal? price,
            string? newClientOrderId,
            int? receiveWindow = null,
            int weight = 1,
            CancellationToken ct = default
            )
        {
            symbol.ValidateMexcSymbol();

            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol }
            };
            parameters.AddOptionalParameter("isIsolated", isIsolated);
            parameters.AddOptionalParameter("side", side);
            parameters.AddOptionalParameter("type", spotOrderType);
            parameters.AddOptionalParameter("quantity", quantity);
            parameters.AddOptionalParameter("quoteOrderQty", quoteOrderQty);
            parameters.AddOptionalParameter("price", price);
            parameters.AddOptionalParameter("newClientOrderId", newClientOrderId);
            parameters.AddOptionalParameter("receiveWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? Options.ReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await MexcV3SendRequestInternal<MexcV3MarginPlacedOrderResponse>(uri, HttpMethod.Delete, ct, parameters, true, HttpMethodParameterPosition.InUri, null, weight: weight).ConfigureAwait(false);
        }

        /// <summary>
        /// 接口Uri地址生成
        /// </summary>
        /// <param name="endpoint">接口</param>
        /// <param name="api">路径</param>
        /// <param name="version">版本</param>
        /// <returns></returns>
        internal Uri GetUrl(string endpoint, string api, string? version = null)
        {
            var result = BaseAddress.AppendPath(api);

            if (!string.IsNullOrEmpty(version))
                result = result.AppendPath($"v{version}");

            return new Uri(result.AppendPath(endpoint));
        }

        internal async Task<MexcV3TradeRuleResult> CheckTestTradeRules(string symbol, decimal? quantity, decimal? quoteQuantity, decimal? price, SpotOrderType? type, CancellationToken ct)
        {
            var outputQuantity = quantity;
            var outputQuoteQuantity = quoteQuantity;
            var outputPrice = price;

            if (Options.SpotApiOptions.TradeRulesBehaviour == TradeRulesBehaviour.None)
                return MexcV3TradeRuleResult.CreateTestPassed(outputQuantity, outputQuoteQuantity, outputPrice);

            if (ExchangeInfo == null || LastExchangeInfoUpdate == null || (DateTime.UtcNow - LastExchangeInfoUpdate.Value).TotalMinutes > Options.SpotApiOptions.TradeRulesUpdateInterval.TotalMinutes)
                await MarketData.GetExchangeInfoAsync(ct).ConfigureAwait(false);

            if (ExchangeInfo == null)
                return MexcV3TradeRuleResult.CreateFailed("Unable to retrieve trading rules, validation failed");

            var symbolData = ExchangeInfo.Symbols.SingleOrDefault(s => string.Equals(s.SymbolName, symbol, StringComparison.CurrentCultureIgnoreCase));
            if (symbolData == null)
                return MexcV3TradeRuleResult.CreateFailed($"Trade rules check failed: Symbol {symbol} not found");

            if (type != null)
            {
                if (!symbolData.OrderTypes.Contains(type.Value))
                    return MexcV3TradeRuleResult.CreateFailed(
                        $"Trade rules check failed: {type} order type not allowed for {symbol}");
            }

            var currentQuantity = outputQuantity ?? quantity.Value;
            var notional = currentQuantity * outputPrice.Value;
            
            return MexcV3TradeRuleResult.CreateTestPassed(outputQuantity, outputQuoteQuantity, outputPrice);
        }

        internal async Task<MexcV3TradeRuleResult> CheckTradeRules(string symbol, decimal? quantity, decimal? quoteQuantity, decimal? price, SpotOrderType? type, CancellationToken ct)
        {
            var outputQuantity = quantity;
            var outputQuoteQuantity = quoteQuantity;
            var outputPrice = price;

            if (Options.SpotApiOptions.TradeRulesBehaviour == TradeRulesBehaviour.None)
                return MexcV3TradeRuleResult.CreatePassed(outputQuantity, outputQuoteQuantity, outputPrice);

            if (!object.Equals(ct, null))
            {
                if (ExchangeInfo == null || LastExchangeInfoUpdate == null || (DateTime.UtcNow - LastExchangeInfoUpdate.Value).TotalMinutes > Options.SpotApiOptions.TradeRulesUpdateInterval.TotalMinutes)
                    await MarketData.GetExchangeInfoAsync((CancellationToken)ct).ConfigureAwait(false);
            }

            if (ExchangeInfo == null || LastExchangeInfoUpdate == null || (DateTime.UtcNow - LastExchangeInfoUpdate.Value).TotalMinutes > Options.SpotApiOptions.TradeRulesUpdateInterval.TotalMinutes)
                await MarketData.GetExchangeInfoAsync(ct).ConfigureAwait(false);

            if (ExchangeInfo == null)
                return MexcV3TradeRuleResult.CreateFailed("Unable to retrieve trading rules, validation failed");

            var symbolData = ExchangeInfo.Symbols.SingleOrDefault(s => string.Equals(s.SymbolName, symbol, StringComparison.CurrentCultureIgnoreCase));
            if (symbolData == null)
                return MexcV3TradeRuleResult.CreateFailed($"Trade rules check failed: Symbol {symbol} not found");

            if (type != null)
            {
                if (!symbolData.OrderTypes.Contains(type.Value))
                    return MexcV3TradeRuleResult.CreateFailed(
                        $"Trade rules check failed: {type} order type not allowed for {symbol}");
            }

            if (price == null)
                return MexcV3TradeRuleResult.CreatePassed(outputQuantity, outputQuoteQuantity, null);            

            var currentQuantity = outputQuantity ?? quantity.Value;
            var notional = currentQuantity * outputPrice.Value;
            
            return MexcV3TradeRuleResult.CreatePassed(outputQuantity, outputQuoteQuantity, outputPrice);
        }

        /// <summary>
        /// 内部发送请求(这个发送是不对参数进行字典排序的，抹茶交易系统的V3版API必须按照指定顺序，不能自己排序，一定要用这个！！！）
        /// </summary>
        /// <typeparam name="T">调用请求需要返回的数据类型（自定义的结构体或者标准变量类型</typeparam>
        /// <param name="uri">Url 地址</param>
        /// <param name="method">请求方式：get post 等</param>
        /// <param name="cancellationToken">取消令牌：是否是取消状态</param>
        /// <param name="parameters">参数</param>
        /// <param name="signed">是否需要签名</param>
        /// <param name="postPosition">Http方法参数位置</param>
        /// <param name="arraySerialization">数组参数序列化</param>
        /// <param name="weight">权重</param>
        /// <param name="ignoreRateLimit">是否忽略速率限制</param>
        /// <returns></returns>
        internal async Task<WebCallResult<T>> MexcV3SendRequestInternal<T>(Uri uri, HttpMethod method, CancellationToken cancellationToken,
            Dictionary<string, object>? parameters = null, bool signed = false, HttpMethodParameterPosition? postPosition = null,
            ArrayParametersSerialization? arraySerialization = null, int weight = 1, bool ignoreRateLimit = false) where T : class
        {
            WebCallResult<T> result = await _baseClient.MexcV3SendRequestInternal<T>(this, uri, method, cancellationToken, parameters, signed, postPosition, arraySerialization, weight, ignoreRateLimit: ignoreRateLimit).ConfigureAwait(false);
            if (!result && result.Error!.Code == 700003 && Options.SpotApiOptions.AutoTimestamp)
            {
                _log.Write(LogLevel.Debug, "Received Invalid Timestamp error, triggering new time sync");
                TimeSyncState.LastSyncTime = DateTime.MinValue;
            }
            return result;
        }

        /// <summary>
        /// 内部发送请求(这个发送是对参数进行默认字典排序的，抹茶交易系统的V3版API如果排序了，会报错，不要用！！！）
        /// </summary>
        /// <typeparam name="T">调用请求需要返回的数据类型（自定义的结构体或者标准变量类型</typeparam>
        /// <param name="uri">Url 地址</param>
        /// <param name="method">请求方式：get post 等</param>
        /// <param name="cancellationToken">取消令牌：是否是取消状态</param>
        /// <param name="parameters">参数</param>
        /// <param name="signed">是否需要签名</param>
        /// <param name="postPosition">Http方法参数位置</param>
        /// <param name="arraySerialization">数组参数序列化</param>
        /// <param name="weight">权重</param>
        /// <param name="ignoreRateLimit">是否忽略速率限制</param>
        /// <returns></returns>
        internal async Task<WebCallResult<T>> SendRequestInternal<T>(Uri uri, HttpMethod method, CancellationToken cancellationToken,
            Dictionary<string, object>? parameters = null, bool signed = false, HttpMethodParameterPosition? postPosition = null,
            ArrayParametersSerialization? arraySerialization = null, int weight = 1, bool ignoreRateLimit = false) where T : class
        {
            var result = await _baseClient.SendRequestInternal<T>(this, uri, method, cancellationToken, parameters, signed, postPosition, arraySerialization, weight, ignoreRateLimit: ignoreRateLimit).ConfigureAwait(false);
            if (!result && result.Error!.Code == 700003 && Options.SpotApiOptions.AutoTimestamp)
            {
                _log.Write(LogLevel.Debug, "Received Invalid Timestamp error, triggering new time sync");
                TimeSyncState.LastSyncTime = DateTime.MinValue;
            }
            return result;
        }

        #endregion

        /// <inheritdoc />
        protected override Task<WebCallResult<DateTime>> GetServerTimestampAsync()
            => MarketData.GetServerTimeAsync();

        /// <inheritdoc />
        public override TimeSyncInfo GetTimeSyncInfo()
            => new TimeSyncInfo(_log, Options.SpotApiOptions.AutoTimestamp, Options.SpotApiOptions.TimestampRecalculationInterval, TimeSyncState);

        /// <inheritdoc />
        public override TimeSpan GetTimeOffset()
            => TimeSyncState.TimeOffset;

        /// <inheritdoc />
        public ISpotClient CommonSpotClient => this;

        /// <inheritdoc />
        public string GetSymbolName(string baseAsset, string quoteAsset) =>
            (baseAsset + quoteAsset).ToUpper(CultureInfo.InvariantCulture);

        internal void InvokeOrderPlaced(OrderId id)
        {
            OnOrderPlaced?.Invoke(id);
        }

        internal void InvokeOrderCanceled(OrderId id)
        {
            OnOrderCanceled?.Invoke(id);
        }

        async Task<WebCallResult<OrderId>> ISpotClient.PlaceOrderAsync(string symbol, CommonOrderSide side, CommonOrderType type, decimal quantity, decimal? price, string? accountId, string? clientOrderId, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException(nameof(symbol) + " required for Mexc " + nameof(ISpotClient.PlaceOrderAsync), nameof(symbol));

            var order = await Trading.PlaceOrderAsync(symbol, GetOrderSide(side), GetOrderType(type), quantity, null, price: price, newClientOrderId: clientOrderId, ct: ct).ConfigureAwait(false);
            if (!order)
                return order.As<OrderId>(null);

            return order.As(new OrderId
            {
                SourceObject = order,
                Id = order.Data.OrderId.ToString(CultureInfo.InvariantCulture)
            });
        }

        async Task<WebCallResult<Order>> IBaseRestClient.GetOrderAsync(string orderId, string? symbol, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(orderId))
                throw new ArgumentException("Order id invalid", nameof(orderId));

            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException(nameof(symbol) + " required for Mexc " + nameof(ISpotClient.GetOrderAsync), nameof(symbol));

            var order = await Trading.GetOrderAsync(symbol!, orderId, ct: ct).ConfigureAwait(false);
            if (!order)
                return order.As<Order>(null);

            return order.As(new Order
            {
                SourceObject = order,
                Id = order.Data.OrderId.ToString(CultureInfo.InvariantCulture),
                Symbol = order.Data.Symbol,
                Price = order.Data.Price,
                Quantity = order.Data.Quantity,
                QuantityFilled = order.Data.QuantityFilled,
                Side = order.Data.Side == Enums.OrderSide.Buy ? CommonOrderSide.Buy : CommonOrderSide.Sell,
                Type = GetOrderType(order.Data.Type),
                Status = GetOrderStatus((OrderStatus)order.Data.Status),
                Timestamp = order.Data.CreateTime
            });
        }

        async Task<WebCallResult<IEnumerable<UserTrade>>> IBaseRestClient.GetOrderTradesAsync(string orderId, string? symbol, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(orderId))
                throw new ArgumentException("Order id invalid", nameof(orderId));

            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException(nameof(symbol) + " required for Mexc " + nameof(ISpotClient.GetOrderTradesAsync), nameof(symbol));

            var trades = await Trading.GetUserTradesAsync(symbol!, orderId, ct: ct).ConfigureAwait(false);
            if (!trades)
                return trades.As<IEnumerable<UserTrade>>(null);

            return trades.As(trades.Data.Select(t =>
                new UserTrade
                {
                    SourceObject = t,
                    Id = t.TradeId.ToString(CultureInfo.InvariantCulture),
                    OrderId = t.OrderId.ToString(CultureInfo.InvariantCulture),
                    Symbol = t.Symbol,
                    Price = t.Price,
                    Quantity = t.Quantity,
                    Fee = t.Fee,
                    FeeAsset = t.FeeAsset,
                    Timestamp = t.Timestamp
                }));
        }

        async Task<WebCallResult<IEnumerable<Order>>> IBaseRestClient.GetOpenOrdersAsync(string? symbol, CancellationToken ct)
        {
            var orderInfo = await Trading.GetOpenOrdersAsync(symbol, ct: ct).ConfigureAwait(false);
            if (!orderInfo)
                return orderInfo.As<IEnumerable<Order>>(null);

            return orderInfo.As(orderInfo.Data.Select(s =>
                new Order
                {
                    SourceObject = s,
                    Id = s.OrderId.ToString(CultureInfo.InvariantCulture),
                    Symbol = s.Symbol,
                    Side = s.Side == Enums.OrderSide.Buy ? CommonOrderSide.Buy : CommonOrderSide.Sell,
                    Price = s.Price,
                    Quantity = s.Quantity,
                    QuantityFilled = s.QuantityFilled,
                    Type = GetOrderType(s.Type),
                    Status = GetOrderStatus((OrderStatus)s.Status),
                    Timestamp = s.CreateTime
                }));
        }

        async Task<WebCallResult<IEnumerable<Order>>> IBaseRestClient.GetClosedOrdersAsync(string? symbol, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException(nameof(symbol) + " required for Mexc " + nameof(ISpotClient.GetClosedOrdersAsync), nameof(symbol));

            var orderInfo = await Trading.GetOrdersAsync(symbol!, ct: ct).ConfigureAwait(false);
            if (!orderInfo)
                return orderInfo.As<IEnumerable<Order>>(null);

            return orderInfo.As(orderInfo.Data.Where(o => o.Status == Enums.OrderStatus.Canceled || o.Status == Enums.OrderStatus.Filled).Select(s =>
                new Order
                {
                    SourceObject = s,
                    Id = s.OrderId.ToString(CultureInfo.InvariantCulture),
                    Symbol = s.Symbol,
                    Price = s.Price,
                    Quantity = s.Quantity,
                    QuantityFilled = s.QuantityFilled,
                    Side = s.Side == Enums.OrderSide.Buy ? CommonOrderSide.Buy: CommonOrderSide.Sell,
                    Type = GetOrderType(s.Type),
                    Status = GetOrderStatus((OrderStatus)s.Status),
                    Timestamp = s.CreateTime
                }));
        }

        async Task<WebCallResult<OrderId>> IBaseRestClient.CancelOrderAsync(string orderId, string? symbol, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(orderId))
                throw new ArgumentException("Order id invalid", nameof(orderId));

            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException(nameof(symbol) + " required for Mexc " + nameof(ISpotClient.CancelOrderAsync), nameof(symbol));

            var order = await Trading.CancelOrderAsync(symbol!, orderId, ct: ct).ConfigureAwait(false);
            if (!order)
                return order.As<OrderId>(null);

            return order.As(new OrderId
            {
                SourceObject = order,
                Id = order.Data.OrderId.ToString(CultureInfo.InvariantCulture)
            });
        }

        async Task<WebCallResult<IEnumerable<Symbol>>> IBaseRestClient.GetSymbolsAsync(CancellationToken ct)
        {
            var exchangeInfo = await MarketData.GetExchangeInfoAsync(ct: ct).ConfigureAwait(false);
            if (!exchangeInfo)
                return exchangeInfo.As<IEnumerable<Symbol>>(null);

            return exchangeInfo.As(exchangeInfo.Data.Symbols.Select(s =>
                new Symbol
                {
                    SourceObject = s,
                    Name = s.SymbolName
                }));
        }

        async Task<WebCallResult<Ticker>> IBaseRestClient.GetTickerAsync(string symbol, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException(nameof(symbol) + " required for Mexc " + nameof(ISpotClient.GetTickerAsync), nameof(symbol));

            var ticker = await MarketData.GetTickerAsync(symbol, ct: ct).ConfigureAwait(false);
            if (!ticker)
                return ticker.As<Ticker>(null);

            return ticker.As(new Ticker
            {
                SourceObject = ticker.Data,
                Symbol = ticker.Data.Symbol,
                HighPrice = ticker.Data.HighPrice,
                LowPrice = ticker.Data.LowPrice,
                Price24H = ticker.Data.PrevDayClosePrice,
                LastPrice = ticker.Data.LastPrice,
                Volume = ticker.Data.Volume
            });
        }

        async Task<WebCallResult<IEnumerable<Ticker>>> IBaseRestClient.GetTickersAsync(CancellationToken ct)
        {
            var tickers = await MarketData.GetTickersAsync(ct: ct).ConfigureAwait(false);
            if (!tickers)
                return tickers.As<IEnumerable<Ticker>>(null);

            return tickers.As(tickers.Data.Select(t => new Ticker
            {
                SourceObject = t,
                Symbol = t.Symbol,
                HighPrice = t.HighPrice,
                LowPrice = t.LowPrice,
                Price24H = t.PrevDayClosePrice,
                LastPrice = t.LastPrice,
                Volume = t.Volume
            }));
        }

        async Task<WebCallResult<IEnumerable<Kline>>> IBaseRestClient.GetKlinesAsync(string symbol, TimeSpan timespan, DateTime? startTime, DateTime? endTime, int? limit, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException(nameof(symbol) + " required for Mexc " + nameof(ISpotClient.GetKlinesAsync), nameof(symbol));

            var klines = await MarketData.GetKlinesAsync(symbol, GetKlineIntervalFromTimespan(timespan), startTime, endTime, limit, ct: ct).ConfigureAwait(false);
            if (!klines)
                return klines.As<IEnumerable<Kline>>(null);

            return klines.As(klines.Data.Select(t => new Kline
            {
                SourceObject = t,
                HighPrice = t.HighPrice,
                LowPrice = t.LowPrice,
                OpenTime = t.OpenTime,
                ClosePrice = t.ClosePrice,
                OpenPrice = t.OpenPrice,
                Volume  = t.Volume
            })); 
        }

        async Task<WebCallResult<OrderBook>> IBaseRestClient.GetOrderBookAsync(string symbol, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException(nameof(symbol) + " required for Mexc " + nameof(ISpotClient.GetOrderBookAsync), nameof(symbol));

            var orderbook = await MarketData.GetOrderBookAsync(symbol, ct: ct).ConfigureAwait(false);
            if (!orderbook)
                return orderbook.As<OrderBook>(null);

            return orderbook.As(new OrderBook
            {
                SourceObject = orderbook.Data,
                Asks = orderbook.Data.Asks.Select(a => new OrderBookEntry { Price = a.Price, Quantity = a.Quantity }),
                Bids = orderbook.Data.Bids.Select(b => new OrderBookEntry { Price = b.Price, Quantity = b.Quantity })
            });
        }

        async Task<WebCallResult<IEnumerable<Trade>>> IBaseRestClient.GetRecentTradesAsync(string symbol, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException(nameof(symbol) + " required for Mexc " + nameof(ISpotClient.GetRecentTradesAsync), nameof(symbol));

            var trades = await MarketData.GetRecentTradesAsync(symbol, ct: ct).ConfigureAwait(false);
            if (!trades)
                return trades.As<IEnumerable<Trade>>(null);

            return trades.As(trades.Data.Select(t => new Trade
            {
                SourceObject = t,
                Symbol = symbol,
                Price = t.Price,
                Quantity = t.BaseQuantity,
                Timestamp = t.TradeTime
            }));
        }

        async Task<WebCallResult<IEnumerable<Balance>>> IBaseRestClient.GetBalancesAsync(string? accountId, CancellationToken ct)
        {
            var balances = await SpotAccountTrade.GetAccountInfoAsync(ct: ct).ConfigureAwait(false);
            if (!balances)
                return balances.As<IEnumerable<Balance>>(null);

            return balances.As(balances.Data.Balances.Select(t => new Balance
            {
                SourceObject = t,
                Asset = t.Asset,
                Available = t.Available,
                Total = t.Total
            }));
        }

        private static CommonOrderType GetOrderType(SpotOrderType orderType)
        {
            if (orderType == SpotOrderType.Limit)
                return CommonOrderType.Limit;
            if (orderType == SpotOrderType.Market)
                return CommonOrderType.Market;
            return CommonOrderType.Other;
        }

        private static CommonOrderStatus GetOrderStatus(Enums.OrderStatus orderStatus)
        {
            if (orderStatus == OrderStatus.New || orderStatus == Enums.OrderStatus.PartiallyFilled)
                return CommonOrderStatus.Active;
            if (orderStatus == OrderStatus.Filled)
                return CommonOrderStatus.Filled;
            return CommonOrderStatus.Canceled;
        }

        private static OrderSide GetOrderSide(CommonOrderSide side)
        {
            if (side == CommonOrderSide.Sell) return Enums.OrderSide.Sell;
            if (side == CommonOrderSide.Buy) return Enums.OrderSide.Buy;

            throw new ArgumentException("Unsupported order side for Mexc order: " + side);
        }

        private static SpotOrderType GetOrderType(CommonOrderType type)
        {
            if (type == CommonOrderType.Limit) return SpotOrderType.Limit;
            if (type == CommonOrderType.Market) return SpotOrderType.Market;

            throw new ArgumentException("Unsupported order type for Mexc order: " + type);
        }

        private static MexcV3RestKlineInterval GetKlineIntervalFromTimespan(TimeSpan timeSpan)
        {
            if (timeSpan == TimeSpan.FromMinutes(1)) return MexcV3RestKlineInterval.OneMinute;
            if (timeSpan == TimeSpan.FromMinutes(3)) return MexcV3RestKlineInterval.ThreeMinutes;
            if (timeSpan == TimeSpan.FromMinutes(5)) return MexcV3RestKlineInterval.FiveMinutes;
            if (timeSpan == TimeSpan.FromMinutes(15)) return MexcV3RestKlineInterval.FifteenMinutes;
            if (timeSpan == TimeSpan.FromMinutes(30)) return MexcV3RestKlineInterval.ThirtyMinutes;
            if (timeSpan == TimeSpan.FromHours(1)) return MexcV3RestKlineInterval.OneHour;
            if (timeSpan == TimeSpan.FromHours(2)) return MexcV3RestKlineInterval.TwoHour;
            if (timeSpan == TimeSpan.FromHours(4)) return MexcV3RestKlineInterval.FourHour;
            if (timeSpan == TimeSpan.FromHours(6)) return MexcV3RestKlineInterval.SixHour;
            if (timeSpan == TimeSpan.FromHours(8)) return MexcV3RestKlineInterval.EightHour;
            if (timeSpan == TimeSpan.FromHours(12)) return MexcV3RestKlineInterval.TwelveHour;
            if (timeSpan == TimeSpan.FromDays(1)) return MexcV3RestKlineInterval.OneDay;
            if (timeSpan == TimeSpan.FromDays(3)) return MexcV3RestKlineInterval.ThreeDay;
            if (timeSpan == TimeSpan.FromDays(7)) return MexcV3RestKlineInterval.OneWeek;
            if (timeSpan == TimeSpan.FromDays(30) || timeSpan == TimeSpan.FromDays(31)) return MexcV3RestKlineInterval.OneMonth;

            throw new ArgumentException("Unsupported timespan for Mexc Klines, check supported intervals using Mexc.Net.Enums.KlineInterval");
        }
    }
}
