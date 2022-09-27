using Mexc.Net.Clients;
using Mexc.Net.Interfaces.Clients;
using Mexc.Net.Objects;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Mexc.Net
{
    /// <summary>
    /// Helper methods for the Mexc API
    /// </summary>
    public static class MexcV3Helpers
    {
        /// <summary>
        /// Signs the given source with the given key using HMAC SHA256.
        /// 使用 HMAC SHA256 使用给定密钥对给定源进行签名。
        /// 仅做参考-官方API DEMO提供的方法
        /// </summary>
        public static string Sign(string source, string key)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            using (HMACSHA256 hmacsha256 = new HMACSHA256(keyBytes))
            {
                byte[] sourceBytes = Encoding.UTF8.GetBytes(source);

                byte[] hash = hmacsha256.ComputeHash(sourceBytes);

                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        /// <summary>
        /// Get the used weight from the response headers
        /// 从响应头中获取使用的权重
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static int? UsedWeight(this IEnumerable<KeyValuePair<string, IEnumerable<string>>>? headers)
        {
            if (headers == null)
                return null;

            var headerValues = headers.SingleOrDefault(s => s.Key.StartsWith("X-MBX-USED-WEIGHT-", StringComparison.InvariantCultureIgnoreCase)).Value;
            if (headerValues != null && int.TryParse(headerValues.First(), out var value))
                return value;
            return null;
        }

        /// <summary>
        /// Get the order count from the response headers
        /// 从响应标头中获取订单计数
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static int? UsedOrderCount(this IEnumerable<KeyValuePair<string, IEnumerable<string>>>? headers)
        {
            if (headers == null)
                return null;

            var headerValues = headers.SingleOrDefault(s => s.Key.StartsWith("X-MBX-ORDER-COUNT-", StringComparison.InvariantCultureIgnoreCase)).Value;
            if (headerValues != null && int.TryParse(headerValues.First(), out var value))
                return value;
            return null;
        }

        /// <summary>
        /// Clamp a quantity between a min and max quantity and floor to the closest step
        /// </summary>
        /// <param name="minQuantity"></param>
        /// <param name="maxQuantity"></param>
        /// <param name="stepSize"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public static decimal ClampQuantity(decimal minQuantity, decimal maxQuantity, decimal stepSize, decimal quantity)
        {
            quantity = Math.Min(maxQuantity, quantity);
            quantity = Math.Max(minQuantity, quantity);
            if (stepSize == 0)
                return quantity;
            quantity -= quantity % stepSize;
            quantity = Floor(quantity);
            return quantity;
        }

        /// <summary>
        /// Clamp a price between a min and max price
        /// </summary>
        /// <param name="minPrice"></param>
        /// <param name="maxPrice"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        public static decimal ClampPrice(decimal minPrice, decimal maxPrice, decimal price)
        {
            price = Math.Min(maxPrice, price);
            price = Math.Max(minPrice, price);
            return price;
        }

        /// <summary>
        /// Floor a price to the closest tick
        /// </summary>
        /// <param name="tickSize"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        public static decimal FloorPrice(decimal tickSize, decimal price)
        {
            price -= price % tickSize;
            price = Floor(price);
            return price;
        }

        /// <summary>
        /// Floor
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static decimal Floor(decimal number)
        {
            return Math.Floor(number * 100000000) / 100000000;
        }

        /// <summary>
        /// Add the IMexcClient and IMexcSocketClient to the sevice collection so they can be injected
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="defaultOptionsCallback">Set default options for the client</param>
        /// <param name="socketClientLifeTime">The lifetime of the IMexcSocketClient for the service collection. Defaults to Scoped.</param>
        /// <returns></returns>
        public static IServiceCollection AddMexc(
            this IServiceCollection services, 
            Action<MexcV3ClientOptions, MexcV3SocketClientOptions>? defaultOptionsCallback = null,
            ServiceLifetime? socketClientLifeTime = null)
        {
            if (defaultOptionsCallback != null)
            {
                var v3Options = new MexcV3ClientOptions();
                var socketV3Options = new MexcV3SocketClientOptions();
                defaultOptionsCallback?.Invoke(v3Options, socketV3Options);

                MexcV3RestClient.SetDefaultOptions(v3Options);
                MexcV3SocketClient.SetDefaultOptions(socketV3Options);
            }

            services.AddTransient<IMexcV3RestClient, MexcV3RestClient>();
            if (socketClientLifeTime == null)
                services.AddScoped<IMexcV3SocketClient, MexcV3SocketClient>();
            else
                services.Add(new ServiceDescriptor(typeof(IMexcV3SocketClient), typeof(MexcV3SocketClient), socketClientLifeTime.Value));
            return services;
        }

        /// <summary>
        /// Validate the string is a valid Mexc symbol.
        /// </summary>
        /// <param name="symbolString">string to validate</param>
        public static void ValidateMexcSymbol(this string symbolString)
        {
            if (string.IsNullOrEmpty(symbolString))
                throw new ArgumentException("Symbol is not provided");

            if(!Regex.IsMatch(symbolString, "^([A-Z|a-z|0-9|]{5,})$"))
                throw new ArgumentException($"{symbolString} is not a valid Mexc symbol. Should be [BaseAsset][QuoteAsset], e.g. BTCUSDT");
        }
    }
}
