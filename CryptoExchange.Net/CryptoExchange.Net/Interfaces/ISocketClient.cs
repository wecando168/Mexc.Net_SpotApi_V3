﻿using System;
using System.Threading.Tasks;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;

namespace CryptoExchange.Net.Interfaces
{
    /// <summary>
    /// Base class for socket API implementations
    /// </summary>
    public interface ISocketClient: IDisposable
    {
        /// <summary>
        /// The options provided for this client
        /// </summary>
        BaseSocketClientOptions ClientOptions { get; }

        /// <summary>
        /// Set the API credentials for this client. All Api clients in this client will use the new credentials, regardless of earlier set options.
        /// </summary>
        /// <param name="credentials">The credentials to set</param>
        void SetApiCredentials(ApiCredentials credentials);

        /// <summary>
        /// Incoming kilobytes per second of data
        /// </summary>
        public double IncomingKbps { get; }

        /// <summary>
        /// The current amount of connections to the API from this client. A connection can have multiple subscriptions.
        /// </summary>
        public int CurrentConnections { get; }
        
        /// <summary>
        /// The current amount of subscriptions running from the client
        /// </summary>
        public int CurrentSubscriptions { get; }

        /// <summary>
        /// Unsubscribe from a stream using the subscription id received when starting the subscription
        /// </summary>
        /// <param name="subscriptionId">The id of the subscription to unsubscribe</param>
        /// <returns></returns>
        Task UnsubscribeAsync(int subscriptionId);

        /// <summary>
        /// Unsubscribe from a stream
        /// </summary>
        /// <param name="subscription">The subscription to unsubscribe</param>
        /// <returns></returns>
        Task UnsubscribeAsync(UpdateSubscription subscription);

        /// <summary>
        /// Unsubscribe all subscriptions
        /// </summary>
        /// <returns></returns>
        Task UnsubscribeAllAsync();
    }
}