using Mexc.Net.Clients;
using Mexc.Net.Objects;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Objects;
using Microsoft.Extensions.Logging;
using Mexc.Net.Enums;
using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Sockets;
using Newtonsoft.Json;
using Mexc.Net.Objects.Models.Spot.Socket;
using Mexc.Net.Interfaces;

#region Provide you API key/secret in these fields to retrieve data related to your account
const string accessKey = "Use Your Exchange Access Key";
const string secretKey = "Use Your Exchange SecretKey Key";
#endregion

//目前现货Rest Api使用最新的V3版本,配置一个默认的V3版的Rest Api 客户端
MexcV3RestClient.SetDefaultOptions(new MexcV3ClientOptions()
{
    //使用accessKey, secretKey生成一个新的API凭证
    ApiCredentials = new ApiCredentials(accessKey, secretKey),
    //LogLevel = LogLevel.Debug
    LogLevel = LogLevel.Trace,
    //OutputOriginalData = true
    
});

//目前现货webSocket Api使用V3版本, 配置一个默认的V3版的webSocket Api 客户端(新版V3 Api)
MexcV3SocketClient.SetDefaultOptions(new MexcV3SocketClientOptions()
{
    ApiCredentials = new ApiCredentials(accessKey, secretKey),
    //LogLevel = LogLevel.Debug
    LogLevel = LogLevel.Trace,
    //OutputOriginalData = true
});

string? read = "";
while (read != "R" && read != "r" && read != "P" && read != "p" && read != "U" && read != "u") 
{
    Console.WriteLine("Run [R]est or [P]ublicSocket  or [U]serSocket example?");
    read = Console.ReadLine();
}

