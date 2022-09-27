using System;
using System.Collections.Generic;
using System.Net.Http;
using Mexc.Net.Enums;
using Mexc.Net.Interfaces.Clients;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;

namespace Mexc.Net.Objects
{
    /// <summary>
    /// Options for the Mexc client
    /// </summary>
    public class MexcV3ClientOptions : BaseRestClientOptions
    {
        /// <summary>
        /// Default options for the spot client
        /// </summary>
        public static MexcV3ClientOptions Default { get; set; } = new MexcV3ClientOptions();

        /// <summary>
        /// The default receive window for requests
        /// </summary>
        public TimeSpan ReceiveWindow { get; set; } = TimeSpan.FromSeconds(5);

        private MexcV3ApiClientOptions _spotApiOptions = new MexcV3ApiClientOptions(MexcV3ApiAddresses.Default.RestClientAddress)
        {
            AutoTimestamp = true,
            RateLimiters = new List<IRateLimiter>
                {
                    new RateLimiter()
                        .AddPartialEndpointLimit("/api/", 1200, TimeSpan.FromMinutes(1))
                        .AddPartialEndpointLimit("/sapi/", 12000, TimeSpan.FromMinutes(1))
                        .AddEndpointLimit("/api/v3/order", 50, TimeSpan.FromSeconds(10), HttpMethod.Post, true)
                }
        };

        /// <summary>
        /// Spot API options
        /// </summary>
        public MexcV3ApiClientOptions SpotApiOptions
        {
            get => _spotApiOptions;
            set => _spotApiOptions = new MexcV3ApiClientOptions(_spotApiOptions, value);
        }

        private MexcV3ApiClientOptions _futuresApiOptions = new MexcV3ApiClientOptions(MexcV3ApiAddresses.Default.FuturesRestClientAddress!)
        {
            AutoTimestamp = true
        };

        /// <summary>
        /// Usd futures API options
        /// </summary>
        public MexcV3ApiClientOptions FuturesApiOptions
        {
            get => _futuresApiOptions;
            set => _futuresApiOptions = new MexcV3ApiClientOptions(_futuresApiOptions, value);
        }

        /// <summary>
        /// ctor
        /// </summary>
        public MexcV3ClientOptions() : this(Default)
        {
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="baseOn">Base the new options on other options</param>
        internal MexcV3ClientOptions(MexcV3ClientOptions baseOn) : base(baseOn)
        {
            if (baseOn == null)
                return;

            ReceiveWindow = baseOn.ReceiveWindow;

            _spotApiOptions = new MexcV3ApiClientOptions(baseOn.SpotApiOptions, null);
            _futuresApiOptions = new MexcV3ApiClientOptions(baseOn.FuturesApiOptions, null);
        }
    }

    /// <summary>
    /// Mexc socket client options
    /// </summary>
    public class MexcV3SocketClientOptions : BaseSocketClientOptions
    {
        /// <summary>
        /// Default options for the spot client
        /// </summary>
        public static MexcV3SocketClientOptions Default { get; set; } = new MexcV3SocketClientOptions()
        {
            SocketSubscriptionsCombineTarget = 10
        };

        private ApiClientOptions _spotStreamsOptions = new ApiClientOptions(MexcV3ApiAddresses.Default.SpotPublicSocketClientAddress);
        /// <summary>
        /// Spot public streams options
        /// </summary>
        public ApiClientOptions SpotStreamsOptions
        {
            get => _spotStreamsOptions;
            set => _spotStreamsOptions = new ApiClientOptions(_spotStreamsOptions, value);
        }

        private ApiClientOptions _spotUserStreamsOptions = new ApiClientOptions(MexcV3ApiAddresses.Default.SpotUserSocketClientAddress);
        /// <summary>
        /// Spot private streams options
        /// </summary>
        public ApiClientOptions SpotUserStreamsOptions
        {
            get => _spotUserStreamsOptions;
            set => _spotUserStreamsOptions = new ApiClientOptions(_spotUserStreamsOptions, value);
        }

