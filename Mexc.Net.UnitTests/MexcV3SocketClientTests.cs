using System;
using Mexc.Net.Objects;
using Mexc.Net.UnitTests.TestImplementations;
using NUnit.Framework;
using Mexc.Net.Enums;
using Mexc.Net.Interfaces;
using System.Threading.Tasks;
using Mexc.Net.Objects.Models;
using Mexc.Net.Objects.Models.Spot.Socket;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;
using Mexc.Net.Interfaces.Clients;

namespace Mexc.Net.UnitTests
{
    [TestFixture()]
    public class MexcV3SocketClientTests
    {
        /// <summary>
        /// 订阅一个交易代码的逐笔交易的测试用例
        /// 用内定数据结构与订阅成功返回值的数据结构进行比较，如果所需比较部分相同则测试通过（这里的相同比较的是结构名称，而不是返回值）
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        [TestCase("BTCUSDT")]
        public async Task SubscribeToTradeUpdatesAsync_Should_TriggerWhenSymbolTickerStreamMessageIsReceived(string symbol)
        {
            #region 建立socket连接，订阅“MexcV3StreamTrade”数据，等待接收到返回值，存入“result”
            // arrange
            MexcV3TestSocket socket = new MexcV3TestSocket();
            IMexcV3SocketClient client = MexcV3TestHelpers.CreateSocketClient(socket, new MexcV3SocketClientOptions()
            {
                LogLevel = LogLevel.Debug
            });

            //Socket获取数据
            MexcV3StreamTrade result = null;
            await client.SpotPublicStreams.SubscribeToTradeUpdatesAsync(symbol, (response) => result = response.Data);
            #endregion

            MexcV3StreamTrade data = new MexcV3StreamTrade()
            {
                Stream = $"spot@public.deals.v3.api@BTCUSDT",                                       //当前订阅数据流
                Data = new MexcV3StreamTradeData                                                    //数据
                {
                    EventTime = new DateTime(2017, 1, 1).AddMilliseconds(1663556442436),            //事件时间eventTime
                    Deals = new MexcV3StreamTradeDeal[]
                        {
                            new MexcV3StreamTradeDeal
                            {
                                TradeType = MexcV3SpotSocketAccountOrderTradeType.buy,                  //交易类型tradeType
                                Price = (decimal)18767.98,                                              //成交价格price
                                DealTime = new DateTime(2017, 1, 1).AddMilliseconds(1663556442409),     //成交时间dealTime
                                Quantity = (decimal)0.096760                                            //成交数量quantity
                            }
                        },
                    EventType = "spot@public.deals.v3.api",
                    Symbol = "BTCUSDT"
                },
                Symbol = "BTCUSDT",
                EventTimeStamp = "1663556442436"
            };

            #region 序列化构造的数据结构为Json字符串（这里不做输出，仅用于调试过程中查看中间结果）
            string temp = JsonConvert.SerializeObject(data);
            #endregion

            // act
            socket.InvokeMessage(data);

            // assert
            Assert.IsNotNull(result);
            //结构比较
            Assert.IsTrue(MexcV3TestHelpers.AreEqual(data.Data.Deals.ToList()[0], result.Data.Deals.ToList()[0]));
        }

        /// <summary>
        /// 订阅一个交易代码的逐笔交易的测试用例
        /// 用内定数据结构与订阅成功返回值的数据结构进行比较，如果所需比较部分相同则测试通过（这里的相同比较的是结构名称，而不是返回值）
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        [TestCase("BTCUSDT", "ETHUSDT")]
        public async Task SubscribeToTradesUpdatesAsync_Should_TriggerWhenSymbolTickerStreamMessageIsReceived(string symbol1,string symbol2)
        {
            #region 建立socket连接，订阅“MexcV3StreamTrade”数据，等待接收到返回值，存入“result”
            // arrange
            MexcV3TestSocket socket = new MexcV3TestSocket();
            IMexcV3SocketClient client = MexcV3TestHelpers.CreateSocketClient(socket, new MexcV3SocketClientOptions()
            {
                LogLevel = LogLevel.Debug
            });

            //Socket获取数据
            MexcV3StreamTrade result = null;
            await client.SpotPublicStreams.SubscribeToTradeUpdatesAsync(new string[] { symbol1, symbol2 }, (response) => result = response.Data);
            #endregion

            MexcV3StreamTrade data = new MexcV3StreamTrade()
            {
                Stream = $"spot@public.deals.v3.api@BTCUSDT",                                           //当前订阅数据流
                Data = new MexcV3StreamTradeData                                                        //数据
                {
                    EventTime = new DateTime(2017, 1, 1).AddMilliseconds(1663556442436),                //事件时间eventTime
                    Deals = new MexcV3StreamTradeDeal[]
                        {
                            new MexcV3StreamTradeDeal
                            {
                                TradeType = MexcV3SpotSocketAccountOrderTradeType.buy,                  //交易类型tradeType
                                Price = (decimal)18767.98,                                              //成交价格price
                                DealTime = new DateTime(2017, 1, 1).AddMilliseconds(1663556442409),     //成交时间dealTime
                                Quantity = (decimal)0.096760                                            //成交数量quantity
                            }
                        },
                    EventType = "spot@public.deals.v3.api",
                    Symbol = "BTCUSDT"
                },
                Symbol = "BTCUSDT",
                EventTimeStamp = "1663556442436"
            };

            #region 序列化构造的数据结构为Json字符串（这里不做输出，仅用于调试过程中查看中间结果）
            string temp = JsonConvert.SerializeObject(data);
            #endregion

            // act
            socket.InvokeMessage(data);

            // assert
            Assert.IsNotNull(result);
            //结构比较
            Assert.IsTrue(MexcV3TestHelpers.AreEqual(data.Data.Deals.ToList()[0], result.Data.Deals.ToList()[0]));
        }

