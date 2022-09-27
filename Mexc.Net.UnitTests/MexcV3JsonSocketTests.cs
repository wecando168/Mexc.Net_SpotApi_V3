using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Mexc.Net.Interfaces.Clients;
using Mexc.Net.Objects.Models.Spot.Socket;
using Mexc.Net.UnitTests.TestImplementations;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Mexc.Net.UnitTests
{
    internal class MexcV3JsonSocketTests
    {
        [Test]
        //Websocket 行情推送-逐笔交易(实时)
        public async Task ValidateTradeUpdateStreamJson()
        {
            await TestFileToObject<MexcV3StreamTrade>(@"JsonResponses/Spot/Socket/MexcV3TradeUpdate.txt");
        }

        [Test]
        //Websocket 行情推送-K线 Streams
        public async Task ValidateKlineUpdateStreamJson()
        {
            await TestFileToObject<MexcV3StreamKline>(@"JsonResponses/Spot/Socket/MexcV3KlineUpdate.txt", new List<string> { "B" });
        }

        [Test]
        //Websocket 行情推送-增量深度信息(实时)
        public async Task ValidateDepthUpdateStreamJson()
        {
            await TestFileToObject<MexcV3StreamDepth>(@"JsonResponses/Spot/Socket/MexcV3DepthUpdate.txt");
        }

        [Test]
        //Websocket 账户信息推送-账户成交(实时)
        public async Task ValidatePrivateDealsUpdateStreamJson()
        {
            await TestFileToObject<MexcV3StreamPrivateDeals>(@"JsonResponses/Spot/Socket/MexcV3PrivateDealsUpdate.txt");
        }

        [Test]
        //Websocket 账户信息推送-账户订单(实时)
        public async Task ValidatePrivateOrdersUpdateStreamJson()
        {
            await TestFileToObject<MexcV3StreamPrivateOrders>(@"JsonResponses/Spot/Socket/MexcV3PrivateOrdersUpdate.txt");
        }

        private static async Task TestFileToObject<T>(string filePath, List<string> ignoreProperties = null)
        {
            var listener = new EnumValueTraceListener();
            Trace.Listeners.Add(listener);
            var path = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            string json;
            try
            {
                var file = File.OpenRead(Path.Combine(path, filePath));
                using var reader = new StreamReader(file);
                json = await reader.ReadToEndAsync();
            }
            catch (FileNotFoundException)
            {
                throw;
            }

            var result = JsonConvert.DeserializeObject<T>(json);
            MexcV3JsonToObjectComparer<IMexcV3SocketClient>.ProcessData("", result, json, ignoreProperties: new Dictionary<string, List<string>>
            {
                { "", ignoreProperties ?? new List<string>() }
            });
            Trace.Listeners.Remove(listener);
        }
    }

    internal class EnumValueTraceListener : TraceListener
    {
        public override void Write(string message)
        {
            if (message.Contains("Cannot map"))
                throw new Exception("Enum value error: " + message);
        }

        public override void WriteLine(string message)
        {
            if (message.Contains("Cannot map"))
                throw new Exception("Enum value error: " + message);
        }
    }
}