if (read == "R" || read == "r")
{
    //一、行情接口测试-已完成
    await TestMarketDataEndpoints();

    //二、母子账户接口-未开发
    //await TestSubAccountEndpoints();

    //三、现货账户和交易接口测试-已完成（批量下单除外）
    //await TestSpotAccountTradeEndpoints();

    //四、钱包接口测试-开发中...
    //await TestWalletEndpoints();

    //五、ETF接口测试-开发中...
    //await TestETFEndpoints();

    //六、杠杆账户和交易接口-未开发
    //await TestMarginAccountTradeEndpoints();

    //七、现货账户WebSocket账户Listen Key维护接口测试-已完成
    //await TestSpotWebSocketAccountEndpoints();
}
else if(read == "P" || read == "p")
{
    MexcV3SocketClient? mexcV3SocketClient = new MexcV3SocketClient();
    CallResult<UpdateSubscription>? publicSubV3scription = null;

    #region 订阅逐笔交易
    Console.WriteLine("Press enter to subscribe trade stream");
    Console.ReadLine();

    //订阅一个交易代码逐笔交易
    publicSubV3scription = await mexcV3SocketClient.SpotPublicStreams.SubscribeToTradeUpdatesAsync("BTCUSDT", data =>
    {
        Console.WriteLine($"Stream:{data.Data.Stream} StreamSymbol:{data.Data.Symbol}");
        Console.WriteLine($"DataEventTime:{data.Data.Data.EventTime} DataEventType:{data.Data.Data.EventType} DataSymbol:{data.Data.Data.Symbol}");
        foreach (var item in data.Data.Data.Deals)
        {
            Console.WriteLine($"DealTradeType:{item.TradeType} DealPrice:{item.Price} DealTime:{item.DealTime} DealQuantity:{item.Quantity}");
        }
        Console.Write($"@ StreamEventTimeStamp:{data.Data.EventTimeStamp}\r\n");
    });

    if (!publicSubV3scription.Success)
    {
        Console.WriteLine("Failed to sub" + publicSubV3scription.Error);
        Console.ReadLine();
        return;
    }

    publicSubV3scription.Data.ConnectionLost += () => Console.WriteLine("Connection lost, trying to reconnect..");
    publicSubV3scription.Data.ConnectionRestored += (t) => Console.WriteLine("Connection restored");

    //订阅多交易代码逐笔交易
    publicSubV3scription = await mexcV3SocketClient.SpotPublicStreams.SubscribeToTradeUpdatesAsync(new string[] { "ETHUSDT", "MXUSDT" }, data =>
    {
        Console.WriteLine($"Stream:{data.Data.Stream} StreamSymbol:{data.Data.Symbol}");
        Console.WriteLine($"DataEventTime:{data.Data.Data.EventTime} DataEventType:{data.Data.Data.EventType} DataSymbol:{data.Data.Data.Symbol}");
        foreach (var item in data.Data.Data.Deals)
        {
            Console.WriteLine($"DealTradeType:{item.TradeType} DealPrice:{item.Price} DealTime:{item.DealTime} DealQuantity:{item.Quantity}");
        }
        Console.Write($"@ StreamEventTimeStamp:{data.Data.EventTimeStamp}\r\n");
    });

    if (!publicSubV3scription.Success)
    {
        Console.WriteLine("Failed to sub" + publicSubV3scription.Error);
        Console.ReadLine();
        return;
    }

    publicSubV3scription.Data.ConnectionLost += () => Console.WriteLine("Connection lost, trying to reconnect..");
    publicSubV3scription.Data.ConnectionRestored += (t) => Console.WriteLine("Connection restored");
    #endregion

    #region 订阅K线
    Console.WriteLine("Press enter to subscribe candlestick stream");
    Console.ReadLine();
    //订阅一个交易代码，单时间区间
    publicSubV3scription = await mexcV3SocketClient.SpotPublicStreams.SubscribeToKlineUpdatesAsync("BTCUSDT", MexcV3StreamsKlineInterval.FiveMinutes, data =>
    {
        Console.WriteLine($"Stream:{data.Data.Stream} StreamSymbol:{data.Data.Symbol}");
        Console.WriteLine($"DataEventTime:{data.Data.Data.EventTime} DataEventType:{data.Data.Data.EventType} DataSymbol:{data.Data.Data.Symbol}");
        Console.WriteLine($"DetailSymbol:{data.Data.Data.Details.Symbol} DetailOpenPrice:{data.Data.Data.Details.OpenPrice} DetailClosePrice:{data.Data.Data.Details.ClosePrice} DetailQuoteVolume:{data.Data.Data.Details.QuoteVolume}");
        Console.Write($"@ StreamEventTimeStamp:{data.Data.EventTimeStamp}\r\n");
    });

    if (!publicSubV3scription.Success)
    {
        Console.WriteLine("Failed to sub" + publicSubV3scription.Error);
        Console.ReadLine();
        return;
    }

    publicSubV3scription.Data.ConnectionLost += () => Console.WriteLine("Connection lost, trying to reconnect..");
    publicSubV3scription.Data.ConnectionRestored += (t) => Console.WriteLine("Connection restored");

    //订阅多交易代码，多时间区间
    publicSubV3scription = await mexcV3SocketClient.SpotPublicStreams.SubscribeToKlineUpdatesAsync(new string[] { "ETHUSDT", "MXUSDT" }, new MexcV3StreamsKlineInterval[] { MexcV3StreamsKlineInterval.OneMinute, MexcV3StreamsKlineInterval.FiveMinutes }, data =>
    {
        Console.WriteLine($"Stream:{data.Data.Stream} StreamSymbol:{data.Data.Symbol}");
        Console.WriteLine($"DataEventTime:{data.Data.Data.EventTime} DataEventType:{data.Data.Data.EventType} DataSymbol:{data.Data.Data.Symbol}");
        Console.WriteLine($"DetailSymbol:{data.Data.Data.Details.Symbol} DetailOpenPrice:{data.Data.Data.Details.OpenPrice} DetailClosePrice:{data.Data.Data.Details.ClosePrice} DetailQuoteVolume:{data.Data.Data.Details.QuoteVolume}");
        Console.Write($"@ StreamEventTimeStamp:{data.Data.EventTimeStamp}\r\n");
    });

    if (!publicSubV3scription.Success)
    {
        Console.WriteLine("Failed to sub" + publicSubV3scription.Error);
        Console.ReadLine();
        return;
    }

    publicSubV3scription.Data.ConnectionLost += () => Console.WriteLine("Connection lost, trying to reconnect..");
    publicSubV3scription.Data.ConnectionRestored += (t) => Console.WriteLine("Connection restored");
    #endregion

    #region 订阅增量深度信息
    Console.WriteLine("Press enter to subscribe to BTCUSDT trade stream");
    Console.ReadLine();

    //订阅一个交易代码增量深度信息
    publicSubV3scription = await mexcV3SocketClient.SpotPublicStreams.SubscribeToDiffDepthUpdatesAsync("BTCUSDT", data =>
    {
        Console.WriteLine($"Stream:{data.Data.Stream} StreamSymbol:{data.Data.Symbol}");
        Console.WriteLine($"DataEventTime:{data.Data.Data.EventTime} DataEventType:{data.Data.Data.EventType} DataSymbol:{data.Data.Data.Symbol}");
        if (!Object.Equals(data.Data.Data.Asks, null))
        {
            foreach (var item in data.Data.Data.Asks)
            {
                Console.WriteLine($"SellPrice:{item.Price} SellQuantity:{item.Quantity}");
            }
        }
        if (!Object.Equals(data.Data.Data.Bids, null))
        {
            foreach (var item in data.Data.Data.Bids)
            {
                Console.WriteLine($"BuyPrice:{item.Price} BuyQuantity:{item.Quantity}");
            }
        }
        Console.Write($"@ StreamEventTimeStamp:{data.Data.EventTimeStamp}\r\n");
    });

    if (!publicSubV3scription.Success)
    {
        Console.WriteLine("Failed to sub" + publicSubV3scription.Error);
        Console.ReadLine();
        return;
    }

    publicSubV3scription.Data.ConnectionLost += () => Console.WriteLine("Connection lost, trying to reconnect..");
    publicSubV3scription.Data.ConnectionRestored += (t) => Console.WriteLine("Connection restored");

    //订阅多个交易代码增量深度信息
    publicSubV3scription = await mexcV3SocketClient.SpotPublicStreams.SubscribeToDiffDepthUpdatesAsync(new string[] { "ETHUSDT", "MXUSDT" }, data =>
    {
        Console.WriteLine($"Stream:{data.Data.Stream} StreamSymbol:{data.Data.Symbol}");
        Console.WriteLine($"DataEventTime:{data.Data.Data.EventTime} DataEventType:{data.Data.Data.EventType} DataSymbol:{data.Data.Data.Symbol}");
        if (!Object.Equals(data.Data.Data.Asks, null))
        {
            foreach (var item in data.Data.Data.Asks)
            {
                Console.WriteLine($"SellPrice:{item.Price} SellQuantity:{item.Quantity}");
            }
        }
        if (!Object.Equals(data.Data.Data.Bids, null))
        {
            foreach (var item in data.Data.Data.Bids)
            {
                Console.WriteLine($"BuyPrice:{item.Price} BuyQuantity:{item.Quantity}");
            }
        }
        Console.Write($"@ StreamEventTimeStamp:{data.Data.EventTimeStamp}\r\n");
    });

    if (!publicSubV3scription.Success)
    {
        Console.WriteLine("Failed to sub" + publicSubV3scription.Error);
        Console.ReadLine();
        return;
    }

    publicSubV3scription.Data.ConnectionLost += () => Console.WriteLine("Connection lost, trying to reconnect..");
    publicSubV3scription.Data.ConnectionRestored += (t) => Console.WriteLine("Connection restored");
    #endregion
}
else if (read == "U" || read == "u")
{
    #region 生成订阅私有数据的监听密钥（Listen Key)
    string listenKey = string.Empty;
    using (var client = new MexcV3RestClient())
    {
        #region 创建现货WebSocket 连接 Listen Key
        Console.WriteLine("创建现货WebSocket 连接 Listen Key");
        WebCallResult<string>? result = await client.SpotApi.WebsocketAccount.StartUserStreamAsync();
        listenKey = result.Data;
        Console.WriteLine($"New Listen Key :{(listenKey != null ? listenKey : "Create failed!")}");
        #endregion
    }
    if (string.IsNullOrWhiteSpace(listenKey))
    {
        return;
    }
    #endregion

    #region 构造可以获取私有账户数据的连接客户端
    MexcV3SocketClientOptions privateOptions = new MexcV3SocketClientOptions();
    privateOptions.SpotStreamsOptions.BaseAddress = MexcV3ApiAddresses.Default.SpotUserSocketClientAddress;
    privateOptions.SpotStreamsOptions.ApiCredentials = new ApiCredentials(accessKey, secretKey);
    MexcV3SocketClient? mexcV3SocketClient = new MexcV3SocketClient(privateOptions);
    CallResult<UpdateSubscription>? privateSubV3scription = null;
    #endregion

    #region 订阅账户成交（实时）
    Console.WriteLine("Press enter to subscribe to account deals stream");
    Console.ReadLine();
    #region 方法一(功能已实现，保留请勿删除）：
    //privateSubV3scription = await mexcV3SocketClient.SpotPrivateStreams.SubscribeToPrivateDealsUpdatesAsync(listenKey, data =>
    //{
    //    Console.WriteLine($"Stream:{data.Data.Stream} StreamSymbol:{data.Data.Symbol}");
    //    Console.WriteLine($"AccountDealsClientOrderId:{data.Data.Data.ClientOrderId} AccountDealsOrderId:{data.Data.Data.OrderId} AccountDealsPrice:{data.Data.Data.Price} AccountDealsQuantity:{data.Data.Data.Quantity} AccountDealsMakerOrTaker:{data.Data.Data.MakerOrTaker}");
    //    Console.Write($"@ StreamEventTimeStamp:{data.Data.EventTimeStamp}\r\n");
    //});
    #endregion

    #region 方法二：
    privateSubV3scription = await mexcV3SocketClient.SpotPrivateStreams.SubscribeToPrivateDealsUpdatesAsync(listenKey, data =>
    {
        MexcV3StreamPrivateDeals? result = data.Data;
        if (result != null)
        {
            result.ListenKey = listenKey;
            Console.WriteLine(
                $"Stream:{result.Stream}\t" +
                $"ListenKey:{result.ListenKey}\r\n" +
                $"Symbol:{result.Symbol} " +
                $"TradeType {result.Data.TradeType} " +
                $"Price {result.Data.Price} " +
                $"Quantity {result.Data.Quantity} " +
                $"ClientOrderId {result.Data.ClientOrderId} " +
                $"OrderId {result.Data.OrderId} " +
                $"TradeId {result.Data.TradeId} " +
                $"@ {result.Data.DealTime}");
        }
    });
    #endregion

    if (!privateSubV3scription.Success)
    {
        Console.WriteLine("Failed to sub" + privateSubV3scription.Error);
        Console.ReadLine();
        return;
    }

    privateSubV3scription.Data.ConnectionLost += () => Console.WriteLine("Connection lost, trying to reconnect..");
    privateSubV3scription.Data.ConnectionRestored += (t) => Console.WriteLine("Connection restored");
    #endregion

    #region 订阅账户订单（实时）
    Console.WriteLine("Press enter to subscribe to account orders stream");
    Console.ReadLine();
    #region 方法一(功能已实现，保留请勿删除）：
    //privateSubV3scription = await mexcV3SocketClient.SpotPrivateStreams.SubscribeToPrivateOrdersUpdatesAsync(listenKey, data =>
    //{
    //    Console.WriteLine($"Stream:{data.Data.Stream} StreamSymbol:{data.Data.Symbol}");
    //    Console.WriteLine($"AccountOrdersClientOrderId:{data.Data.Data.ClientOrderId} AccountOrdersOrderId:{data.Data.Data.OrderId} AccountOrdersPrice:{data.Data.Data.Price} AccountDealsQuantity:{data.Data.Data.Quantity} AccountDealsMakerOrTaker:{data.Data.Data.MakerOrTaker}");
    //    Console.Write($"@ StreamEventTimeStamp:{data.Data.EventTimeStamp}\r\n");
    //});
    #endregion

    #region 方法二：
    privateSubV3scription = await mexcV3SocketClient.SpotPrivateStreams.SubscribeToPrivateOrdersUpdatesAsync(listenKey, data =>
    {
        MexcV3StreamPrivateOrders? result = data.Data;
        if (result != null)
        {
            result.ListenKey = listenKey;
            Console.WriteLine(
                $"Stream:{result.Stream}\t" +
                $"ListenKey:{result.ListenKey}\r\n" +
                $"Symbol:{result.Symbol} " +
                $"ClientOrderId {result.Data.ClientOrderId} " +
                $"OrderId {result.Data.OrderId} " +
                $"Price {result.Data.Price} " +
                $"Quantity {result.Data.Quantity} " +
                $"@ {result.Data.CreateTime}"
                );
        }
    });
    #endregion

    if (!privateSubV3scription.Success)
    {
        Console.WriteLine("Failed to sub" + privateSubV3scription.Error);
        Console.ReadLine();
        return;
    }

    privateSubV3scription.Data.ConnectionLost += () => Console.WriteLine("Connection lost, trying to reconnect..");
    privateSubV3scription.Data.ConnectionRestored += (t) => Console.WriteLine("Connection restored");
    #endregion
}