        /// <summary>
        /// 订阅K线数据流应在收到K线流消息时触发
        /// </summary>
        [TestCase("1663036484045")]
        public void SubscribingToKlineStream_Should_TriggerWhenKlineStreamMessageIsReceived(string eventTimestamp)
        {
            // arrange
            MexcV3TestSocket socket = new MexcV3TestSocket();
            IMexcV3SocketClient client = MexcV3TestHelpers.CreateSocketClient(socket);

            MexcV3StreamKline result = null;
            client.SpotPublicStreams.SubscribeToKlineUpdatesAsync("BTCUSDT", MexcV3StreamsKlineInterval.FifteenMinutes, (test) => result = test.Data);

            MexcV3StreamKline data = new MexcV3StreamKline()
            {
                Symbol = "BTCUSDT",
                SymbolDisplay = "BTCUSDT",
                Stream = "spot@public.kline.v3.api@BTCUSDT@Min15",
                EventTimeStamp = eventTimestamp,
                Data = new MexcV3StreamKlineData()
                {
                    EventTime = new DateTime(2017, 1, 1).AddMilliseconds(Convert.ToDouble(eventTimestamp)),
                    EventType = "spot@public.kline.v3.api",
                    Symbol = "BTCUSDT",
                    Details = new MexcV3StreamKlineDetail()
                    {
                        CloseTime = new DateTime(2017, 1, 1),
                        QuoteVolume = (decimal)213072.97695067,
                        ClosePrice = (decimal)22203.24,
                        HighPrice = (decimal)22203.69,
                        Interval = MexcV3StreamsKlineInterval.FifteenMinutes,
                        LowPrice = (decimal)22172.4,
                        OpenPrice = (decimal)22174.24,
                        Symbol = "BTCUSDT",
                        OpenTime = new DateTime(2017, 1, 1),
                        Volume = (decimal)9.604942
                    }
                }
            };

            // act
            socket.InvokeMessage(data);

            // assert
            Assert.IsNotNull(result);
            Assert.IsTrue(MexcV3TestHelpers.AreEqual(data, result, "Data"));
            Assert.IsTrue(MexcV3TestHelpers.AreEqual(data.Data, result.Data));
        }

        /// <summary>
        /// 订阅单一交易代码增量深度信息(实时)
        /// </summary>
        [TestCase("BTCUSDT")]
        public void SubscribeToDiffDeptStream_Should_TriggerWhenKlineStreamMessageIsReceived(string symbol)
        {
            // arrange
            MexcV3TestSocket socket = new MexcV3TestSocket();
            IMexcV3SocketClient client = MexcV3TestHelpers.CreateSocketClient(socket);

            MexcV3StreamDepth result = null;
            client.SpotPublicStreams.SubscribeToDiffDepthUpdatesAsync(symbol, (test) => result = test.Data);

            MexcV3StreamDepth data = new MexcV3StreamDepth()
            {
                Stream = "spot@public.increase.depth.v3.api@BTCUSDT",
                Data = new MexcV3StreamDepthData()
                {
                    EventTime = new DateTime(2017, 1, 1),
                    EventType = "spot@public.kline.v3.api",
                    Symbol = "BTCUSDT",
                    Asks = new MexcV3StreamDepthAskOrBidDetails[]
                    {
                        new MexcV3StreamDepthAskOrBidDetails
                        {
                            Price = (decimal)18767.98,                 //成交价格price
                            Quantity = (decimal)0.096760               //成交数量quantity
                        }
                    },
                    Bids = new MexcV3StreamDepthAskOrBidDetails[]
                    {
                        new MexcV3StreamDepthAskOrBidDetails
                        {
                            Price = (decimal)18737.98,                 //成交价格price
                            Quantity = (decimal)0.082760               //成交数量quantity
                        }
                    }
                },
                Symbol = "BTCUSDT",
                EventTimeStamp = "1663556442409"
            };

            // act
            socket.InvokeMessage(data);

            // assert
            Assert.IsNotNull(result);
            Assert.IsTrue(MexcV3TestHelpers.AreEqual(data, result, "Data"));
            Assert.IsTrue(MexcV3TestHelpers.AreEqual(data.Data.Asks.ToList()[0], result.Data.Asks.ToList()[0]));
            Assert.IsTrue(MexcV3TestHelpers.AreEqual(data.Data.Bids.ToList()[0], result.Data.Bids.ToList()[0]));
        }

