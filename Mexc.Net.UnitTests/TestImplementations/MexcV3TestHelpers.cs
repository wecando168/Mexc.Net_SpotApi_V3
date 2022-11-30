using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mexc.Net.Clients;
using Mexc.Net.Interfaces.Clients;
using Mexc.Net.Objects;
using CryptoExchange.Net;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Logging;
using CryptoExchange.Net.Sockets;
using Moq;
using Newtonsoft.Json;

namespace Mexc.Net.UnitTests.TestImplementations
{
    public class MexcV3TestHelpers
    {
        [ExcludeFromCodeCoverage]
        public static bool AreEqual<T>(T self, T to, params string[] ignore) where T : class
        {
            if (self != null && to != null)
            {
                Type type = self.GetType();
                List<string> ignoreList = new List<string>(ignore);
                foreach (PropertyInfo pi in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (ignoreList.Contains(pi.Name))
                    {
                        continue;
                    }

                    object selfValue = type.GetProperty(pi.Name).GetValue(self, null);
                    object toValue = type.GetProperty(pi.Name).GetValue(to, null);

                    if (pi.PropertyType.IsClass && !pi.PropertyType.Module.ScopeName.Equals("System.Private.CoreLib.dll"))
                    {
                        // Check of "CommonLanguageRuntimeLibrary" is needed because string is also a class
                        if (AreEqual(selfValue, toValue, ignore))
                        {
                            continue;
                        }

                        return false;
                    }

                    if (selfValue != toValue && (selfValue == null || !selfValue.Equals(toValue)))
                    {
                        return false;
                    }
                }

                return true;
            }

            return self == to;
        }

        public static IMexcV3RestClient CreateClient(MexcV3ClientOptions options = null)
        {
            IMexcV3RestClient client;
            client = options != null ? new MexcV3RestClient(options) : new MexcV3RestClient();
            client.SpotApi.RequestFactory = Mock.Of<IRequestFactory>();
            return client;
        }

        public static IMexcV3RestClient CreateResponseClient(string response, MexcV3ClientOptions options = null)
        {
            MexcV3RestClient client = (MexcV3RestClient)CreateClient(options);
            SetResponse(client, response);
            return client;
        }

        public static IMexcV3RestClient CreateResponseClient<T>(T response, MexcV3ClientOptions options = null)
        {
            MexcV3RestClient client = (MexcV3RestClient)CreateClient(options);
            SetResponse(client, JsonConvert.SerializeObject(response));
            return client;
        }

        public static IMexcV3SocketClient CreateSocketClient(IWebsocket socket, MexcV3SocketClientOptions options = null)
        {
            IMexcV3SocketClient client;
            client = options != null ? new MexcV3SocketClient(options) : new MexcV3SocketClient();
            client.SpotPublicStreams.SocketFactory = Mock.Of<IWebsocketFactory>();
            Mock.Get(client.SpotPublicStreams.SocketFactory).Setup(f => f.CreateWebsocket(It.IsAny<Log>(), It.IsAny<WebSocketParameters>())).Returns(socket);
            return client;
        }

        public static void SetResponse(IMexcV3RestClient client, string responseData)
        {
            byte[] expectedBytes = Encoding.UTF8.GetBytes(responseData);
            MemoryStream responseStream = new MemoryStream();
            responseStream.Write(expectedBytes, 0, expectedBytes.Length);
            responseStream.Seek(0, SeekOrigin.Begin);

            Mock<IResponse> response = new Mock<IResponse>();
            response.Setup(c => c.IsSuccessStatusCode).Returns(true);
            response.Setup(c => c.GetResponseStreamAsync()).Returns(Task.FromResult((Stream)responseStream));

            Mock<IRequest> request = new Mock<IRequest>();
            request.Setup(c => c.Uri).Returns(new Uri("http://www.test.com"));
            request.Setup(c => c.GetHeaders()).Returns(new Dictionary<string, IEnumerable<string>>());
            request.Setup(c => c.GetResponseAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(response.Object));

            Mock<IRequestFactory> factory = Mock.Get(client.SpotApi.RequestFactory);
            factory.Setup(c => c.Create(It.IsAny<HttpMethod>(), It.IsAny<Uri>(), It.IsAny<int>()))
                .Returns(request.Object);
        }