//行情接口测试-已完成
static async Task TestMarketDataEndpoints()
{
    using (var mexcV3RestClient = new MexcV3RestClient())
    {
        #region 测试服务器连通性
        Console.WriteLine("测试服务器连通性");
        await HandleRequest("Ping Server", () => mexcV3RestClient.SpotApi.MarketData.PingAsync(), 
            result => $"{result.ToString()}"
            );
        #endregion

        #region 获取服务器当前时间戳
        Console.WriteLine("获取服务器当前时间戳");
        await HandleRequest("Server Time Stamp", () => mexcV3RestClient.SpotApi.MarketData.GetServerTimeStampAsync(), 
            result => $"{result.ToString()}"
            );
        #endregion

        #region 获取服务器当前时间
        Console.WriteLine("获取服务器当前时间");
        await HandleRequest("Server Time", () => mexcV3RestClient.SpotApi.MarketData.GetServerTimeAsync(), 
            result => $"{result.Date.ToString()}"
            );
        #endregion

        #region 获取交易规范信息，提取交易代码列表
        Console.WriteLine("获取交易规范信息，提取交易代码列表");
        await HandleRequest("Symbol list", () => mexcV3RestClient.SpotApi.MarketData.GetExchangeInfoAsync(), 
            result => string.Join("", result.Symbols.Select(s => $"\r\n{s.SymbolName.PadRight(14,' ')}基础币:{s.BaseAsset.PadRight(10, ' ')}报价币:{s.QuoteAsset.PadRight(10, ' ')}价格精度:{s.QuotePrecision}\t数量精度:{s.QuoteAssetPrecision}").Take(100)) + "\r\n......"
            );
        #endregion

        #region 深度信息
        Console.WriteLine("深度信息");
        await HandleRequest("Order Book", () => mexcV3RestClient.SpotApi.MarketData.GetOrderBookAsync(
            symbol: "SHIBDOGE",
            limit: 500),
            result => $"\r\nSymbol:{result.Symbol.PadRight(15, ' ')}Last Update Id:{result.LastUpdateId}" + string.Join(", ", result.Bids.Select(b => $"\r\nBuy Price:{b.Price.ToString().PadRight(12, ' ')}Buy Quantity:{b.Quantity.ToString().PadRight(12, ' ')}").Take(10)) + "\r\netc......"
            );
        #endregion

        #region 近期成交列表
        Console.WriteLine("近期成交列表");
        await HandleRequest("Recent Trades List", () => mexcV3RestClient.SpotApi.MarketData.GetRecentTradesAsync(
            symbol: "PIGUSDT",
            limit: 500),
            result => string.Join(", ", result.Select(s => $"\r\nOrder Id:{(string.IsNullOrWhiteSpace(s.OrderId) ? string.Empty : s.OrderId).PadRight(12, ' ')}Price:{s.Price.ToString().PadRight(12, ' ')}Base Quantity:{s.BaseQuantity.ToString().PadRight(12, ' ')}Quote Quantity:{s.QuoteQuantity.ToString().PadRight(12, ' ')}").Take(10)) + "\r\netc......"
            );
        #endregion

        #region 旧交易查询
        Console.WriteLine("旧交易查询");
        await HandleRequest("Old Trade Lookup", () => mexcV3RestClient.SpotApi.MarketData.GetTradeHistoryAsync(
            symbol: "PIGUSDT",
            limit: 500),
            result => string.Join(", ", result.Select(s => $"\r\nOrder Id:{(string.IsNullOrWhiteSpace(s.OrderId) ? string.Empty : s.OrderId).PadRight(12, ' ')}Price:{s.Price.ToString().PadRight(12, ' ')}Base Quantity:{s.BaseQuantity.ToString().PadRight(12, ' ')}Quote Quantity:{s.QuoteQuantity.ToString().PadRight(12, ' ')}").Take(10)) + "\r\netc......"
            );
        #endregion

        #region 近期成交(归集)
        Console.WriteLine("近期成交(归集)");
        await HandleRequest("Compressed Aggregate Trades List", () => mexcV3RestClient.SpotApi.MarketData.GetAggregatedTradeHistoryAsync(
            symbol: "BTCUSDT",
            startTime: DateTime.UtcNow.AddHours(-1),
            endTime: DateTime.UtcNow,
            limit: 500),
            result => string.Join(", ", result.Select(s => $"\r\n" +
            $"Order Id:{(string.IsNullOrWhiteSpace(s.OrderId) ? string.Empty : s.OrderId).PadRight(4, ' ')}" +
            $"First tradeId:{(string.IsNullOrWhiteSpace(s.FirstTradeId) ? string.Empty : s.FirstTradeId).PadRight(4, ' ')}" +
            $"Last tradeId:{(string.IsNullOrWhiteSpace(s.LastTradeId) ? string.Empty : s.LastTradeId).PadRight(4, ' ')}" +
            $"Price:{s.Price.ToString().PadRight(12, ' ')}Quantity:{s.Quantity.ToString().PadRight(12, ' ')}" +
            $"TradeTime:{s.TradeTime.ToString().PadRight(12, ' ')}").Take(10)) + "\r\netc......"
            );
        #endregion 

        #region K线数据
        Console.WriteLine("K线数据");
        await HandleRequest("SHIBDOGE Kline", () => mexcV3RestClient.SpotApi.MarketData.GetKlinesAsync(
            symbol: "SHIBDOGE",
            interval: MexcV3RestKlineInterval.OneMinute,
            startTime: DateTime.UtcNow.AddDays(-6),
            endTime: DateTime.UtcNow,
            limit: 500),
            result => string.Join(", ", result.Select(s => $"\r\n{s.OpenPrice} {s.ClosePrice}").Take(10)) + "\r\netc......"
            );
        #endregion
        
        #region 当前平均价格(指定单一交易代码）
        Console.WriteLine("指定单一交易代码当前平均价格");
        await HandleRequest("Current Average Price", () => mexcV3RestClient.SpotApi.MarketData.GetCurrentAvgPriceAsync(
            symbol: "SHIBDOGE"),
            result => $"\r\nPrice:{result.Price.ToString().PadRight(15, ' ')}Minutes:{result.Minutes.ToString().PadRight(15, ' ')}"
            );
        #endregion

        #region 24小时价格滚动情况(指定单一交易代码）
        Console.WriteLine("指定单一交易代码24小时价格滚动情况");
        await HandleRequest("24hr Ticker Price Change Statistics", () => mexcV3RestClient.SpotApi.MarketData.GetTickerAsync(
            symbol: "SHIBDOGE"),
            result => $"\r\nSymbol:{result.Symbol.PadRight(15, ' ')}Open Price:{result.OpenPrice.ToString().PadRight(15, ' ')}Last Price:{result.LastPrice.ToString().PadRight(15, ' ')}Price Change Percent:{result.PriceChangePercent.ToString().PadRight(15, ' ')}"
            );
        #endregion

        #region 24小时价格滚动情况（所有交易代码）
        Console.WriteLine("所有交易代码24小时价格滚动情况");
        await HandleRequest("24hr All Tickers Price Change Statistics", () => mexcV3RestClient.SpotApi.MarketData.GetTickersAsync(),
            result => string.Join(", ", result.Select(s => $"\r\nSymbol:{s.Symbol.PadRight(15, ' ')}Open Price:{s.OpenPrice.ToString().PadRight(15, ' ')}Last Price:{s.LastPrice.ToString().PadRight(15, ' ')}Price Change Percent:{s.PriceChangePercent.ToString().PadRight(15, ' ')}").Take(10)) + "\r\netc......"
            );
        #endregion

        #region 最新价格(指定单一交易代码）
        Console.WriteLine("指定单一交易代码最新价格");
        await HandleRequest("Symbol Price Ticker", () => mexcV3RestClient.SpotApi.MarketData.GetPriceAsync(
            symbol: "SHIBDOGE"),
            result => $"\r\nSymbol:{result.Symbol.PadRight(15, ' ')}Price:{result.Price.ToString().PadRight(15, ' ')}"
            );
        #endregion

        #region 最新价格（所有交易代码）
        Console.WriteLine("所有交易代码最新价格");
        await HandleRequest("Symbol Price Ticker", () => mexcV3RestClient.SpotApi.MarketData.GetPricesAsync(),
            result => string.Join(", ", result.Select(s => $"\r\nSymbol:{s.Symbol.PadRight(15, ' ')}Price:{s.Price.ToString().PadRight(15, ' ')}").Take(10)) + "\r\netc......"
            );
        #endregion

        #region 当前最优挂单(指定单一交易代码）
        Console.WriteLine("指定单一交易代码当前最优挂单");
        await HandleRequest("Symbol Order Book Ticker", () => mexcV3RestClient.SpotApi.MarketData.GetBookPriceAsync(
            symbol: "SHIBDOGE"),
            result => $"\r\nSymbol:{result.Symbol.PadRight(15, ' ')}Best Buy Price:{result.BestBidPrice.ToString().PadRight(15, ' ')}Best Buy Quantity:{result.BestBidQuantity.ToString().PadRight(15, ' ')}Best Sell Price:{result.BestAskPrice.ToString().PadRight(15, ' ')}Best Sell Price:{result.BestAskQuantity.ToString().PadRight(15, ' ')}"
            );
        #endregion
        
        #region 当前最优挂单（所有交易代码）
        Console.WriteLine("所有交易代码当前最优挂单");
        await HandleRequest("Symbol Order Book Ticker", () => mexcV3RestClient.SpotApi.MarketData.GetBookPricesAsync(),
            result => string.Join(" ", result.Select(s => $"\r\nSymbol:{s.Symbol.PadRight(15, ' ')}\r\nBest Buy  Price:{s.BestBidPrice.ToString().PadRight(15, ' ')}Best Buy  Quantity:{s.BestBidQuantity.ToString().PadRight(15, ' ')}\r\nBest Sell Price:{s.BestAskPrice.ToString().PadRight(15, ' ')}Best Sell Quantity:{s.BestAskQuantity.ToString().PadRight(15, ' ')}").Take(100)) + "\r\netc......"
            );
        #endregion
    }
}

