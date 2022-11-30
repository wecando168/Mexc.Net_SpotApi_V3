using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Logging;
using CryptoExchange.Net.Objects;
using Mexc.Net.Objects.Internal;
using Microsoft.Extensions.Logging;

namespace Mexc.Net
{
    /// <summary>
    /// Mexc Authentication Provider
    /// </summary>
    public class MexcV3AuthenticationProvider : AuthenticationProvider
    {
        private readonly bool signPublicRequests;
        private Log _log = new Log("MexcV3AuthenticationProvider");

        /// <summary>
        /// 抹茶请求增加签名验证的方法
        /// </summary>
        /// <param name="credentials">证书</param>
        /// <param name="signPublicRequests">公用数据是否需要签名</param>
        public MexcV3AuthenticationProvider(ApiCredentials credentials, bool signPublicRequests = false) : base(credentials)
        {
            this.signPublicRequests = signPublicRequests;
        }

        /// <summary>
        /// 重载请求增加签名验证的方法（底层实现都是使用自动字典排序的参数，但是抹茶不能这样，这个重载的方法保留，但是暂时不用）
        /// </summary>
        /// <param name="apiClient">API客户端</param>
        /// <param name="uri">请求链接</param>
        /// <param name="method">请求方法</param>
        /// <param name="providedParameters">提供的参数</param>
        /// <param name="auth">是否需要授权</param>
        /// <param name="arraySerialization">数组序列化</param>
        /// <param name="parameterPosition">参数位置</param>
        /// <param name="uriParameters">用于存放请求链接中的参数</param>
        /// <param name="bodyParameters">用于存放请求体中的参数</param>
        /// <param name="headers">用于存放请求头中的参数</param>
        public override void AuthenticateRequest(
            RestApiClient apiClient,
            Uri uri,
            HttpMethod method,
            Dictionary<string, object> providedParameters,
            bool auth,
            ArrayParametersSerialization arraySerialization,
            HttpMethodParameterPosition parameterPosition,
            out SortedDictionary<string, object> uriParameters, 
            out SortedDictionary<string, object> bodyParameters,
            out Dictionary<string, string> headers)
        {
            //1、参数位置如果是InUri，则提取参数存入uriParameters/参数位置如果是InBody，则提取参数存入bodyParameters
            uriParameters = (parameterPosition == HttpMethodParameterPosition.InUri) ? new SortedDictionary<string, object>(providedParameters) : new SortedDictionary<string, object>();
            bodyParameters = (parameterPosition == HttpMethodParameterPosition.InBody) ? new SortedDictionary<string, object>(providedParameters) : new SortedDictionary<string, object>();

            //2、抹茶请求Header中添加签名相关参数：x-mexc-apikey	API key中的access key            
            headers = new Dictionary<string, string>() { { "x-mexc-apikey", Credentials.Key!.GetString() } };

            //3、抹茶请求体类型设置：Content-Type	application/json（底层CryptoExchange.Net进行了设置）

            if (!auth)
                return;

            //4、请求所需要的所有参数存入parameters参数列表
            SortedDictionary<string, object>? parameters = (parameterPosition == HttpMethodParameterPosition.InUri) ? uriParameters : bodyParameters;

            //5、parameters参数列表加入时间戳参数
            //签名接口均需要传递timestamp参数，其值应当是请求发送时刻的unix时间戳(毫秒)。
            //服务器收到请求时会判断请求中的时间戳，如果是5000毫秒之前发出的，则请求会被认为无效。这个时间空窗值可以通过发送可选参数recvWindow来定义。
            string? timestamp = GetMillisecondTimestamp(apiClient);
            parameters.Add("timestamp", timestamp);

            //6、使用提供的参数创建一个新的 uri 作为查询
            uri = uri.SetParameters(uriParameters, arraySerialization);

            //7、对请求链接中的参数进行签名
            //签名使用HMAC SHA256算法.
            //API-KEY所对应的API-Secret作为 HMAC SHA256 的密钥
            //其他所有参数作为HMAC SHA256的操作对象，得到的输出即为签名。
            //7.1 提取签名字符串
            string? prepareSignData = (parameterPosition == HttpMethodParameterPosition.InUri) ? uri.Query.Replace("?", "") : parameters.ToFormData();
            _log.Write(LogLevel.Debug, $"Prepare sign data:\r\n{prepareSignData}");
            //7.2 签名操作
            string? signHMACSHA256 = SignHMACSHA256(prepareSignData);
            _log.Write(LogLevel.Debug, $"Sign:\r\n{prepareSignData}");

            //8、签名转小写
            signHMACSHA256 = signHMACSHA256.ToLower();

            //调用SIGNED 接口时，除了接口本身所需的参数外，还需要在query string 或 request body中传递 signature, 即签名参数（在批量操作的API中，若参数值中有逗号等特殊符号，这些符号在签名时需要做URL encode）。
            //9、参数增加签名
            parameters.Add("signature", signHMACSHA256);
        }