        private ApiClientOptions _futuresStreamsOptions = new ApiClientOptions(MexcV3ApiAddresses.Default.FuturesSocketClientAddress!);
        /// <summary>
        /// Usd futures streams options
        /// </summary>
        public ApiClientOptions FuturesStreamsOptions
        {
            get => _futuresStreamsOptions;
            set => _futuresStreamsOptions = new ApiClientOptions(_futuresStreamsOptions, value);
        }

        /// <summary>
        /// ctor
        /// </summary>
        public MexcV3SocketClientOptions() : this(Default)
        {
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="baseOn">Base the new options on other options</param>
        internal MexcV3SocketClientOptions(MexcV3SocketClientOptions baseOn) : base(baseOn)
        {
            if (baseOn == null)
                return;

            _spotStreamsOptions = new ApiClientOptions(baseOn.SpotStreamsOptions, null);
            _futuresStreamsOptions = new ApiClientOptions(baseOn.FuturesStreamsOptions, null);
        }
    }

    /// <summary>
    /// Mexc API client options
    /// </summary>
    public class MexcV3ApiClientOptions : RestApiClientOptions
    {
        /// <summary>
        /// A manual offset for the timestamp. Should only be used if AutoTimestamp and regular time synchronization on the OS is not reliable enough
        /// </summary>
        public TimeSpan TimestampOffset { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// Whether to check the trade rules when placing new orders and what to do if the trade isn't valid
        /// </summary>
        public TradeRulesBehaviour TradeRulesBehaviour { get; set; } = TradeRulesBehaviour.None;
        /// <summary>
        /// How often the trade rules should be updated. Only used when TradeRulesBehaviour is not None
        /// </summary>
        public TimeSpan TradeRulesUpdateInterval { get; set; } = TimeSpan.FromMinutes(60);

        /// <summary>
        /// ctor
        /// </summary>
        public MexcV3ApiClientOptions()
        {
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="baseAddress"></param>
        internal MexcV3ApiClientOptions(string baseAddress) : base(baseAddress)
        {
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="baseOn"></param>
        /// <param name="newValues"></param>
        internal MexcV3ApiClientOptions(MexcV3ApiClientOptions baseOn, MexcV3ApiClientOptions? newValues) : base(baseOn, newValues)
        {
            TimestampOffset = newValues?.TimestampOffset ?? baseOn.TimestampOffset;
            TradeRulesBehaviour = newValues?.TradeRulesBehaviour ?? baseOn.TradeRulesBehaviour;
            TradeRulesUpdateInterval = newValues?.TradeRulesUpdateInterval ?? baseOn.TradeRulesUpdateInterval;
        }
    }

    /// <summary>
    /// Mexc symbol order book options
    /// </summary>
    public class MexcV3OrderBookOptions : OrderBookOptions
    {
        /// <summary>
        /// The top amount of results to keep in sync. If for example limit=10 is used, the order book will contain the 10 best bids and 10 best asks. Leaving this null will sync the full order book
        /// </summary>
        public int? Limit { get; set; }

        /// <summary>
        /// Update interval in milliseconds, either 100 or 1000. Defaults to 1000
        /// </summary>
        public int? UpdateInterval { get; set; }

        /// <summary>
        /// After how much time we should consider the connection dropped if no data is received for this time after the initial subscriptions
        /// </summary>
        public TimeSpan? InitialDataTimeout { get; set; }

        /// <summary>
        /// The rest client to use for requesting the initial order book
        /// </summary>
        public IMexcV3RestClient? RestClient { get; set; }

        /// <summary>
        /// The client to use for the socket connection. When using the same client for multiple order books the connection can be shared.
        /// </summary>
        public IMexcV3SocketClient? SocketClient { get; set; }
    }
}
