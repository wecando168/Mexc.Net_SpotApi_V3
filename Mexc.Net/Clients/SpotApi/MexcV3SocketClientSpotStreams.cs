using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mexc.Net.Converters;
using Mexc.Net.Enums;
using Mexc.Net.Interfaces;
using Mexc.Net.Interfaces.Clients.SpotApi;
using Mexc.Net.Objects;
using Mexc.Net.Objects.Models;
using Mexc.Net.Objects.Models.Spot.Socket;
using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Logging;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Mexc.Net.Objects.Internal;

namespace Mexc.Net.Clients.SpotApi
{
    /// <inheritdoc />
    public class MexcV3SocketClientSpotStreams : SocketApiClient, IMexcV3SocketClientSpotStreams
    {
        private new readonly Log _log = new Log("MexcV3SocketClientSpotStreams");

        #region fields
        private readonly string _baseAddressAuthenticated;
        private readonly string _baseAddressMbp;

        private const string tradesStreamEndpoint = "spot@public.deals.v3.api";             //逐笔交易(实时)
        private const string klineStreamEndpoint = "spot@public.kline.v3.api";              //K线 Streams
        private const string depthStreamEndpoint = "spot@public.increase.depth.v3.api";     //增量深度信息(实时)

        private const string privateDealsUpdateEvent = "spot@private.deals.v3.api";         //账户成交(实时)
        private const string privateOrdersUpdateEvent = "spot@private.orders.v3.api";       //账户订单(实时)

        private const string bookTickerStreamEndpoint = "@bookTicker";
        private const string allBookTickerStreamEndpoint = "!bookTicker";


        private const string aggregatedTradesStreamEndpoint = "@aggTrade";
        private const string symbolTickerStreamEndpoint = "@ticker";
        private const string allSymbolTickerStreamEndpoint = "!ticker@arr";
        private const string partialBookDepthStreamEndpoint = "@depth";
        private const string symbolMiniTickerStreamEndpoint = "@miniTicker";
        private const string allSymbolMiniTickerStreamEndpoint = "!miniTicker@arr";

        private const string bltvInfoEndpoint = "@tokenNav";
        private const string bltvKlineEndpoint = "@nav_kline";

        private const string executionUpdateEvent = "executionReport";
        private const string ocoOrderUpdateEvent = "listStatus";
        private const string accountPositionUpdateEvent = "outboundAccountPosition";
        private const string balanceUpdateEvent = "balanceUpdate";
        #endregion

        #region constructor/destructor

        /// <summary>
        /// Create a new public instance of MexcSocketClientSpot with default options
        /// </summary>
        public MexcV3SocketClientSpotStreams(Log log, MexcV3SocketClientOptions options) :
            base(log, options, options.SpotStreamsOptions)
        {
            _log = log;
            _baseAddressAuthenticated = options.SpotStreamsOptions.BaseAddressAuthenticated;
            _baseAddressMbp = options.SpotStreamsOptions.BaseAddressInrementalOrderBook;
        }

        /// <inheritdoc />
        protected override AuthenticationProvider CreateAuthenticationProvider(ApiCredentials credentials)
            => new MexcV3AuthenticationProvider(credentials);
        #endregion

        #region methods

        #region Trade Streams(public auth)

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToTradeUpdatesAsync(string symbol,
            Action<DataEvent<MexcV3StreamTrade>> onMessage, CancellationToken ct = default) =>
            await SubscribeToTradeUpdatesAsync(new[] { symbol }, onMessage, ct).ConfigureAwait(false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbols"></param>
        /// <param name="onMessage"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<CallResult<UpdateSubscription>> SubscribeToTradeUpdatesAsync(
            IEnumerable<string> symbols,
            Action<DataEvent<MexcV3StreamTrade>> onMessage,
            CancellationToken ct = default)
        {
            symbols.ValidateNotNull(nameof(symbols));
            foreach (var symbol in symbols)
            {
                symbol.ValidateMexcSymbol();
            }
            var request = new MexcV3SubscribeRequest(NextId().ToString(CultureInfo.InvariantCulture), $"market.*.trade.detail");
            Action<DataEvent<MexcV3StreamTrade>>? handler = new Action<DataEvent<MexcV3StreamTrade>>(data => onMessage(data));
            symbols = symbols.Select(a =>
                $"{tradesStreamEndpoint}" +
                $"@{a.ToUpper(CultureInfo.InvariantCulture)}"
                ).ToArray();
            CallResult<UpdateSubscription>? response = await SubscribeAsync(
                request: symbols,
                url: BaseAddress,
                identifier: $"market.*.trade.detail",
                authenticated: true,
                dataHandler: handler,
                ct: ct
                ).ConfigureAwait(false);
            return response;
        }
        #endregion

