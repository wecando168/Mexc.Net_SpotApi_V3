namespace Mexc.Net.Objects
{
    /// <summary>
    /// Api addresses
    /// </summary>
    public class MexcV3ApiAddresses
    {
        /// <summary>
        /// The address used by the MexcClient for the Spot API
        /// </summary>
        public string SpotRestClientAddress { get; set; } = string.Empty;

        /// <summary>
        /// The address used by the MexcSocketClient for the Spot Public API
        /// </summary>
        public string SpotPublicSocketClientAddress { get; set; } = string.Empty;

        /// <summary>
        /// The address used by the MexcSocketClient for the Spot Private API
        /// </summary>
        public string SpotPrivaeSocketClientAddress { get; set; } = string.Empty;

        /// <summary>
        /// The address used by the MexcClient for the futures API
        /// </summary>
        public string? FuturesRestClientAddress { get; set; }

        /// <summary>
        /// The address used by the MexcSocketClient for the publice futures API
        /// </summary>
        public string? FuturesPublicSocketClientAddress { get; set; }

        /// <summary>
        /// The address used by the MexcSocketClient for the private futures API
        /// </summary>
        public string? FuturesPrivateSocketClientAddress { get; set; }

        /// <summary>
        /// The default addresses to connect to the mexc.com API
        /// </summary>
        public static MexcV3ApiAddresses Default = new MexcV3ApiAddresses
        {
            SpotRestClientAddress = "https://api.mexc.com",
            SpotPublicSocketClientAddress = "wss://wbs.mexc.com",
            SpotPrivaeSocketClientAddress = "wss://wbs.mexc.me",
            FuturesRestClientAddress = "https://contract.mexc.com",
            FuturesPublicSocketClientAddress = "wss://contract.mexc.com/ws",
            FuturesPrivateSocketClientAddress = "wss://contract.mexc.com/ws",
        };

        /// <summary>
        /// The addresses to connect to the mexc testnet
        /// </summary>
        public static MexcV3ApiAddresses TestNet = new MexcV3ApiAddresses
        {
            SpotRestClientAddress = "",
            SpotPublicSocketClientAddress = "",
            SpotPrivaeSocketClientAddress = "",
            FuturesRestClientAddress = "",
            FuturesPublicSocketClientAddress = "",
            FuturesPrivateSocketClientAddress = "",
        };
    }
}
