# Mexc.Net_SpotApi_V3
[![.NET](https://github.com/wecando168/Mexc.Net_SpotApi_V3/actions/workflows/dotnet.yml/badge.svg)](https://github.com/wecando168/Mexc.Net_SpotApi_V3/actions/workflows/dotnet.yml) ![Nuget version](https://img.shields.io/nuget/v/Mexc.net.svg)  ![Nuget downloads](https://img.shields.io/nuget/dt/Mexc.Net.svg)

Mexc.Net is a wrapper around the Mexc API as described on [Mexc](https://github.com/mxcdevelop/APIDoc), including all features the API provides using clear and readable objects. The library support the spot, (isolated) margin and futures API's, both the REST and websocket API's.

**If you think something is broken, something is missing or have any questions, please open an [Issue](https://github.com/wecando168/Mexc.Net_SpotApi_V3/issues)**

[Documentation](https://github.com/wecando168/Mexc.Net_SpotApi_V3)

## Support the project
I develop and maintain this package on my own for free in my spare time, any support is greatly appreciated.

### Referral link
Sign up using the following referral link to pay a small percentage of the trading fees you pay to support the project instead of paying them straight to Mexc. This doesn't cost you a thing!
[Link](https://www.mexc.com/register?inviteCode=157Hw)

### Donate
Make a one time donation in a crypto currency of your choice.
No matter what cryptocurrency you use, you can donate through the following public chain addresses

**Bsc/Heco/Eth**:  0x23ab66ec4c03c9305d1c5dcbb889fb0efbbb0e44 

## Release notes
* Version 1.0.0 - 27 Sept 2022
    * Suppot mexc v3 rest client endpoints:
        * SpotApi.MarketData.PingAsync
        * SpotApi.MarketData.GetServerTimeStampAsync
        * SpotApi.MarketData.GetServerTimeAsync
        * SpotApi.MarketData.GetExchangeInfoAsync
        * SpotApi.MarketData.GetOrderBookAsync
        * SpotApi.MarketData.GetRecentTradesAsync
        * SpotApi.MarketData.GetTradeHistoryAsync
        * SpotApi.MarketData.GetAggregatedTradeHistoryAsync
        * SpotApi.MarketData.GetKlinesAsync
        * SpotApi.MarketData.GetCurrentAvgPriceAsync
        * SpotApi.MarketData.GetTickerAsync
        * SpotApi.MarketData.GetTickersAsync
        * SpotApi.MarketData.GetPriceAsync
        * SpotApi.MarketData.GetPricesAsync
        * SpotApi.MarketData.GetBookPriceAsync
        * SpotApi.MarketData.GetBookPricesAsync

        * SpotApi.SpotAccountTrade.PlaceTestOrderAsync
        * SpotApi.SpotAccountTrade.PlaceOrderAsync
        * SpotApi.SpotAccountTrade.CancelOrderAsync
        * SpotApi.SpotAccountTrade.CancelOpenOrdersAsync
        * SpotApi.SpotAccountTrade.GetOrderAsync
        * SpotApi.SpotAccountTrade.GetOpenOrdersAsync
        * SpotApi.SpotAccountTrade.SpotAccountTrade
        * SpotApi.SpotAccountTrade.GetAccountInfoAsync
        * SpotApi.SpotAccountTrade.GetUserTradesAsync
   
        * SpotApi.WebsocketAccount.StartUserStreamAsync
        * SpotApi.WebsocketAccount.KeepAliveUserStreamAsync
        * SpotApi.WebsocketAccount.StopUserStreamAsync

    * Suppot mexc v3 socket client endpoints: 
        * SpotPublicStreams.SubscribeToTradeUpdatesAsync
        * SpotPublicStreams.SubscribeToKlineUpdatesAsync
        * SpotPublicStreams.SubscribeToDiffDepthUpdatesAsync

        * SpotPrivateStreams.SubscribeToPrivateDealsUpdatesAsync
        * SpotPrivateStreams.SubscribeToPrivateOrdersUpdatesAsync