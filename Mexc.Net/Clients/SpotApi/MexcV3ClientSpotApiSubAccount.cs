using Mexc.Net.Interfaces.Clients.SpotApi;
using CryptoExchange.Net.Logging;

namespace Mexc.Net.Clients.SpotApi
{
    /// <inheritdoc />
    public class MexcV3ClientSpotApiSubAccount : IMexcV3ClientSpotApiSubAccount
    {
        private const string xxxxxxxxEndpoint = "xxxxxxxx";

        private const string api = "api";
        private const string publicVersion = "3";

        private readonly Log _log;

        private readonly MexcV3ClientSpotApi _baseClient;

        internal MexcV3ClientSpotApiSubAccount(Log log, MexcV3ClientSpotApi baseClient)
        {
            _log = log;
            _baseClient = baseClient;
        }
    }
}
