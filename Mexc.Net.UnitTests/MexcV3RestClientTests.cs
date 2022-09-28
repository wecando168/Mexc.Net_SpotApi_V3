using Mexc.Net.Objects;
using Mexc.Net.UnitTests.TestImplementations;
using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Requests;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using CryptoExchange.Net.Objects;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;
using CryptoExchange.Net.Sockets;
using Mexc.Net.Objects.Models.Spot;
using Mexc.Net.Clients.SpotApi;

namespace Mexc.Net.UnitTests
{
    [TestFixture()]
    public class MexcV3RestClientTests
    {
        [TestCase(1508837063996)]
        [TestCase(1507156891385)]
        public async Task GetServerTime_Should_RespondWithServerTimeDateTime(long milisecondsTime)
        {
            // arrange
            DateTime expected = new DateTime(1970, 1, 1).AddMilliseconds(milisecondsTime);
            var time = new MexcV3CheckServerTime() { ServerTime = expected };
            var client = MexcV3TestHelpers.CreateResponseClient(JsonConvert.SerializeObject(time));

            // act
            var result = await client.SpotApi.MarketData.GetServerTimeAsync();

            // assert
            Assert.AreEqual(true, result.Success);
            Assert.AreEqual(expected, result.Data);
        }

        [TestCase(1508837063996)]
        [TestCase(1507156891385)]
        public async Task GetServerTimeStampAsync_Should_RespondWithServerTimeDateTime(long milisecondsTime)
        {
            // arrange
            long expected = milisecondsTime;
            var time = new MexcV3CheckServerTimeStamp() { ServerTime = expected };
            var client = MexcV3TestHelpers.CreateResponseClient(JsonConvert.SerializeObject(time));

            // act
            var result = await client.SpotApi.MarketData.GetServerTimeStampAsync();

            // assert
            Assert.AreEqual(true, result.Success);
            Assert.AreEqual(expected, result.Data);
        }

        [TestCase]
        public async Task StartUserStream_Should_RespondWithListenKey()
        {
            // arrange
            var key = new MexcV3ListenKey()
            {
                ListenKey = "123"
            };

            var client = MexcV3TestHelpers.CreateResponseClient(key, new MexcV3ClientOptions()
            {
                ApiCredentials = new ApiCredentials("Test", "Test"),
                SpotApiOptions = new MexcV3ApiClientOptions
                {
                    AutoTimestamp = false
                }
            });

            // act
            var result = await client.SpotApi.WebsocketAccount.StartUserStreamAsync();

            // assert
            Assert.IsTrue(result.Success);
            Assert.IsTrue(key.ListenKey == result.Data);
        }

        [TestCase]
        public async Task KeepAliveUserStream_Should_Respond()
        {
            // arrange
            var client = MexcV3TestHelpers.CreateResponseClient("{}", new MexcV3ClientOptions()
            {
                ApiCredentials = new ApiCredentials("Test", "Test"),
                SpotApiOptions = new MexcV3ApiClientOptions
                {
                    AutoTimestamp = false
                }
            });

            // act
            var result = await client.SpotApi.WebsocketAccount.KeepAliveUserStreamAsync("test");

            // assert
            Assert.IsTrue(result.Success);
        }

        [TestCase]
        public async Task StopUserStream_Should_Respond()
        {
            // arrange
            var client = MexcV3TestHelpers.CreateResponseClient("{}", new MexcV3ClientOptions()
            {
                ApiCredentials = new ApiCredentials("Test", "Test"),
                SpotApiOptions = new MexcV3ApiClientOptions
                {
                    AutoTimestamp = false
                }
            });

            // act
            var result = await client.SpotApi.WebsocketAccount.StopUserStreamAsync("test");

            // assert
            Assert.IsTrue(result.Success);
        }

