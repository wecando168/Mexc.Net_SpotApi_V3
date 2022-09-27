using CryptoExchange.Net.Interfaces.CommonClients;
using System;

namespace Mexc.Net.Interfaces.Clients.SpotApi
{
    /// <summary>
    /// Mexc Spot API endpoints
    /// </summary>
    public interface IMexcV3ClientSpotApi : IDisposable
    {
        /// <summary>
        /// Market Data Endpoints
        /// 行情接口
        /// </summary>
        public IMexcV3ClientSpotApiMarketData MarketData { get; }

        /// <summary>
        /// Sub Account Endpoints
        /// 母子账户接口
        /// </summary>
        public IMexcV3ClientSpotApiSubAccount SubAccount { get; }

        /// <summary>
        /// Spot Account/Trade Endpoints
        /// 现货账户和交易接口
        /// </summary>
        public IMexcV3ClientSpotApiSpotAccountTrade SpotAccountTrade { get; }

        /// <summary>
        /// Wallet Endpoints
        /// 钱包接口
        /// </summary>
        public IMexcV3ClientSpotApiWallet Wallet { get; }

        /// <summary>
        /// Exchange Traded Funds Endpoints(ETF Endpoints)
        /// ETF接口
        /// </summary>
        public IMexcV3ClientSpotApiETF ExchangeTradedFunds { get; }

        /// <summary>
        /// Websocket 账户接口
        /// </summary>
        public IMexcV3ClientSpotApiWebsocketAccount WebsocketAccount { get; }

        /// <summary>
        /// Margin Account and Trading Interface Endpoints
        /// 杠杆账户和交易接口
        /// </summary>
        public IMexcV3ClientSpotApiMarginAccountTrade MarginAccountTrade { get; }

        /// <summary>
        /// Endpoints related to orders and trades
        /// 与订单和交易相关的端点
        /// </summary>
        public IMexcV3ClientSpotApiTrading Trading { get; }

        /// <summary>
        /// Get the ISpotClient for this client. This is a common interface which allows for some basic operations without knowing any details of the exchange.
        /// 获取此客户端的 ISpotClient。 这是一个通用接口，允许在不知道任何交换细节的情况下进行一些基本操作。
        /// </summary>
        /// <returns></returns>
        public ISpotClient CommonSpotClient { get; }
    }
}
