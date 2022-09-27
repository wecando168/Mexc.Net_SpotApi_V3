using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mexc.Net.Clients.SpotApi;
using Mexc.Net.Interfaces.Clients;
using Mexc.Net.Interfaces.Clients.SpotApi;
using Mexc.Net.Objects;
using Mexc.Net.Objects.Internal;
using CryptoExchange.Net;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mexc.Net.Clients
{
    /// <inheritdoc cref="IMexcV3SocketClient" />
    public class MexcV3SocketClient : BaseSocketClient, IMexcV3SocketClient
    {
        #region fields
        #endregion

        #region Api clients

        /// <inheritdoc />
        public IMexcV3SocketClientSpotStreams SpotPublicStreams { get; set; }

        /// <inheritdoc />
        public IMexcV3SocketClientSpotStreams SpotPrivateStreams { get; set; }

        #endregion

        #region constructor/destructor

        /// <summary>
        /// Create a new instance of MexcSocketClientSpot with default options
        /// </summary>
        public MexcV3SocketClient() : this(MexcV3SocketClientOptions.Default)
        {
        }

        /// <summary>
        /// Create a new instance of MexcSocketClientSpot using provided options
        /// </summary>
        /// <param name="options">The options to use for this client</param>
        public MexcV3SocketClient(MexcV3SocketClientOptions options) : base("Mexc", options)
        {
            SetDataInterpreter((data) => string.Empty, null);
            RateLimitPerSocketPerSecond = 4;

            SpotPublicStreams = AddApiClient(new MexcV3SocketClientSpotStreams(log, this, options));
            SpotPrivateStreams = AddApiClient(new MexcV3SocketClientSpotStreams(log, this, options));
        }
        #endregion 

        #region methods

        /// <summary>
        /// Set the default options to be used when creating new clients
        /// </summary>
        /// <param name="options">Options to use as default</param>
        public static void SetDefaultOptions(MexcV3SocketClientOptions options)
        {
            MexcV3SocketClientOptions.Default = options;
        }

        internal CallResult<T> DeserializeInternal<T>(JToken obj, JsonSerializer? serializer = null, int? requestId = null)
            => Deserialize<T>(obj, serializer, requestId);

        internal Task<CallResult<UpdateSubscription>> SubscribeInternal<T>(SocketApiClient apiClient, string url, IEnumerable<string> topics, Action<DataEvent<T>> onData, CancellationToken ct)
        {
            MexcV3SocketRequest? request = new MexcV3SocketRequest
            {
                Method = "SUBSCRIPTION",
                Params = topics.ToArray(),
            };

            Task<CallResult<UpdateSubscription>>? response = SubscribeAsync(apiClient, url.AppendPath("ws"), request, null, false, onData, ct);
            if(response.Status != TaskStatus.WaitingForActivation)
            {
                Console.WriteLine(response.Status.ToString());
            }
            return response;
        }

        internal Task<CallResult<UpdateSubscription>> SubscribeInternal<T>(SocketApiClient apiClient, string url, IEnumerable<string> topics, string listenKey, Action<DataEvent<T>> onData, CancellationToken ct)
        {
            MexcV3SocketRequest? request = new MexcV3SocketRequest
            {
                Method = "SUBSCRIPTION",
                Params = topics.ToArray(),
            };

            Task<CallResult<UpdateSubscription>>? response = SubscribeAsync(apiClient, url.AppendPath($"ws?listenKey={listenKey}"), request, null, false, onData, ct);
            if (response.Status != TaskStatus.WaitingForActivation)
            {
                Console.WriteLine(response.Status.ToString());
            }
            return response;
        }

        /// <inheritdoc />
        protected override bool HandleQueryResponse<T>(SocketConnection s, object request, JToken data, out CallResult<T> callResult)
        {
            throw new NotImplementedException();
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
                Console.Write($"Socket Subscription error : {msg}\r\n");
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
                        Console.Write($"Socket Subscription {requestStream} completed\r\n");
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
        }

        /// <inheritdoc />
        protected override bool MessageMatchesHandler(SocketConnection socketConnection, JToken message, string identifier)
        {
            return true;
        }

        /// <inheritdoc />
        protected override Task<CallResult<bool>> AuthenticateSocketAsync(SocketConnection s)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        protected override async Task<bool> UnsubscribeAsync(SocketConnection connection, SocketSubscription subscription)
        {
            var topics = ((MexcV3SocketRequest)subscription.Request!).Params;
            var unsub = new MexcV3SocketRequest
            {
                Method = "UNSUBSCRIPTION",
                Params = topics
            };
            var result = false;

            if (!connection.Connected)
                return true;

            await connection.SendAndWaitAsync(unsub, ClientOptions.SocketResponseTimeout, data =>
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
        }
        #endregion
    }
}
