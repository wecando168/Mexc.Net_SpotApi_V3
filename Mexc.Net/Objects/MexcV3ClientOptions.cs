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
    /// MexcV3 client options
    /// </summary>
    public class MexcV3ClientOptions : ClientOptions
    {
        /// <summary>
        /// Whether to check the trade rules when placing new orders and what to do if the trade isn't valid
        /// </summary>
        public TradeRulesBehaviour TradeRulesBehaviour { get; set; } = TradeRulesBehaviour.None;

        /// <summary>
        /// How often the trade rules should be updated. Only used when TradeRulesBehaviour is not None
        /// 交易规则应多久更新一次（仅在 TradeRulesBehaviour != None 时使用)
        /// </summary>
        public TimeSpan TradeRulesUpdateInterval { get; set; } = TimeSpan.FromMinutes(60);

        /// <summary>
        /// Default options for the client
        /// </summary>
        public static MexcV3ClientOptions Default { get; set; } = new MexcV3ClientOptions();

        /// <summary>
        /// Whether public requests should be signed if ApiCredentials are provided. Needed for accurate rate limiting.
        /// </summary>
        public bool SignPublicRequests { get; set; } = false;

        /// <summary>
        /// The default receive window for requests
        /// </summary>
        public TimeSpan ReceiveWindow { get; set; } = TimeSpan.FromSeconds(5);

        /// <summary>
        /// 现货Api设置
        /// </summary>
        private RestApiClientOptions _spotApiOptions = new RestApiClientOptions(MexcV3ApiAddresses.Default.SpotRestClientAddress)
        {
            RateLimiters = new List<IRateLimiter>
            {
                new RateLimiter()
                .AddPartialEndpointLimit("/api/", 1200, TimeSpan.FromMinutes(1))
                .AddPartialEndpointLimit("/sapi/", 12000, TimeSpan.FromMinutes(1))
                .AddEndpointLimit("/api/v3/order", 50, TimeSpan.FromSeconds(10), HttpMethod.Post, true)
            }
        };

        /// <summary>
        /// 合约Api设置
        /// </summary>
        private RestApiClientOptions _futuresApiOptions = new RestApiClientOptions(MexcV3ApiAddresses.Default.FuturesRestClientAddress)
        {
        };

        /// <summary>
        /// Spot API options
        /// </summary>
        public RestApiClientOptions SpotApiOptions
        {
            get => _spotApiOptions;
            set => _spotApiOptions = new RestApiClientOptions(_spotApiOptions, value);
        }

        /// <summary>
        /// Usd futures API options
        /// </summary>
        public RestApiClientOptions FuturesApiOptions
        {
            get => _futuresApiOptions;
            set => _futuresApiOptions = new RestApiClientOptions(_futuresApiOptions, value);
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

            SignPublicRequests = baseOn.SignPublicRequests;
            _spotApiOptions = new RestApiClientOptions(baseOn.SpotApiOptions, null);
            _futuresApiOptions = new RestApiClientOptions(baseOn.FuturesApiOptions, null);
        }
    }

    /// <summary>
    /// MexcV3 socket client options
    /// </summary>
    public class MexcV3SocketClientOptions : ClientOptions
    {
        /// <summary>
        /// Default options for the spot client
        /// </summary>
        public static MexcV3SocketClientOptions Default { get; set; } = new MexcV3SocketClientOptions();


        private MexcV3SocketApiClientOptions _spotStreamsOptions = new MexcV3SocketApiClientOptions
            (MexcV3ApiAddresses.Default.SpotPublicSocketClientAddress, MexcV3ApiAddresses.Default.SpotPrivaeSocketClientAddress)
        {
            SocketSubscriptionsCombineTarget = 10
        };

        /// <summary>
        /// Spot public streams options
        /// </summary>
        public MexcV3SocketApiClientOptions SpotStreamsOptions
        {
            get => _spotStreamsOptions;
            set => _spotStreamsOptions = new MexcV3SocketApiClientOptions(_spotStreamsOptions, value);
        }        

        private MexcV3SocketApiClientOptions _futuresStreamsOptions = new MexcV3SocketApiClientOptions
            (MexcV3ApiAddresses.Default.FuturesPublicSocketClientAddress!, MexcV3ApiAddresses.Default.FuturesPrivateSocketClientAddress!)
        {
            SocketSubscriptionsCombineTarget = 10
        };

        /// <summary>
        /// Usd futures streams options
        /// </summary>
        public MexcV3SocketApiClientOptions FuturesStreamsOptions
        {
            get => _futuresStreamsOptions;
            set => _futuresStreamsOptions = new MexcV3SocketApiClientOptions(_futuresStreamsOptions, value);
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

            _spotStreamsOptions = new MexcV3SocketApiClientOptions(baseOn.SpotStreamsOptions, null);
            _futuresStreamsOptions = new MexcV3SocketApiClientOptions(baseOn.FuturesStreamsOptions, null);
        }
    }

    /// <summary>
    /// MexcV3 api client options
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
        /// 交易规则应多久更新一次（仅在 TradeRulesBehaviour != None 时使用)
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
    /// MexcV3 socket api client options
    /// </summary>
    public class MexcV3SocketApiClientOptions : SocketApiClientOptions
    {
        /// <summary>
        /// The base address for the authenticated websocket
        /// </summary>
        public string BaseAddressAuthenticated { get; set; }

        /// <summary>
        /// The base address for the market by price websocket
        /// </summary>
        public string BaseAddressInrementalOrderBook { get; set; }

        /// <summary>
        /// ctor
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public MexcV3SocketApiClientOptions()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="baseAddress"></param>
        /// <param name="baseAddressAuthenticated"></param>
        internal MexcV3SocketApiClientOptions(string baseAddress, string baseAddressAuthenticated) : base(baseAddress)
        {
            BaseAddressAuthenticated = baseAddressAuthenticated;
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="baseAddress"></param>
        /// <param name="baseAddressAuthenticated"></param>
        /// <param name="baseAddressIncrementalOrderBook"></param>
        internal MexcV3SocketApiClientOptions(string baseAddress, string baseAddressAuthenticated, string baseAddressIncrementalOrderBook) : base(baseAddress)
        {
            BaseAddressAuthenticated = baseAddressAuthenticated;
            BaseAddressInrementalOrderBook = baseAddressIncrementalOrderBook;
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="baseOn"></param>
        /// <param name="newValues"></param>
        internal MexcV3SocketApiClientOptions(MexcV3SocketApiClientOptions baseOn, MexcV3SocketApiClientOptions? newValues) : base(baseOn, newValues)
        {
            BaseAddressAuthenticated = newValues?.BaseAddressAuthenticated ?? baseOn.BaseAddressAuthenticated;
            BaseAddressInrementalOrderBook = newValues?.BaseAddressInrementalOrderBook ?? baseOn.BaseAddressInrementalOrderBook;
        }
    }

    /// <summary>
    /// Socket API client options
    /// </summary>
    public class MexcV3SocketFuturesApiClientOptions : SocketApiClientOptions
    {
        /// <summary>
        /// The base address for the authenticated websocket
        /// </summary>
        public string BaseAddressAuthenticated { get; set; }

        /// <summary>
        /// The base address for the index api
        /// </summary>
        public string BaseAddressIndex { get; set; }

        /// <summary>
        /// ctor
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public MexcV3SocketFuturesApiClientOptions()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="baseAddress"></param>
        /// <param name="baseAddressAuthenticated"></param>
        /// <param name="baseAddressIndex"></param>
        internal MexcV3SocketFuturesApiClientOptions(string baseAddress, string baseAddressAuthenticated, string baseAddressIndex) : base(baseAddress)
        {
            BaseAddressAuthenticated = baseAddressAuthenticated;
            BaseAddressIndex = baseAddressIndex;
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="baseOn"></param>
        /// <param name="newValues"></param>
        internal MexcV3SocketFuturesApiClientOptions(MexcV3SocketFuturesApiClientOptions baseOn, MexcV3SocketFuturesApiClientOptions? newValues) : base(baseOn, newValues)
        {
            BaseAddressAuthenticated = newValues?.BaseAddressAuthenticated ?? baseOn.BaseAddressAuthenticated;
            BaseAddressIndex = newValues?.BaseAddressIndex ?? baseOn.BaseAddressIndex;
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