        #region Kline/Candlestick Streams(public auth)

        internal Task<CallResult<UpdateSubscription>> SubscribeAsync<T>(string url, IEnumerable<string> topics, Action<DataEvent<T>> onData, CancellationToken ct)
        {
            MexcV3SocketRequest? request = new MexcV3SocketRequest
            {
                Method = "SUBSCRIPTION",
                Params = topics.ToArray(),
            };

            Task<CallResult<UpdateSubscription>>? response = SubscribeAsync(url.AppendPath("ws"), request, null, false, onData, ct);
            if (response.Status != TaskStatus.WaitingForActivation)
            {
                Console.WriteLine(response.Status.ToString());
            }
            return response;
        }

        internal Task<CallResult<UpdateSubscription>> SubscribeAsync<T>(string url, IEnumerable<string> topics, string listenKey, Action<DataEvent<T>> onData, CancellationToken ct)
        {
            MexcV3SocketRequest? request = new MexcV3SocketRequest
            {
                Method = "SUBSCRIPTION",
                Params = topics.ToArray(),
            };

            Task<CallResult<UpdateSubscription>>? response = SubscribeAsync(url.AppendPath($"ws?listenKey={listenKey}"), request, null, false, onData, ct);
            if (response.Status != TaskStatus.WaitingForActivation)
            {
                Console.WriteLine(response.Status.ToString());
            }
            return response;
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToKlineUpdatesAsync(string symbol, MexcV3StreamsKlineInterval interval, 
            Action<DataEvent<MexcV3StreamKline>> onMessage, CancellationToken ct = default) =>
            await SubscribeToKlineUpdatesAsync(new[] { symbol }, interval, onMessage, ct).ConfigureAwait(false);

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToKlineUpdatesAsync(string symbol, IEnumerable<MexcV3StreamsKlineInterval> intervals, 
            Action<DataEvent<MexcV3StreamKline>> onMessage, CancellationToken ct = default) =>
            await SubscribeToKlineUpdatesAsync(new[] { symbol }, intervals, onMessage, ct).ConfigureAwait(false);

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToKlineUpdatesAsync(IEnumerable<string> symbols, MexcV3StreamsKlineInterval interval, 
            Action<DataEvent<MexcV3StreamKline>> onMessage, CancellationToken ct = default) =>
            await SubscribeToKlineUpdatesAsync(symbols, new[] { interval }, onMessage, ct).ConfigureAwait(false);

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToKlineUpdatesAsync(IEnumerable<string> symbols, IEnumerable<MexcV3StreamsKlineInterval> intervals, 
            Action<DataEvent<MexcV3StreamKline>> onMessage, CancellationToken ct = default)
        {
            symbols.ValidateNotNull(nameof(symbols));
            foreach (var symbol in symbols)
                symbol.ValidateMexcSymbol();
            Action<DataEvent<MexcV3StreamKline>>? handler = new Action<DataEvent<MexcV3StreamKline>>(data => onMessage(data));
            symbols = symbols.SelectMany(a =>
                intervals.Select(i =>
                    $"{klineStreamEndpoint}" +
                    $"@{a.ToUpper(CultureInfo.InvariantCulture)}" +
                    $"@{JsonConvert.SerializeObject(i, new MexcV3StreamsKlineIntervalConverter(false))}"
                    )
                ).ToArray();
            CallResult<UpdateSubscription>? response = await SubscribeAsync(
                request:symbols,
                url:BaseAddress,
                identifier:null,
                authenticated:false,
                dataHandler:handler,
                ct:ct
                ).ConfigureAwait(false);
            return response;
        }

        #endregion

        #region Diff. Depth Stream(public auth)

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToDiffDepthUpdatesAsync(string symbol,
            Action<DataEvent<MexcV3StreamDepth>> onMessage, CancellationToken ct = default) =>
            await SubscribeToDiffDepthUpdatesAsync(new[] { symbol }, onMessage, ct).ConfigureAwait(false);

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToDiffDepthUpdatesAsync(IEnumerable<string> symbols,
            Action<DataEvent<MexcV3StreamDepth>> onMessage, CancellationToken ct = default)
        {
            symbols.ValidateNotNull(nameof(symbols));
            foreach (var symbol in symbols)
                symbol.ValidateMexcSymbol();
            Action<DataEvent<MexcV3StreamDepth>>? handler = new Action<DataEvent<MexcV3StreamDepth>>(data => onMessage(data));
            symbols = symbols.Select(a =>
                $"{depthStreamEndpoint}" +
                $"@{a.ToUpper(CultureInfo.InvariantCulture)}"
                ).ToArray();
            CallResult<UpdateSubscription>? response = await SubscribeAsync(
                request: symbols,
                url: BaseAddress,
                identifier: null,
                authenticated: false,
                dataHandler: handler,
                ct: ct
                ).ConfigureAwait(false);
            return response;
        }

        #endregion

        #region deals Stream(private auth)

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToPrivateDealsUpdatesAsync(
            string listenKey,
            Action<DataEvent<MexcV3StreamPrivateDeals>>? onMessage,
            CancellationToken ct = default
            )
        {
            listenKey.ValidateNotNull(nameof(listenKey));

            #region 方法一：
            Action<DataEvent<MexcV3StreamPrivateDeals>>? handler = new Action<DataEvent<MexcV3StreamPrivateDeals>>(data => onMessage(data));
            CallResult<UpdateSubscription>? response = await SubscribeAsync(BaseAddress, new[] { privateDealsUpdateEvent }, listenKey, handler, ct).ConfigureAwait(false);
            return response;
            #endregion

            #region 方法二(已经通过，备用误删）：
            //Action<DataEvent<MexcV3StreamPrivateDeals>>? handler = new Action<DataEvent<MexcV3StreamPrivateDeals>>(data =>
            //{
            //    JToken? combinedToken = JToken.Parse(data.Data.ToString());
            //    JToken? token = combinedToken["d"];
            //    if (token == null)
            //        return;

            //    string? evnt = token.Root["c"]?.ToString();
            //    if (evnt == null)
            //        return;

            //    CallResult<MexcV3StreamPrivateDeals>? result = _baseClient.DeserializeInternal<MexcV3StreamPrivateDeals>(combinedToken);
            //    if (result)
            //    {
            //        result.Data.Stream = combinedToken["c"]!.Value<string>()!;
            //        result.Data.ListenKey = listenKey;
            //        onMessage?.Invoke(data);
            //    }
            //    else
            //        _log.Write(LogLevel.Warning, "Couldn't deserialize data received from private deals stream: " + result.Error);
            //});
            //return await _baseClient.SubscribeInternal(this, BaseAddress, new[] { privateDealsUpdateEvent }, listenKey, handler, ct).ConfigureAwait(false);
            #endregion
        }
        #endregion

        #region orders Stream(private auth)

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToPrivateOrdersUpdatesAsync(
            string listenKey,
            Action<DataEvent<MexcV3StreamPrivateOrders>>? onMessage,
            CancellationToken ct = default)
        {
            listenKey.ValidateNotNull(nameof(listenKey));

            #region 方法一：
            Action<DataEvent<MexcV3StreamPrivateOrders>>? handler = new Action<DataEvent<MexcV3StreamPrivateOrders>>(data => onMessage(data));
            CallResult<UpdateSubscription>? response = await SubscribeAsync(BaseAddress, new[] { privateOrdersUpdateEvent }, listenKey, handler, ct).ConfigureAwait(false);
            return response;
            #endregion

            #region 方法二(已经通过，备用误删）：
            //Action<DataEvent<MexcV3StreamPrivateOrders>>? handler = new Action<DataEvent<MexcV3StreamPrivateOrders>>(data =>
            //{
            //    JToken? combinedToken = JToken.Parse(data.Data.ToString());
            //    JToken? token = combinedToken["d"];
            //    if (token == null)
            //        return;

            //    string? evnt = token.Root["c"]?.ToString();
            //    if (evnt == null)
            //        return;

            //    CallResult<MexcV3StreamPrivateOrders>? result = _baseClient.DeserializeInternal<MexcV3StreamPrivateOrders>(combinedToken);
            //    if (result)
            //    {
            //        result.Data.Stream = combinedToken["c"]!.Value<string>()!;
            //        result.Data.ListenKey = listenKey;
            //        onMessage?.Invoke(data);
            //    }
            //    else
            //        _log.Write(LogLevel.Warning, "Couldn't deserialize data received from private orders stream: " + result.Error);
            //});
            //return await _baseClient.SubscribeInternal(this, BaseAddress, new[] { privateOrdersUpdateEvent }, listenKey, handler, ct).ConfigureAwait(false);
            #endregion

        }
        #endregion

        #region private
        /// <inheritdoc />
        protected override bool HandleQueryResponse<T>(SocketConnection s, object request, JToken data, out CallResult<T> callResult)
        {
            throw new NotImplementedException();

            //callResult = new CallResult<T>(default(T)!);
            //var v1Data = (data["data"] != null || data["tick"] != null) && data["rep"] != null;
            //var v1Error = data["status"] != null && data["status"]!.ToString() == "error";
            //var isV1QueryResponse = v1Data || v1Error;
            //if (isV1QueryResponse)
            //{
            //    var hRequest = (MexcV3SocketRequest)request;
            //    var id = data["id"];
            //    if (id == null)
            //        return false;

            //    if (id.ToString() != hRequest.Id)
            //        return false;

            //    if (v1Error)
            //    {
            //        var error = new ServerError(data["err-msg"]!.ToString());
            //        callResult = new CallResult<T>(error);
            //        return true;
            //    }

            //    var desResult = Deserialize<T>(data);
            //    if (!desResult)
            //    {
            //        _log.Write(LogLevel.Warning, $"Socket {s.SocketId} Failed to deserialize data: {desResult.Error}. Data: {data}");
            //        callResult = new CallResult<T>(desResult.Error!);
            //        return true;
            //    }

            //    callResult = new CallResult<T>(desResult.Data);
            //    return true;
            //}

            //var action = data["action"]?.ToString();
            //var isV2Response = action == "req";
            //if (isV2Response)
            //{
            //    var hRequest = (HuobiAuthenticatedSubscribeRequest)request;
            //    var channel = data["ch"]?.ToString();
            //    if (channel != hRequest.Channel)
            //        return false;

            //    var desResult = Deserialize<T>(data);
            //    if (!desResult)
            //    {
            //        _log.Write(LogLevel.Warning, $"Socket {s.SocketId} Failed to deserialize data: {desResult.Error}. Data: {data}");
            //        return false;
            //    }

            //    callResult = new CallResult<T>(desResult.Data);
            //    return true;
            //}

            //return false;
        }

        /// <inheritdoc />
        protected override bool HandleSubscriptionResponse(SocketConnection s, SocketSubscription subscription, object request, JToken message, out CallResult<object>? callResult)
        {
            callResult = null;
            if (message.Type != JTokenType.Object)
                return false;

            JToken? id = message["id"];
            if (id == null)
                return false;

            JToken? code = message["code"];
            if (code == null)
                return false;

            JToken? msg = message["msg"];
            if (msg == null)
                return false;

            MexcV3SocketRequest bRequest = (MexcV3SocketRequest)request;
            if (msg.ToString().IndexOf("no subscription success", StringComparison.OrdinalIgnoreCase) != -1)
            {
                _log.Write(LogLevel.Error, $"Socket Subscription error : {msg}\r\n");
                return false;
            }

            JToken? result = message["msg"];
            string[] resultItemArray = result.ToString().Split(',');
            bool successedSubscrip = false;
            foreach (string? requestStream in bRequest.Params)
            {
                foreach (string? responceItem in resultItemArray)
                {
                    if (requestStream == responceItem)
                    {
                        _log.Write(LogLevel.Trace, $"Socket Subscription {requestStream} completed\r\n");
                        successedSubscrip = true;
                    }
                }
            }
            if (successedSubscrip)
            {
                callResult = new CallResult<object>(new object());
                return true;
            }
            else
            {
                return false;
            }

            var error = message["error"];
            if (error == null)
            {
                callResult = new CallResult<object>(new ServerError("Unknown error: " + message));
                return true;
            }

            callResult = new CallResult<object>(new ServerError(error["code"]!.Value<int>(), error["msg"]!.ToString()));
            return true;


            //callResult = null;
            //var status = message["status"]?.ToString();
            //var isError = status == "error";
            //if (isError)
            //{
            //    if (request is HuobiSubscribeRequest hRequest)
            //    {
            //        var subResponse = Deserialize<HuobiSubscribeResponse>(message);
            //        if (!subResponse)
            //        {
            //            _log.Write(LogLevel.Warning, $"Socket {s.SocketId} Subscription failed: " + subResponse.Error);
            //            return false;
            //        }

            //        var id = subResponse.Data.Id;
            //        if (id != hRequest.Id)
            //            return false; // Not for this request

            //        _log.Write(LogLevel.Warning, $"Socket {s.SocketId} Subscription failed: " + subResponse.Data.ErrorMessage);
            //        callResult = new CallResult<object>(new ServerError($"{subResponse.Data.ErrorCode}, {subResponse.Data.ErrorMessage}"));
            //        return true;
            //    }

            //    if (request is HuobiAuthenticatedSubscribeRequest haRequest)
            //    {
            //        var subResponse = Deserialize<HuobiAuthSubscribeResponse>(message);
            //        if (!subResponse)
            //        {
            //            _log.Write(LogLevel.Warning, $"Socket {s.SocketId} Subscription failed: " + subResponse.Error);
            //            callResult = new CallResult<object>(subResponse.Error!);
            //            return false;
            //        }

            //        var id = subResponse.Data.Channel;
            //        if (id != haRequest.Channel)
            //            return false; // Not for this request

            //        _log.Write(LogLevel.Warning, $"Socket {s.SocketId} Subscription failed: " + subResponse.Data.Code);
            //        callResult = new CallResult<object>(new ServerError(subResponse.Data.Code, "Failed to subscribe"));
            //        return true;
            //    }
            //}

            //var v1Sub = message["subbed"] != null;
            //if (v1Sub)
            //{
            //    var subResponse = Deserialize<HuobiSubscribeResponse>(message);
            //    if (!subResponse)
            //    {
            //        _log.Write(LogLevel.Warning, $"Socket {s.SocketId} Subscription failed: " + subResponse.Error);
            //        return false;
            //    }

            //    var hRequest = (HuobiSubscribeRequest)request;
            //    if (subResponse.Data.Id != hRequest.Id)
            //        return false;

            //    if (!subResponse.Data.IsSuccessful)
            //    {
            //        _log.Write(LogLevel.Warning, $"Socket {s.SocketId} Subscription failed: " + subResponse.Data.ErrorMessage);
            //        callResult = new CallResult<object>(new ServerError($"{subResponse.Data.ErrorCode}, {subResponse.Data.ErrorMessage}"));
            //        return true;
            //    }

            //    _log.Write(LogLevel.Debug, $"Socket {s.SocketId} Subscription completed");
            //    callResult = new CallResult<object>(subResponse.Data);
            //    return true;
            //}

            //var action = message["action"]?.ToString();
            //var v2Sub = action == "sub";
            //if (v2Sub)
            //{
            //    var subResponse = Deserialize<HuobiAuthSubscribeResponse>(message);
            //    if (!subResponse)
            //    {
            //        _log.Write(LogLevel.Warning, $"Socket {s.SocketId} Subscription failed: " + subResponse.Error);
            //        callResult = new CallResult<object>(subResponse.Error!);
            //        return false;
            //    }

            //    var hRequest = (HuobiAuthenticatedSubscribeRequest)request;
            //    if (subResponse.Data.Channel != hRequest.Channel)
            //        return false;

            //    if (!subResponse.Data.IsSuccessful)
            //    {
            //        _log.Write(LogLevel.Warning, $"Socket {s.SocketId} Subscription failed: " + subResponse.Data.Message);
            //        callResult = new CallResult<object>(new ServerError(subResponse.Data.Code, subResponse.Data.Message));
            //        return true;
            //    }

            //    _log.Write(LogLevel.Debug, $"Socket {s.SocketId} Subscription completed");
            //    callResult = new CallResult<object>(subResponse.Data);
            //    return true;
            //}

            //var operation = message["op"]?.ToString();
            //var usdtMarginSub = operation == "sub";
            //if (usdtMarginSub)
            //{
            //    var subResponse = Deserialize<HuobiSocketResponse2>(message);
            //    if (!subResponse)
            //    {
            //        _log.Write(LogLevel.Warning, $"Socket {s.SocketId} Subscription failed: " + subResponse.Error);
            //        callResult = new CallResult<object>(subResponse.Error!);
            //        return false;
            //    }

            //    var hRequest = (HuobiSocketRequest2)request;
            //    if (subResponse.Data.Topic != hRequest.Topic)
            //        return false;

            //    if (!subResponse.Data.IsSuccessful)
            //    {
            //        _log.Write(LogLevel.Warning, $"Socket {s.SocketId} Subscription failed: " + subResponse.Data.ErrorMessage);
            //        callResult = new CallResult<object>(new ServerError(subResponse.Data.ErrorCode + " - " + subResponse.Data.ErrorMessage));
            //        return true;
            //    }

            //    _log.Write(LogLevel.Debug, $"Socket {s.SocketId} Subscription completed");
            //    callResult = new CallResult<object>(subResponse.Data);
            //    return true;
            //}

            //return false;
        }

        /// <inheritdoc />
        protected override bool MessageMatchesHandler(SocketConnection socketConnection, JToken message, object request)
        {
            if (message.Type != JTokenType.Object)
                return false;

            MexcV3SocketRequest? bRequest = (MexcV3SocketRequest)request;
            var stream = message["c"];
            if (stream == null)
                return false;

            return bRequest.Params.Contains(stream.ToString());


            //if (request is HuobiSubscribeRequest hRequest)
            //    return hRequest.Topic == message["ch"]?.ToString();

            //if (request is HuobiAuthenticatedSubscribeRequest haRequest)
            //    return haRequest.Channel == message["ch"]?.ToString();

            //if (request is HuobiSocketRequest2 hRequest2)
            //{
            //    if (hRequest2.Topic == message["topic"]?.ToString())
            //        return true;

            //    if (hRequest2.Topic.Contains("*") && hRequest2.Topic.Split('.')[0] == message["topic"]?.ToString().Split('.')[0])
            //        return true;
            //}

            //return false;
        }

        /// <inheritdoc />
        protected override bool MessageMatchesHandler(SocketConnection socketConnection, JToken message, string identifier)
        {
            return true;
            //if (message.Type != JTokenType.Object)
            //    return false;

            //if (identifier == "PingV1" && message["ping"] != null)
            //    return true;

            //if (identifier == "PingV2" && message["action"]?.ToString() == "ping")
            //    return true;

            //if (identifier == "PingV3" && message["op"]?.ToString() == "ping")
            //    return true;

            //return false;
        }

        /// <inheritdoc />
        protected override async Task<CallResult<bool>> AuthenticateSocketAsync(SocketConnection s)
        {
            throw new NotImplementedException();

            //if (s.ApiClient.AuthenticationProvider == null)
            //    return new CallResult<bool>(new NoApiCredentialsError());

            //var result = new CallResult<bool>(new ServerError("No response from server"));
            //if (s.ApiClient is HuobiSocketClientUsdtMarginSwapStreams)
            //{
            //    await s.SendAndWaitAsync(((HuobiAuthenticationProvider)s.ApiClient.AuthenticationProvider).GetWebsocketAuthentication2(s.ConnectionUri), Options.SocketResponseTimeout, data =>
            //    {
            //        if (data["op"]?.ToString() != "auth")
            //            return false;

            //        var authResponse = Deserialize<HuobiAuthResponse>(data);
            //        if (!authResponse)
            //        {
            //            _log.Write(LogLevel.Warning, $"Socket {s.SocketId} Authorization failed: " + authResponse.Error);
            //            result = new CallResult<bool>(authResponse.Error!);
            //            return true;
            //        }
            //        if (!authResponse.Data.IsSuccessful)
            //        {
            //            _log.Write(LogLevel.Warning, $"Socket {s.SocketId} Authorization failed: " + authResponse.Data.Message);
            //            result = new CallResult<bool>(new ServerError(authResponse.Data.Code, authResponse.Data.Message));
            //            return true;
            //        }

            //        _log.Write(LogLevel.Debug, $"Socket {s.SocketId} Authorization completed");
            //        result = new CallResult<bool>(true);
            //        return true;
            //    }).ConfigureAwait(false);
            //}
            //else
            //{
            //    await s.SendAndWaitAsync(((HuobiAuthenticationProvider)s.ApiClient.AuthenticationProvider).GetWebsocketAuthentication(s.ConnectionUri), Options.SocketResponseTimeout, data =>
            //    {
            //        if (data["ch"]?.ToString() != "auth")
            //            return false;

            //        var authResponse = Deserialize<HuobiAuthSubscribeResponse>(data);
            //        if (!authResponse)
            //        {
            //            _log.Write(LogLevel.Warning, $"Socket {s.SocketId} Authorization failed: " + authResponse.Error);
            //            result = new CallResult<bool>(authResponse.Error!);
            //            return true;
            //        }
            //        if (!authResponse.Data.IsSuccessful)
            //        {
            //            _log.Write(LogLevel.Warning, $"Socket {s.SocketId} Authorization failed: " + authResponse.Data.Message);
            //            result = new CallResult<bool>(new ServerError(authResponse.Data.Code, authResponse.Data.Message));
            //            return true;
            //        }

            //        _log.Write(LogLevel.Debug, $"Socket {s.SocketId} Authorization completed");
            //        result = new CallResult<bool>(true);
            //        return true;
            //    }).ConfigureAwait(false);
            //}

            //return result;
        }

        /// <inheritdoc />
        protected override async Task<bool> UnsubscribeAsync(SocketConnection connection, SocketSubscription s)
        {
            var topics = ((MexcV3SocketRequest)s.Request!).Params;
            var unsub = new MexcV3SocketRequest
            {
                Method = "UNSUBSCRIPTION",
                Params = topics
            };
            var result = false;

            if (!connection.Connected)
                return true;

            await connection.SendAndWaitAsync(unsub, Options.SocketResponseTimeout, data =>
            {
                if (data.Type != JTokenType.Object)
                    return false;

                var id = data["id"];
                if (id == null)
                    return false;

                //if ((int)id != unsub.Id)
                //    return false;

                var result = data["result"];
                if (result?.Type == JTokenType.Null)
                {
                    result = true;
                    return true;
                }

                return true;
            }).ConfigureAwait(false);
            return result;

            //var result = false;
            //if (s.Request is HuobiSubscribeRequest hRequest)
            //{
            //    var unsubId = NextId().ToString();
            //    var unsub = new HuobiUnsubscribeRequest(unsubId, hRequest.Topic);

            //    await connection.SendAndWaitAsync(unsub, Options.SocketResponseTimeout, data =>
            //    {
            //        if (data.Type != JTokenType.Object)
            //            return false;

            //        var id = data["id"]?.ToString();
            //        if (id == unsubId)
            //        {
            //            result = data["status"]?.ToString() == "ok";
            //            return true;
            //        }

            //        return false;
            //    }).ConfigureAwait(false);
            //    return result;
            //}

            //if (s.Request is HuobiAuthenticatedSubscribeRequest haRequest)
            //{
            //    var unsub = new Dictionary<string, object>()
            //    {
            //        { "action", "unsub" },
            //        { "ch", haRequest.Channel },
            //    };

            //    await connection.SendAndWaitAsync(unsub, Options.SocketResponseTimeout, data =>
            //    {
            //        if (data.Type != JTokenType.Object)
            //            return false;

            //        if (data["action"]?.ToString() == "unsub" && data["ch"]?.ToString() == haRequest.Channel)
            //        {
            //            result = data["code"]?.Value<int>() == 200;
            //            return true;
            //        }

            //        return false;
            //    }).ConfigureAwait(false);
            //    return result;
            //}

            //if (s.Request is HuobiSocketRequest2 hRequest2)
            //{
            //    var unsub = new
            //    {
            //        op = "unsub",
            //        topic = hRequest2.Topic,
            //        cid = NextId().ToString()
            //    };
            //    await connection.SendAndWaitAsync(unsub, Options.SocketResponseTimeout, data =>
            //    {
            //        if (data.Type != JTokenType.Object)
            //            return false;

            //        if (data["op"]?.ToString() == "unsub" && data["cid"]?.ToString() == unsub.cid)
            //        {
            //            result = data["err-code"]?.Value<int>() == 0;
            //            return true;
            //        }

            //        return false;
            //    }).ConfigureAwait(false);
            //    return result;
            //}

            //throw new InvalidOperationException("Unknown request type");
        }
        #endregion

        #endregion
    }
}