        /// <summary>
        /// 重载请求增加签名验证的方法（参数不做字典排序）
        /// </summary>
        /// <param name="apiClient">API客户端</param>
        /// <param name="uri">请求链接</param>
        /// <param name="method">请求方法</param>
        /// <param name="providedParameters">提供的参数</param>
        /// <param name="auth">是否需要授权</param>
        /// <param name="arraySerialization">数组序列化</param>
        /// <param name="parameterPosition">参数位置</param>
        /// <param name="uriParameters">用于存放请求链接中的参数</param>
        /// <param name="bodyParameters">用于存放请求体中的参数</param>
        /// <param name="headers">用于存放请求头中的参数</param>
        public override void MexcV3AuthenticateRequest(RestApiClient apiClient,
            Uri uri,
            HttpMethod method,
            Dictionary<string, object> providedParameters,
            bool auth,
            ArrayParametersSerialization arraySerialization,
            HttpMethodParameterPosition parameterPosition,
            out Dictionary<string, object> uriParameters,
            out Dictionary<string, object> bodyParameters,
            out Dictionary<string, string> headers)
        {
            //1、参数位置如果是InUri，则提取参数存入uriParameters/参数位置如果是InBody，则提取参数存入bodyParameters
            uriParameters = (parameterPosition == HttpMethodParameterPosition.InUri) ? new Dictionary<string, object>(providedParameters) : new Dictionary<string, object>();
            bodyParameters = (parameterPosition == HttpMethodParameterPosition.InBody) ? new Dictionary<string, object>(providedParameters) : new Dictionary<string, object>();

            //2、抹茶请求Header中添加签名相关参数：x-mexc-apikey	API key中的access key            
            headers = new Dictionary<string, string>() { { "x-mexc-apikey", Credentials.Key!.GetString() } };

            //3、抹茶请求体类型设置：Content-Type	application/json（底层CryptoExchange.Net进行了设置）

            if (!auth)
                return;

            //4、请求所需要的所有参数存入parameters参数列表
            Dictionary<string, object>? parameters = (parameterPosition == HttpMethodParameterPosition.InUri) ? uriParameters : bodyParameters;

            //5、parameters参数列表加入时间戳参数
            //签名接口均需要传递timestamp参数，其值应当是请求发送时刻的unix时间戳(毫秒)。
            //服务器收到请求时会判断请求中的时间戳，如果是5000毫秒之前发出的，则请求会被认为无效。这个时间空窗值可以通过发送可选参数recvWindow来定义。
            string? timestamp = GetMillisecondTimestamp(apiClient);
            parameters.Add("timestamp", timestamp);

            //6、使用提供的参数创建一个新的 uri 作为查询
            //var json = JsonConvert.SerializeObject(uriParameters);
            //var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            uri = uri.MexcV3SetParameters(uriParameters, arraySerialization);

            //7、提取签名字符串
            string? prepareSignData = (parameterPosition == HttpMethodParameterPosition.InUri) ? uri.Query.Replace("?", "") : parameters.ToFormData();

            //8、签名字符串中的转义字符改为大写（抹茶用小写出来的签名是不对的）
            prepareSignData = prepareSignData.Replace("%5b", "%5B");    //[
            prepareSignData = prepareSignData.Replace("%7b", "%7B");    //{
            prepareSignData = prepareSignData.Replace("%22", "%22");    //"
            prepareSignData = prepareSignData.Replace("%3a", "%3A");    //:
            prepareSignData = prepareSignData.Replace("%2c", "%2C");    //,
            prepareSignData = prepareSignData.Replace("%7d", "%7D");    //}
            prepareSignData = prepareSignData.Replace("%5d", "%5D");    //]
            _log.Write(LogLevel.Debug, $"Prepare sign data:\r\n{prepareSignData}");

            //9、签名操作
            //签名使用HMAC SHA256算法.
            //API-KEY所对应的API-Secret作为 HMAC SHA256 的密钥
            //其他所有参数作为HMAC SHA256的操作对象，得到的输出即为签名。
            string? signHMACSHA256 = SignHMACSHA256(prepareSignData);
            _log.Write(LogLevel.Debug, $"Sign:\r\n{prepareSignData}");

            //10、签名转小写
            signHMACSHA256 = signHMACSHA256.ToLower();

            //调用SIGNED 接口时，除了接口本身所需的参数外，还需要在query string 或 request body中传递 signature, 即签名参数（在批量操作的API中，若参数值中有逗号等特殊符号，这些符号在签名时需要做URL encode）。
            //11、参数增加签名
            parameters.Add("signature", signHMACSHA256);
        }

