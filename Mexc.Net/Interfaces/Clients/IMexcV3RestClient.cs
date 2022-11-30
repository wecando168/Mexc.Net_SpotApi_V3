using Mexc.Net.Interfaces.Clients.SpotApi;
using CryptoExchange.Net.Interfaces;
using Mexc.Net.Interfaces.Clients.Futures;

namespace Mexc.Net.Interfaces.Clients
{
    /// <summary>
    /// Client for accessing the Mexc Rest API. 
    /// </summary>
    public interface IMexcV3RestClient: IRestClient
    { 
        /// <summary>
        /// Spot API endpoints
        /// </summary>
        IMexcV3ClientSpotApi SpotApi { get; }

        /// <summary>
        /// Usdt margin swap endpoints
        /// </summary>
        IMexcClientFutresApi FutresApi { get; }
    }
}
