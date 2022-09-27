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
        public string RestClientAddress { get; set; } = string.Empty;

        /// <summary>
        /// The address used by the MexcSocketClient for the Spot Public API
        /// </summary>
        public string SpotPublicSocketClientAddress { get; set; } = string.Empty;

        /// <summary>
        /// The address used by the MexcSocketClient for the Spot Private API
        /// </summary>
        public string SpotUserSocketClientAddress { get; set; } = string.Empty;

        /// <summary>
        /// The address used by the MexcClient for the futures API
        /// </summary>
        public string? FuturesRestClientAddress { get; set; }
        /// <summary>
        /// The address used by the MexcSocketClient for the futures API
        /// </summary>
        public string? FuturesSocketClientAddress { get; set; }

        /// <summary>
        /// The default addresses to connect to the mexc.com API
        /// </summary>
        public static MexcV3ApiAddresses Default = new MexcV3ApiAddresses
        {
            RestClientAddress = "https://api.mexc.com",
            SpotPublicSocketClientAddress = "wss://wbs.mexc.com",
            SpotUserSocketClientAddress = "wss://wbs.mexc.me",
            FuturesRestClientAddress = "https://contract.mexc.com",
            FuturesSocketClientAddress = "wss://contract.mexc.com/ws",
        };

        /// <summary>
        /// The addresses to connect to the mexc testnet
        /// </summary>
        public static MexcV3ApiAddresses TestNet = new MexcV3ApiAddresses
        {
            RestClientAddress = "",
            SpotPublicSocketClientAddress = "",
            SpotUserSocketClientAddress = "",
            FuturesRestClientAddress = "",
            FuturesSocketClientAddress = "",
        };
    }
}