        [TestCase("BTCUSDT")]
        [TestCase("ETHUSDT")]
        [TestCase("MXUSDT")]
        [TestCase("DAIUSDT")]
        public async Task EnablingAutoTimestamp_Should_CallServerTime(string symbol)
        {
            // arrange
            var client = MexcV3TestHelpers.CreateResponseClient("{}", new MexcV3ClientOptions()
            {
                ApiCredentials = new ApiCredentials("TestKey", "TestSecret"),
                SpotApiOptions = new MexcV3ApiClientOptions
                {
                    AutoTimestamp = true
                }
            });

            // act
            try
            {
                await client.SpotApi.SpotAccountTrade.GetOpenOrdersAsync(symbol);
            }
            catch (Exception)
            {
                //Exception is thrown because stream is being read twice, doesn't happen normally
            }


            // assert
            Mock.Get(client.RequestFactory).Verify(f => f.Create(It.IsAny<HttpMethod>(), It.Is<Uri>((uri) => uri.ToString().Contains("/time")), It.IsAny<int>()), Times.Exactly(2));
        }

        [TestCase()]
        public async Task ReceivingMexcError_Should_ReturnMexcErrorAndNotSuccess()
        {
            // arrange
            var client = MexcV3TestHelpers.CreateClient();
            MexcV3TestHelpers.SetErrorWithResponse(client, "{\"msg\": \"Error!\", \"code\": 123}", HttpStatusCode.BadRequest);

            // act
            var result = await client.SpotApi.MarketData.GetServerTimeAsync();

            // assert
            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Error);
            Assert.IsTrue(result.Error.Code == 123);
            Assert.IsTrue(result.Error.Message == "Error!");
        }

        [Test]
        public void ProvidingApiCredentials_Should_SaveApiCredentials()
        {
            // arrange
            // act
            var authProvider = new MexcV3AuthenticationProvider(new ApiCredentials("TestKey", "TestSecret"));

            // assert
            Assert.AreEqual(authProvider.Credentials.Key.GetString(), "TestKey");
            Assert.AreEqual(authProvider.Credentials.Secret.GetString(), "TestSecret");
        }

        [Test]
        public void AddingAuthToRequest_Should_AddApiKeyHeader()
        {
            // arrange
            var authProvider = new MexcV3AuthenticationProvider(new ApiCredentials("TestKey", "TestSecret"));
            var client = new HttpClient();
            var request = new Request(new HttpRequestMessage(HttpMethod.Get, "https://api.mexc.com"), client, 1);

            // act
            var headers = new Dictionary<string, string>();
            authProvider.AuthenticateRequest(null, request.Uri, HttpMethod.Get, new Dictionary<string, object>(), true, ArrayParametersSerialization.MultipleValues,
                HttpMethodParameterPosition.InUri, out var uriParameters, out var bodyParameters, out headers);

            // assert
            Assert.IsTrue(headers.First().Key == "x-mexc-apikey" && headers.First().Value == "TestKey");
        }       

        [TestCase("BTCUSDT", true)]
        [TestCase("NANOUSDT", true)]
        [TestCase("NANOAUSDTA", true)]
        [TestCase("NANOBTC", true)]
        [TestCase("ETHBTC", true)]
        [TestCase("BEETC", true)]
        [TestCase("EETC", false)]
        [TestCase("KP3RBNB", true)]
        [TestCase("BTC-USDT", false)]
        [TestCase("BTC-USD", false)]
        public void CheckValidMexcSymbol(string symbol, bool isValid)
        {
            if (isValid)
                Assert.DoesNotThrow(symbol.ValidateMexcSymbol);
            else
                Assert.Throws(typeof(ArgumentException), symbol.ValidateMexcSymbol);
        }

        [Test]
        public void CheckSocketInterfaces()
        {
            var assembly = Assembly.GetAssembly(typeof(MexcV3SocketClientSpotStreams));
            var clientInterfaces = assembly.GetTypes().Where(t => t.Name.StartsWith("IMexcSocketClient"));

            foreach (var clientInterface in clientInterfaces)
            {
                var implementation = assembly.GetTypes().Single(t => t.IsAssignableTo(clientInterface) && t != clientInterface);
                int methods = 0;
                foreach (var method in implementation.GetMethods().Where(m => m.ReturnType.IsAssignableTo(typeof(Task<CallResult<UpdateSubscription>>))))
                {
                    var interfaceMethod = clientInterface.GetMethod(method.Name, method.GetParameters().Select(p => p.ParameterType).ToArray());
                    Assert.NotNull(interfaceMethod, $"Missing interface for method {method.Name} in {implementation.Name} implementing interface {clientInterface.GetType().Name}");
                    methods++;
                }
                Debug.WriteLine($"{clientInterface.Name} {methods} methods validated");
            }
        }
    }
}
