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
            SpotApi = AddApiClient(new MexcV3ClientSpotApi(log, this, options));
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

        /// <inheritdoc />
        protected override Error ParseErrorResponse(JToken error)
        {
            if (!error.HasValues)
                return new ServerError(error.ToString());

            if (error["msg"] == null && error["code"] == null)
                return new ServerError(error.ToString());

            if (error["msg"] != null && error["code"] == null)
                return new ServerError((string)error["msg"]!);

            return new ServerError((int)error["code"]!, (string)error["msg"]!);
        }
        
        internal Task<WebCallResult<T>> MexcV3SendRequestInternal<T>(RestApiClient apiClient, Uri uri, HttpMethod method, CancellationToken cancellationToken,
            Dictionary<string, object>? parameters = null, bool signed = false, HttpMethodParameterPosition? postPosition = null,
            ArrayParametersSerialization? arraySerialization = null, int weight = 1, bool ignoreRateLimit = false) where T : class
        {            
            Task<WebCallResult<T>>? response = base.MexcV3SendRequestAsync<T>(apiClient, uri, method, cancellationToken, parameters, signed, postPosition, arraySerialization, requestWeight: weight, ignoreRatelimit: ignoreRateLimit);
            return response;
        }

        internal Task<WebCallResult<T>> SendRequestInternal<T>(RestApiClient apiClient, Uri uri, HttpMethod method, CancellationToken cancellationToken,
            Dictionary<string, object>? parameters = null, bool signed = false, HttpMethodParameterPosition? postPosition = null,
            ArrayParametersSerialization? arraySerialization = null, int weight = 1, bool ignoreRateLimit = false) where T : class
        {
            return base.SendRequestAsync<T>(apiClient, uri, method, cancellationToken, parameters, signed, postPosition, arraySerialization, requestWeight: weight, ignoreRatelimit: ignoreRateLimit);
        }
    }
}
