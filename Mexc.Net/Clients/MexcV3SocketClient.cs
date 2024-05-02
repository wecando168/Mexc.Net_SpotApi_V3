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
using CryptoExchange.Net.Logging;
using Microsoft.Extensions.Logging;
using Mexc.Net.Interfaces.Clients.Futures;

namespace Mexc.Net.Clients
{
    /// <inheritdoc cref="IMexcV3SocketClient" />
    public class MexcV3SocketClient : BaseSocketClient, IMexcV3SocketClient
    {
        private Log _log = new Log("MexcV3SocketClient");

        #region fields
        #endregion

        #region Api clients

        /// <inheritdoc />
        public IWWTMexcV3SocketClientSpotStreams SpotPublicStreams { get; set; }

        /// <inheritdoc />
        public IWWTMexcV3SocketClientSpotStreams SpotPrivateStreams { get; set; }

        /// <inheritdoc />
        public IMexcV3SocketClientFuturesStreams FuturesPublicStreams { get; set; }

        /// <inheritdoc />
        /// <inheritdoc />
        public IMexcV3SocketClientFuturesStreams FuturesPrivateStreams { get; set; }

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
            SpotPublicStreams = AddApiClient(new WWTMexcV3SocketClientSpotStreams(log, options));
            SpotPrivateStreams = AddApiClient(new WWTMexcV3SocketClientSpotStreams(log, options));
            FuturesPublicStreams = AddApiClient(new MexcV3SocketClientFuturesStreams (log, options));
            FuturesPrivateStreams = AddApiClient(new MexcV3SocketClientFuturesStreams(log, options));
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

        //internal CallResult<T> DeserializeInternal<T>(JToken obj, JsonSerializer? serializer = null, int? requestId = null)
        //    => Deserialize<T>(obj, serializer, requestId);

        //internal Task<CallResult<UpdateSubscription>> SubscribeInternal<T>(SocketApiClient apiClient, string url, IEnumerable<string> topics, Action<DataEvent<T>> onData, CancellationToken ct)
        //{
        //    MexcV3SocketRequest? request = new MexcV3SocketRequest
        //    {
        //        Method = "SUBSCRIPTION",
        //        Params = topics.ToArray(),
        //    };

        //    Task<CallResult<UpdateSubscription>>? response = SubscribeAsync(apiClient, url.AppendPath("ws"), request, null, false, onData, ct);
        //    if(response.Status != TaskStatus.WaitingForActivation)
        //    {
        //        _log.Write(LogLevel.Trace, response.Status.ToString());
        //    }
        //    return response;
        //}

        //internal Task<CallResult<UpdateSubscription>> SubscribeInternal<T>(SocketApiClient apiClient, string url, IEnumerable<string> topics, string listenKey, Action<DataEvent<T>> onData, CancellationToken ct)
        //{
        //    MexcV3SocketRequest? request = new MexcV3SocketRequest
        //    {
        //        Method = "SUBSCRIPTION",
        //        Params = topics.ToArray(),
        //    };

        //    Task<CallResult<UpdateSubscription>>? response = SubscribeAsync(apiClient, url.AppendPath($"ws?listenKey={listenKey}"), request, null, false, onData, ct);
        //    if (response.Status != TaskStatus.WaitingForActivation)
        //    {
        //        _log.Write(LogLevel.Trace, response.Status.ToString());
        //    }
        //    return response;
        //}
        #endregion
    }
}
