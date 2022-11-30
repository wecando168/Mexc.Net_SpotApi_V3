using Mexc.Net.Interfaces.Clients.SpotApi;
using CryptoExchange.Net.Interfaces;
using Mexc.Net.Interfaces.Clients.Futures;

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
        public IMexcV3SocketClientSpotStreams SpotPublicStreams { get; }

        /// <summary>
        /// Spot streams
        /// </summary>
        public IMexcV3SocketClientSpotStreams SpotPrivateStreams { get; }

        /// <summary>
        /// Futures streams
        /// </summary>
        public IMexcV3SocketClientFuturesStreams FuturesPublicStreams { get; }

        /// <summary>
        /// Futures streams
        /// </summary>
        public IMexcV3SocketClientFuturesStreams FuturesPrivateStreams { get; }
    }
}