//母子账户接口-未开发
static async Task TestSubAccountEndpoints()
{
    using (var mexcV3RestClient = new MexcV3RestClient())
    {
        #region 对MexcClient客户端的新实例使用新的设置(这里不设置则使用之前的默认设置）
        //使用accessKey, secretKey生成一个新的API凭证
        //ApiCredentials apiCredentials = new ApiCredentials(accessKey, secretKey);
        //当前客户端使用新生成的API凭证
        //client.SetApiCredentials(apiCredentials);
        #endregion
    }
}

//现货账户和交易接口测试-已完成（批量下单除外）
static async Task TestSpotAccountTradeEndpoints()
{
    using (var mexcV3RestClient = new MexcV3RestClient())
    {
        #region 对MexcClient客户端的新实例使用新的设置(这里不设置则使用之前的默认设置）
        //使用accessKey, secretKey生成一个新的API凭证
        //ApiCredentials apiCredentials = new ApiCredentials(accessKey, secretKey);
        //当前客户端使用新生成的API凭证
        //client.SetApiCredentials(apiCredentials);
        #endregion

        #region 现货测试下单
        Console.WriteLine("现货测试下单");
        await HandleRequest("Test New Order", () => mexcV3RestClient.SpotApi.SpotAccountTrade.PlaceTestOrderAsync(
            symbol: "BTCUSDT",
            side: OrderSide.Buy,
            type: SpotOrderType.Limit,
            quantity: (decimal)1,
            quoteQuantity: null,
            price: (decimal)200,
            newClientOrderId: $"TestOrder{DateTimeConverter.ConvertToMilliseconds(DateTime.UtcNow).ToString()}",
            receiveWindow: 60000), result => $"{(object.Equals(result, null) ? "Error !" : "Successful")}"
            );
        #endregion

        #region 现货下单
        Console.WriteLine("现货下单");
        string testClientOrderId = $"NewOrder{DateTimeConverter.ConvertToMilliseconds(DateTime.UtcNow).ToString()}";
        await HandleRequest("New Order", () => mexcV3RestClient.SpotApi.SpotAccountTrade.PlaceOrderAsync(
            symbol: "USDCUSDT",
            side: OrderSide.Buy,
            type: SpotOrderType.Limit,
            quantity: (decimal)10,
            quoteQuantity: null,
            price: (decimal)0.9,
            newClientOrderId: $"{testClientOrderId}",
            receiveWindow: 60000), result => $"{(object.Equals(result.ErrorCode, null) ? $"{result.Symbol} {result.OrderId} {result.ClientOrderId} {result.Quantity} {result.Price}" : $"{result.ErrorCode} {result.ErrorMessage}")}"
            );
        #endregion

        #region 撤销订单
        Console.WriteLine("撤销订单");
        await HandleRequest("Cancel an active order", () => mexcV3RestClient.SpotApi.SpotAccountTrade.CancelOrderAsync(
            symbol: "USDCUSDT",
            orderId: string.Empty,                              //被撤销订单编号
            origClientOrderId: $"{testClientOrderId}",          //被撤销订单的用户自定义单号
            newClientOrderId: string.Empty,                     //撤销操作的用户自定义单号
            receiveWindow: 60000), 
            result => $"{(object.Equals(result.ErrorCode, null) ? $"{result.Symbol} {result.OrderId} {result.Quantity} {result.Price}" : $"{result.ErrorCode} {result.ErrorMessage}")}"
            );
        #endregion

        #region 撤销单一交易对所有订单
        Console.WriteLine("撤销单一交易对所有订单");
        await HandleRequest("Cancel a single symbol all pending orders", () => mexcV3RestClient.SpotApi.SpotAccountTrade.CancelOpenOrdersAsync(
            symbol: "USDCUSDT",                                   //交易代码
            receiveWindow: 60000), 
            result => string.Join("", result.Select(s => $"\r\n{s.OrderId.ToString().PadRight(14, ' ')} 状态:{s.Status.ToString().PadRight(10, ' ')}").Take(50)) + "\r\n......"
            );
        #endregion

        #region 查询订单 查询指定订单状态功能
        Console.WriteLine("查询订单");
        await HandleRequest("Check an order's status", () => mexcV3RestClient.SpotApi.SpotAccountTrade.GetOrderAsync(
            symbol: "USDCUSDT",                                 //交易代码
            orderId: string.Empty,                              //要查询的订单编号
            origClientOrderId: $"{testClientOrderId}",          //要查询的订单的用户自定义单号
            receiveWindow: 60000), 
            result => $"{(object.Equals(result.ErrorCode, null) ? $"{result.Symbol} {result.OrderId} {result.Quantity} {result.Price}" : $"{result.ErrorCode} {result.ErrorMessage}")}"
            );
        #endregion
        
        #region 查询当前挂单
        Console.WriteLine("查询当前挂单");
        await HandleRequest("Get all open orders on a symbol", () => mexcV3RestClient.SpotApi.SpotAccountTrade.GetOpenOrdersAsync(
            symbol: "PIGUSDT",
            receiveWindow: 60000), 
            result => string.Join("", result.Select(s => $"\r\n{s.OrderId.ToString().PadRight(14, ' ')} 状态:{s.Status.ToString().PadRight(10, ' ')}").Take(50)) + "\r\n......"
            );
        #endregion

        #region 查询所有订单(指定交易代码，指定时间段内的所有订单）
        Console.WriteLine("查询所有订单");
        await HandleRequest("Get all account orders; active, cancelled or completed", () => mexcV3RestClient.SpotApi.SpotAccountTrade.GetOrdersAsync(
            symbol: "PIGUSDT",
            startTime: DateTime.UtcNow.AddDays(-6),
            endTime: DateTime.UtcNow,
            limit: 500,
            receiveWindow: 60000), 
            result => string.Join("", result.Select(s => $"\r\n" +
            $"OrderId:{s.OrderId.ToString().PadRight(14, ' ')}" +
            $"Status:{(s.Status != null ? s.Status.ToString().PadRight(10, ' ') : string.Empty)}").Take(50)) + 
            "\r\n......"
            );
        #endregion

        #region 账户信息
        Console.WriteLine("账户信息");
        await HandleRequest("Account Information", () => mexcV3RestClient.SpotApi.SpotAccountTrade.GetAccountInfoAsync(
            receiveWindow: 60000), 
            result => $"{(object.Equals(result.ErrorCode, null) ? $"\r\nAccount Type:{result.AccountType} Can Trade:{result.CanTrade} Can Withdraw:{result.CanWithdraw} Can Deposit:{result.CanDeposit} Update Time{result.UpdateTime}" : $"{result.Permissions.Select(s => (",", s.ToString()))}")}"
            );
        #endregion
        
        #region 获取账户指定交易对的成交历史
        Console.WriteLine("获取账户指定交易对的成交历史");
        await HandleRequest("Account Trade List", () => mexcV3RestClient.SpotApi.SpotAccountTrade.GetUserTradesAsync(
            symbol: "PIGUSDT",
            orderId: null,
            startTime: DateTime.UtcNow.AddDays(-6),
            endTime: DateTime.UtcNow,
            limit: 500,
            receiveWindow: 60000), 
            result => string.Join("", result.Select(s => $"\r\nSymbol:{s.Symbol.ToString().PadRight(12, ' ')}{s.OrderId.ToString().PadRight(33, ' ')}Price:{s.Price.ToString().PadRight(14, ' ')}Quantity:{s.Quantity.ToString().PadRight(11, ' ')}QuoteQuantity:{s.QuoteQuantity.ToString().PadRight(14, ' ')}").Take(50)) + "\r\n......"
            );
        #endregion

        #region 未实现功能
        #region 现货批量下单（支持单次批量下20单,限频2次/秒）
        //测试结构1
        //PlacedOrder placedOrderA = new PlacedOrder
        //{
        //    Symbol = "USDCUSDT",
        //    Side = OrderSide.Buy,
        //    Type = SpotOrderType.Limit,
        //    Quantity = 12,
        //    QuoteQuantity = null,
        //    Price = (decimal)0.9,
        //    ClientOrderId = $"BatchOrder{DateTimeConverter.ConvertToMilliseconds(DateTime.UtcNow).ToString()}"
        //};
        //PlacedOrder placedOrderB = new PlacedOrder
        //{
        //    Symbol = "USDCUSDT",
        //    Side = OrderSide.Buy,
        //    Type = SpotOrderType.Limit,
        //    Quantity = 10,
        //    QuoteQuantity = null,
        //    Price = (decimal)0.8,
        //    ClientOrderId = $"BatchOrder{DateTimeConverter.ConvertToMilliseconds(DateTime.UtcNow).ToString()}"
        //};


        //MexcV3BatchPlacedOrderRequest mexcV3BatchPlacedOrderRequest = new MexcV3BatchPlacedOrderRequest();
        //mexcV3BatchPlacedOrderRequest.placedOrderList = new List<PlacedOrder>() { placedOrderA, placedOrderB };

        //await HandleRequest("Batch Orders", () => client.SpotApi.SpotAccountTrade.BatchPlaceOrderAsync(
        //    mexcV3BatchPlacedOrderRequest,
        //    receiveWindow: 60000), result => $"{(object.Equals(result.ErrorCode, null) ? $"{result.Symbol} {result.OrderId} {result.Quantity} {result.Price}" : $"{result.ErrorCode} {result.ErrorMessage}")}");

        //测试结构2
        //MexcV3BatchPlacedOrderRequestTest testA = new MexcV3BatchPlacedOrderRequestTest
        //{
        //    Symbol = "USDCUSDT",
        //    Side = OrderSide.Buy,
        //    Type = SpotOrderType.Limit,
        //    Quantity = 12,
        //    QuoteQuantity = null,
        //    Price = (decimal)0.9,
        //    ClientOrderId = $"BatchOrderA{DateTimeConverter.ConvertToMilliseconds(DateTime.UtcNow).ToString()}"
        //};
        //MexcV3BatchPlacedOrderRequestTest testB = new MexcV3BatchPlacedOrderRequestTest
        //{
        //    Symbol = "USDCUSDT",
        //    Side = OrderSide.Buy,
        //    Type = SpotOrderType.Limit,
        //    Quantity = 10,
        //    QuoteQuantity = null,
        //    Price = (decimal)0.8,
        //    ClientOrderId = $"BatchOrderB{DateTimeConverter.ConvertToMilliseconds(DateTime.UtcNow).ToString()}"
        //};
        //IEnumerable<MexcV3BatchPlacedOrderRequestTest> items = new List<MexcV3BatchPlacedOrderRequestTest>();
        //List<MexcV3BatchPlacedOrderRequestTest> list = items.ToList();
        //list.Add(testA);
        //list.Add(testB);
        //items = (IEnumerable<MexcV3BatchPlacedOrderRequestTest>)list;

        //string? jsontestA = JsonConvert.SerializeObject(testA);
        //string? jsontestB = JsonConvert.SerializeObject(testB);
        //List<string> strings = new List<string>();
        //strings.Add(jsontestA);
        //strings.Add(jsontestB);
        //string stringTemp = $"[{jsontestA},{jsontestB}]";
        //Dictionary<string, string>? values = JsonConvert.DeserializeObject<Dictionary<string, string>>(stringTemp);

        //mexcV3BatchPlacedOrderRequestTestList.Concat(new[] { testA, testB });
        //mexcV3BatchPlacedOrderRequestTestList.Append(testA);
        //mexcV3BatchPlacedOrderRequestTestList.Append(testB);

        //await HandleRequest("Batch Orders Test", () => client.SpotApi.SpotAccountTrade.BatchPlaceOrderTestAsync(
        //    stringTemp,
        //    receiveWindow: 60000),
        //    result => $"{(object.Equals(result.ErrorCode, null) ? $"{result.Symbol} {result.OrderId} {result.Quantity} {result.Price}" : $"{result.ErrorCode} {result.ErrorMessage}")}");
        #endregion
        #endregion
    }
}

