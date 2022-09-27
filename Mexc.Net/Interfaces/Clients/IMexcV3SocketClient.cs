﻿using Mexc.Net.Interfaces.Clients.SpotApi;
using CryptoExchange.Net.Interfaces;

namespace Mexc.Net.Interfaces.Clients
{
    /// <summary>
    /// Client for accessing the Mexc websocket API
    /// </summary>
    public interface IMexcV3SocketClient : ISocketClient
    {
        /// <summary>
        /// Spot streams
        /// </summary>
        IMexcV3SocketClientSpotStreams SpotPublicStreams { get; }
    }
}
