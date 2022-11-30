using Mexc.Net.Objects;
using CryptoExchange.Net;
using CryptoExchange.Net.Objects;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Mexc.Net.Interfaces.Clients;
using Mexc.Net.Interfaces.Clients.SpotApi;
using Mexc.Net.Clients.SpotApi;
using Mexc.Net.Clients.GeneralApi;
using Mexc.Net.Interfaces.Clients.GeneralApi;
using Mexc.Net.Interfaces.Clients.Futures;

namespace Mexc.Net.Clients
{
    /// <inheritdoc cref="IMexcV3RestClient" />
    public class MexcV3RestClient : BaseRestClient, IMexcV3RestClient
    {
        #region Api clients

        /// <inheritdoc />
        public IMexcV3ClientGeneralApi GeneralApi { get; }

        /// <inheritdoc />
        public IMexcV3ClientSpotApi SpotApi { get; }

        /// <inheritdoc />
        public IMexcClientFutresApi FutresApi { get; }

        #endregion

        #region constructor/destructor
        /// <summary>
        /// Create a new instance of MexcClient using the default options
        /// 使用默认选项创建 MexcClient 的新实例
        /// </summary>
        public MexcV3RestClient() : this(MexcV3ClientOptions.Default)
        {
        }

        /// <summary>
        /// Create a new instance of MexcClient using provided options
        /// 使用提供的选项创建 MexcClient 的新实例
        /// </summary>
        /// <param name="options">The options to use for this client</param>
        public MexcV3RestClient(MexcV3ClientOptions options) : base("Mexc", options)
        {
            GeneralApi = AddApiClient(new MexcV3ClientGeneralApi(log, this, options));
            SpotApi = AddApiClient(new MexcV3ClientSpotApi(log, options));
        }
        #endregion

        /// <summary>
        /// Set the default options to be used when creating new clients
        /// 设置创建新客户端时要使用的默认选项
        /// </summary>
        /// <param name="options">Options to use as default</param>
        public static void SetDefaultOptions(MexcV3ClientOptions options)
        {
            MexcV3ClientOptions.Default = options;
        }
    }
}