//钱包接口测试-开发中...
static async Task TestWalletEndpoints()
{
    using (var mexcV3RestClient = new MexcV3RestClient())
    {
        #region 对MexcClient客户端的新实例使用新的设置(这里不设置则使用之前的默认设置）
        //使用accessKey, secretKey生成一个新的API凭证
        //ApiCredentials apiCredentials = new ApiCredentials(accessKey, secretKey);
        //当前客户端使用新生成的API凭证
        //client.SetApiCredentials(apiCredentials);
        #endregion

        #region 查询币种信息 返回币种详细信息以及智能合约地址
        Console.WriteLine("查询币种信息");
        await HandleRequest("Currency list", () => mexcV3RestClient.SpotApi.Wallet.GetUserAssetsAsync(),
            result => string.Join(", ", result.Select(s => $"\r\nAsset:{s.Asset.PadRight(14, ' ')}Name:{s.Name.PadRight(10, ' ')}China Info:......").Take(100)) + "\r\n......"
            );
        #endregion
    }
}

//ETF接口测试-开发中...
static async Task TestETFEndpoints()
{
    using (var mexcV3RestClient = new MexcV3RestClient())
    {
        #region 对MexcClient客户端的新实例使用新的设置(这里不设置则使用之前的默认设置）
        //使用accessKey, secretKey生成一个新的API凭证
        //ApiCredentials apiCredentials = new ApiCredentials(accessKey, secretKey);
        //当前客户端使用新生成的API凭证
        //client.SetApiCredentials(apiCredentials);
        #endregion

        #region 获取杠杆ETF基础信息(测试返回OK，但是接收返回值有问题，不知道啥情况啊！！！)
        Console.WriteLine("获取杠杆ETF基础信息");
        await HandleRequest("ETF Info", () => mexcV3RestClient.SpotApi.ExchangeTradedFunds.GetETFInfoAsync(
            //symbol: "OP3SUSDT",
            //receiveWindow: 60000
            ),
            result => string.Join(", ", result.Data.Select(s => $"\r\n")));
        //result => string.Join(", ", result.Select(s => $"\r\n" +
        //    $"ETF Symbol:{(string.IsNullOrWhiteSpace(s.Symbol) ? string.Empty : s.Symbol).PadRight(12, ' ')}" +
        //    $"Net Value:{(string.IsNullOrWhiteSpace(s.NetValue.ToString()) ? string.Empty : s.NetValue.ToString()).PadRight(12, ' ')}" +
        //    $"Fee Rate:{(string.IsNullOrWhiteSpace(s.FeeRate.ToString()) ? string.Empty : s.FeeRate.ToString()).PadRight(12, ' ')}" +
        //    $"Time Stamp:{s.Timestamp.ToString().PadRight(12, ' ')}").Take(10)) + "\r\netc......"
        #endregion
    }
}