        /// <summary>
        /// 订阅账户成交(实时)
        /// </summary>
        [TestCase("6c2826daa1c98c1b33542fc08a733d753431134f7da8d2d09217820b7bad85e7")]
        public void SubscribeToPrivateDealsStream_Should_TriggerWhenKlineStreamMessageIsReceived(string listenKey)
        {
            // arrange
            MexcV3TestSocket socket = new MexcV3TestSocket();
            IMexcV3SocketClient client = MexcV3TestHelpers.CreateSocketClient(socket);

            MexcV3StreamPrivateDeals result = null;
            client.SpotPublicStreams.SubscribeToPrivateDealsUpdatesAsync(listenKey, (test) => result = test.Data);

            MexcV3StreamPrivateDeals data = new MexcV3StreamPrivateDeals()
            {
                ListenKey = listenKey,
                Stream = "spot@private.deals.v3.api",
                Data = new MexcV3StreamPrivateDealsData()
                {
                    TradeType = MexcV3SpotSocketAccountOrderTradeType.buy,
                    DealTime = new DateTime(2017, 1, 1)
                },
                Symbol = "BTCUSDT",
                SymbolDisplay = "BTCUSDT",
                EventTimeStamp = "1663556442409"
            };

            // act
            socket.InvokeMessage(data);

            // assert
            Assert.IsNotNull(result);
            Assert.IsTrue(MexcV3TestHelpers.AreEqual(data, result, "Data"));
            Assert.IsTrue(MexcV3TestHelpers.AreEqual(data.Data, result.Data));
        }

        /// <summary>
        /// 订阅账户订单(实时)
        /// </summary>
        [TestCase("6c2826daa1c98c1b33542fc08a733d753431134f7da8d2d09217820b7bad85e7")]
        public void SubscribeToPrivateOrdersStream_Should_TriggerWhenKlineStreamMessageIsReceived(string listenKey)
        {
            // arrange
            MexcV3TestSocket socket = new MexcV3TestSocket();
            IMexcV3SocketClient client = MexcV3TestHelpers.CreateSocketClient(socket);

            MexcV3StreamPrivateOrders result = null;
            client.SpotPublicStreams.SubscribeToPrivateOrdersUpdatesAsync(listenKey, (test) => result = test.Data);

            MexcV3StreamPrivateOrders data = new MexcV3StreamPrivateOrders()
            {
                ListenKey = listenKey,
                Stream = "spot@private.orders.v3.api",
                EventTime = new DateTime(1970, 1, 1).AddMilliseconds(1663556442409),
                Data = new MexcV3StreamPrivateOrdersData()
                {
                    RemainAmount = (decimal)5.4,
                    CreateTime = new DateTime(2017, 1, 1),
                    TradeType = MexcV3SpotSocketAccountOrderTradeType.buy,
                    RemainQuantity = (decimal)6,
                    Amount = (decimal)5.4,
                    ClientOrderId = "",
                    OrderId = "835014be43e8426998e00eb98e449ae5",
                    MakerOrTaker = MexcV3SpotSocketAccountOrderMakerOrTaker.isTaker,
                    OrderType = MexcV3SpotSocketAccountOrderType.LIMIT,
                    Price = (decimal)0.9,
                    OrderStatus = MexcV3SpotSocketAccountOrderStatus.NewOrder,
                    Quantity = (decimal)6
                },
                Symbol = "BTCUSDT",
                SymbolDisplay = "BTCUSDT",
                EventTimeStamp = "1663556442409"
            };

            // act
            socket.InvokeMessage(data);

            // assert
            Assert.IsNotNull(result);
            Assert.IsTrue(MexcV3TestHelpers.AreEqual(data, result, "Data"));
            Assert.IsTrue(MexcV3TestHelpers.AreEqual(data.Data, result.Data));
        }
    }
}