        public static void SetErrorWithResponse(IMexcV3RestClient client, string responseData, HttpStatusCode code)
        {
            byte[] expectedBytes = Encoding.UTF8.GetBytes(responseData);
            MemoryStream responseStream = new MemoryStream();
            responseStream.Write(expectedBytes, 0, expectedBytes.Length);
            responseStream.Seek(0, SeekOrigin.Begin);

            Mock<IResponse> response = new Mock<IResponse>();
            response.Setup(c => c.IsSuccessStatusCode).Returns(false);
            response.Setup(c => c.GetResponseStreamAsync()).Returns(Task.FromResult((Stream)responseStream));

            Mock<IRequest> request = new Mock<IRequest>();
            request.Setup(c => c.Uri).Returns(new Uri("http://www.test.com"));
            request.Setup(c => c.GetHeaders()).Returns(new Dictionary<string, IEnumerable<string>>());
            request.Setup(c => c.GetResponseAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(response.Object));

            Mock<IRequestFactory> factory = Mock.Get(client.SpotApi.RequestFactory);
            factory.Setup(c => c.Create(It.IsAny<HttpMethod>(), It.IsAny<Uri>(), It.IsAny<int>()))
                .Returns(request.Object);
        }

        public static object GetTestValue(Type type, int i)
        {
            if (type == typeof(bool))
                return true;

            if (type == typeof(bool?))
                return (bool?)true;

            if (type == typeof(decimal))
                return i / 100m;

            if (type == typeof(decimal?))
                return (decimal?)(i / 100m);

            if (type == typeof(int))
                return i;

            if (type == typeof(int?))
                return (int?)i;

            if (type == typeof(long))
                return (long)i;

            if (type == typeof(long?))
                return (long?)i;

            if (type == typeof(DateTime))
                return new DateTime(2019, 1, Math.Max(i, 1));

            if (type == typeof(DateTime?))
                return (DateTime?)new DateTime(2019, 1, Math.Max(i, 1));

            if (type == typeof(string))
                return "string" + i;

            if (type == typeof(IEnumerable<string>))
                return new[] { "string" + i };

            if (type.IsEnum)
            {
                return Activator.CreateInstance(type);
            }

            if (type.IsArray)
            {
                Type elementType = type.GetElementType()!;
                Array result = Array.CreateInstance(elementType, 2);
                result.SetValue(GetTestValue(elementType, 0), 0);
                result.SetValue(GetTestValue(elementType, 1), 1);
                return result;
            }

            if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(List<>)))
            {
                IList result = (IList)Activator.CreateInstance(type)!;
                result.Add(GetTestValue(type.GetGenericArguments()[0], 0));
                result.Add(GetTestValue(type.GetGenericArguments()[0], 1));
                return result;
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                IDictionary result = (IDictionary)Activator.CreateInstance(type)!;
                result.Add(GetTestValue(type.GetGenericArguments()[0], 0)!, GetTestValue(type.GetGenericArguments()[1], 0));
                result.Add(GetTestValue(type.GetGenericArguments()[0], 1)!, GetTestValue(type.GetGenericArguments()[1], 1));
                return Convert.ChangeType(result, type);
            }

            return null;
        }

        public static async Task<object> InvokeAsync(MethodInfo @this, object obj, params object[] parameters)
        {
            Task task = (Task)@this.Invoke(obj, parameters);
            await task.ConfigureAwait(false);
            PropertyInfo resultProperty = task.GetType().GetProperty("Result");
            return resultProperty.GetValue(task);
        }
    }
}