//杠杆账户和交易接口-未开发(以下两个仅仅作为结构参考，未做相关测试)
static async Task TestMarginAccountTradeEndpoints()
{
    using (var mexcV3RestClient = new MexcV3RestClient())
    {
        #region 对MexcClient客户端的新实例使用新的设置(这里不设置则使用之前的默认设置）
        //使用accessKey, secretKey生成一个新的API凭证
        //ApiCredentials apiCredentials = new ApiCredentials(accessKey, secretKey);
        //当前客户端使用新生成的API凭证
        //client.SetApiCredentials(apiCredentials);
        #endregion

        #region 切换杠杆账户模式
        Console.WriteLine("切换杠杆账户模式");
        await HandleRequest("switch trademode of margin account", () => mexcV3RestClient.SpotApi.MarginAccountTrade.MarginTradeModeAsync(
            symbol: "USDCUSDT",
            tradeMode: 0,
            receiveWindow: 60000), result => $"{(object.Equals(result.ErrorCode, null) ? $"{result.ToString()}" : $"{result.ErrorCode} {result.ErrorMessage}")}"
            );
        #endregion

        #region 杠杆账户下单
        Console.WriteLine("杠杆账户下单");
        string testClientOrderId = $"New Margin Order {DateTimeConverter.ConvertToMilliseconds(DateTime.UtcNow).ToString()}";
        await HandleRequest("New Margin Order", () => mexcV3RestClient.SpotApi.MarginAccountTrade.MarginPlaceOrderAsync(
            symbol: "USDCUSDT",
            side: OrderSide.Buy,
            type: SpotOrderType.Limit,
            quantity: (decimal)10,
            quoteQuantity: null,
            price: (decimal)0.9,
            newClientOrderId: $"{testClientOrderId}",
            receiveWindow: 60000), result => $"{(object.Equals(result.ErrorCode, null) ? $"{result.ToString()}" : $"{result.ErrorCode} {result.ErrorMessage}")}"
            );
        #endregion
    }
}