        /// <summary>
        /// 请求增加签名验证的方法（与上面重载得方法是一样得，尝试不使用重载如果可以，删除重载得签名方法）
        /// </summary>
        /// <param name="apiClient">API客户端</param>
        /// <param name="uri">请求链接</param>
        /// <param name="method">请求方法</param>
        /// <param name="providedParameters">提供的参数</param>
        /// <param name="auth">是否需要授权</param>
        /// <param name="arraySerialization">数组序列化</param>
        /// <param name="parameterPosition">参数位置</param>
        /// <param name="uriParameters">用于存放请求链接中的参数</param>
        /// <param name="bodyParameters">用于存放请求体中的参数</param>
        /// <param name="headers">用于存放请求头中的参数</param>
        public void MexcV3AuthenticateRequestTest(RestApiClient apiClient,
            Uri uri,
            HttpMethod method,
            Dictionary<string, object> providedParameters,
            bool auth,
            ArrayParametersSerialization arraySerialization,
            HttpMethodParameterPosition parameterPosition,
            out Dictionary<string, object> uriParameters,
            out Dictionary<string, object> bodyParameters,
            out Dictionary<string, string> headers)
        {
            //1、参数位置如果是InUri，则提取参数存入uriParameters/参数位置如果是InBody，则提取参数存入bodyParameters
            uriParameters = (parameterPosition == HttpMethodParameterPosition.InUri) ? new Dictionary<string, object>(providedParameters) : new Dictionary<string, object>();
            bodyParameters = (parameterPosition == HttpMethodParameterPosition.InBody) ? new Dictionary<string, object>(providedParameters) : new Dictionary<string, object>();

            //2、抹茶请求Header中添加签名相关参数：x-mexc-apikey	API key中的access key            
            headers = new Dictionary<string, string>() { { "x-mexc-apikey", Credentials.Key!.GetString() } };

            //3、抹茶请求体类型设置：Content-Type	application/json（底层CryptoExchange.Net进行了设置）

            if (!auth)
                return;

            //4、请求所需要的所有参数存入parameters参数列表
            Dictionary<string, object>? parameters = (parameterPosition == HttpMethodParameterPosition.InUri) ? uriParameters : bodyParameters;

            //5、parameters参数列表加入时间戳参数
            //签名接口均需要传递timestamp参数，其值应当是请求发送时刻的unix时间戳(毫秒)。
            //服务器收到请求时会判断请求中的时间戳，如果是5000毫秒之前发出的，则请求会被认为无效。这个时间空窗值可以通过发送可选参数recvWindow来定义。
            string? timestamp = GetMillisecondTimestamp(apiClient);
            parameters.Add("timestamp", timestamp);

            //6、使用提供的参数创建一个新的 uri 作为查询
            //var json = JsonConvert.SerializeObject(uriParameters);
            //var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            uri = uri.MexcV3SetParameters(uriParameters, arraySerialization);

            //7、提取签名字符串
            string? prepareSignData = (parameterPosition == HttpMethodParameterPosition.InUri) ? uri.Query.Replace("?", "") : parameters.ToFormData();

            //8、签名字符串中的转义字符改为大写（抹茶用小写出来的签名是不对的）
            prepareSignData = prepareSignData.Replace("%5b", "%5B");    //[
            prepareSignData = prepareSignData.Replace("%7b", "%7B");    //{
            prepareSignData = prepareSignData.Replace("%22", "%22");    //"
            prepareSignData = prepareSignData.Replace("%3a", "%3A");    //:
            prepareSignData = prepareSignData.Replace("%2c", "%2C");    //,
            prepareSignData = prepareSignData.Replace("%7d", "%7D");    //}
            prepareSignData = prepareSignData.Replace("%5d", "%5D");    //]
            _log.Write(LogLevel.Debug, $"Prepare sign data:\r\n{prepareSignData}");

            //9、签名操作
            //签名使用HMAC SHA256算法.
            //API-KEY所对应的API-Secret作为 HMAC SHA256 的密钥
            //其他所有参数作为HMAC SHA256的操作对象，得到的输出即为签名。
            string? signHMACSHA256 = SignHMACSHA256(prepareSignData);
            _log.Write(LogLevel.Debug, $"Sign:\r\n{prepareSignData}");

            //10、签名转小写
            signHMACSHA256 = signHMACSHA256.ToLower();

            //调用SIGNED 接口时，除了接口本身所需的参数外，还需要在query string 或 request body中传递 signature, 即签名参数（在批量操作的API中，若参数值中有逗号等特殊符号，这些符号在签名时需要做URL encode）。
            //11、参数增加签名
            parameters.Add("signature", signHMACSHA256);
        }