//现货账户WebSocket账户Listen Key维护接口测试-已完成
static async Task TestSpotWebSocketAccountEndpoints()
{
    using (var client = new MexcV3RestClient())
    {
        #region 对MexcClient客户端的新实例使用新的设置(这里不设置则使用之前的默认设置）
        //使用accessKey, secretKey生成一个新的API凭证
        //ApiCredentials apiCredentials = new ApiCredentials(accessKey, secretKey);
        //当前客户端使用新生成的API凭证
        //client.SetApiCredentials(apiCredentials);
        #endregion

        #region 创建现货WebSocket 连接 Listen Key
        Console.WriteLine("创建现货WebSocket 连接 Listen Key");
        //await HandleRequest("Test Create New Listen Key", () => client.SpotApi.WebsocketAccount.StartUserStreamAsync(
        //    ), result => $"{(object.Equals(result, null) ? "Error !" : $"Successful:{result.ToString()}")}"
        //    );
        WebCallResult<string>? createResult = await client.SpotApi.WebsocketAccount.StartUserStreamAsync();
        string myListenKey = createResult.Data;
        Console.WriteLine($"New Listen Key :{(myListenKey != null ? myListenKey:"Create failed!")}");
        #endregion

        #region 延长现货WebSocket 连接 Listen Key有效期
        if (myListenKey != null)
        {
            Console.WriteLine("延长现货WebSocket 连接 Listen Key有效期");
            //await HandleRequest("Test Keep-alive Listen Key", () => client.SpotApi.WebsocketAccount.KeepAliveUserStreamAsync(
            //    listenKey: myListenKey
            //    ), result => $"{(object.Equals(result, null) ? "Error !" : $"Successful:{result.ToString()}")}"
            //    );
            WebCallResult<string>? keepAliveResult = await client.SpotApi.WebsocketAccount.KeepAliveUserStreamAsync(
                listenKey: myListenKey
                );
            string keepAliveListenKey = keepAliveResult.Data;
            Console.WriteLine($"Keep-aliv Listen Key :{(keepAliveListenKey != null ? keepAliveListenKey : "Keep-alive failed!")}");
        }
        #endregion

        #region 关闭现货WebSocket 连接中指定的 Listen Key
        if (myListenKey != null)
        {
            Console.WriteLine("关闭现货WebSocket 连接中指定的 Listen Key");
            //await HandleRequest("Test Stop Listen Key", () => client.SpotApi.WebsocketAccount.StopUserStreamAsync(
            //    listenKey: myListenKey
            //    ), result => $"{(object.Equals(result, null) ? "Error !" : $"Successful:{result.ToString()}")}"
            //    );
            WebCallResult<string>? stopResult = await client.SpotApi.WebsocketAccount.StopUserStreamAsync(
                listenKey: myListenKey
                );
            string stopListenKey = stopResult.Data;
            Console.WriteLine($"Keep-aliv Listen Key :{(stopListenKey != null ? stopListenKey : "stop failed!")}");
        }
        #endregion
    }
}

static async Task HandleRequest<T>(string action, Func<Task<WebCallResult<T>>> request, Func<T, string> outputData)
{
    Console.WriteLine("Press enter to continue");
    Console.ReadLine();
    Console.Clear();
    Console.WriteLine("Requesting " + action + " ..");
    var result = await request();
    if (result.Success)
    {
        Console.WriteLine($"{action}: " + outputData(result.Data));
    }
    else
    {
        if (result.ResponseStatusCode == System.Net.HttpStatusCode.OK && result.Error.Code == null && result.Data == null)
        {
            Console.WriteLine($"No related records found");
        }
        else
        {
            Console.WriteLine($"Failed to retrieve data: {result.Error}");            
        }
        Console.WriteLine();
    }
}

Console.ReadLine();