        internal MexcV3AuthenticationRequest GetWebsocketAuthentication(Uri uri)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("accessKey", Credentials.Key!.GetString());
            parameters.Add("signatureMethod", "HmacSHA256");
            parameters.Add("signatureVersion", 2.1);
            parameters.Add("timestamp", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture));

            var sortedParameters = parameters.OrderBy(kv => Encoding.UTF8.GetBytes(WebUtility.UrlEncode(kv.Key)!), new ByteOrderComparer());
            var paramString = uri.SetParameters(sortedParameters, ArrayParametersSerialization.Array).Query.Replace("?", "");
            paramString = new Regex(@"%[a-f0-9]{2}").Replace(paramString, m => m.Value.ToUpperInvariant()).Replace("%2C", ".");
            var signData = $"GET\n{uri.Host}\n{uri.AbsolutePath}\n{paramString}";
            var signature = SignHMACSHA256(signData, SignOutputType.Base64);

            return new MexcV3AuthenticationRequest(Credentials.Key!.GetString(), (string)parameters["timestamp"], signature);
        }

        internal MexcV3AuthenticationRequest2 GetWebsocketAuthentication2(Uri uri)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("AccessKeyId", Credentials.Key!.GetString());
            parameters.Add("SignatureMethod", "HmacSHA256");
            parameters.Add("SignatureVersion", 2);
            parameters.Add("Timestamp", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture));

            var sortedParameters = parameters.OrderBy(kv => Encoding.UTF8.GetBytes(WebUtility.UrlEncode(kv.Key)!), new ByteOrderComparer());
            var paramString = uri.SetParameters(sortedParameters, ArrayParametersSerialization.Array).Query.Replace("?", "");
            paramString = new Regex(@"%[a-f0-9]{2}").Replace(paramString, m => m.Value.ToUpperInvariant()).Replace("%2C", ".");
            var signData = $"GET\n{uri.Host}\n{uri.AbsolutePath}\n{paramString}";
            var signature = SignHMACSHA256(signData, SignOutputType.Base64);

            return new MexcV3AuthenticationRequest2(Credentials.Key!.GetString(), (string)parameters["Timestamp"], signature);
        }
    }
}
