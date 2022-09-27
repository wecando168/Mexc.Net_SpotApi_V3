using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Mexc.Net.Converters;
using Mexc.Net.Enums;
using Mexc.Net.Interfaces.Clients.GeneralApi;
using CryptoExchange.Net;
using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Objects;
using Newtonsoft.Json;

namespace Mexc.Net.Clients.GeneralApi
{
    /// <inheritdoc />
    public class MexcV3ClientGeneralApiSubAccount : IMexcV3ClientGeneralApiSubAccount
    {
        private const string subAccountListEndpoint = "sub-account/list";
        private const string subAccountTransferHistoryEndpoint = "sub-account/sub/transfer/history";
        private const string transferSubAccountEndpoint = "sub-account/universalTransfer";
        private const string queryUniversalTransferHistoryEndpoint = "sub-account/universalTransfer";
        private const string subAccountStatusEndpoint = "sub-account/status";
        private const string subAccountAssetsEndpoint = "sub-account/assets";

        private const string subAccountDepositAddressEndpoint = "capital/deposit/subAddress";
        private const string subAccountDepositHistoryEndpoint = "capital/deposit/subHisrec";


        private const string subAccountEnableMarginEndpoint = "sub-account/margin/enable";
        private const string subAccountMarginDetailsEndpoint = "sub-account/margin/account";
        private const string subAccountMarginSummaryEndpoint = "sub-account/margin/accountSummary";
        private const string subAccountTransferMarginSpotEndpoint = "sub-account/margin/transfer";

        private const string subAccountEnableFuturesEndpoint = "sub-account/futures/enable";
        private const string subAccountFuturesDetailsEndpoint = "sub-account/futures/account";
        private const string subAccountFuturesSummaryEndpoint = "sub-account/futures/accountSummary";
        private const string subAccountTransferFuturesSpotEndpoint = "sub-account/futures/transfer";
        private const string subAccountFuturesPositionRiskEndpoint = "sub-account/futures/positionRisk";

        private const string subAccountTransferToSubEndpoint = "sub-account/transfer/subToSub";
        private const string subAccountTransferToMasterEndpoint = "sub-account/transfer/subToMaster";
        private const string subAccountTransferHistorySubAccountEndpoint = "sub-account/transfer/subUserHistory";

        private const string subAccountSpotSummaryEndpoint = "sub-account/spotSummary";

        private const string subAccountCreateVirtualEndpoint = "sub-account/virtualSubAccount";
        private const string subAccountEnableBlvtEndpoint = "sub-account/blvt/enable";

        private const string toggleIpRestrictionEndpoint = "account/apiRestrictions/ipRestriction";
        private const string addIpRestrictionListEndpoint = "account/apiRestrictions/ipRestriction/ipList";
        private const string getIpRestrictionListEndpoint = "account/apiRestrictions/ipRestriction";
        private const string removeIpRestrictionListEndpoint = "account/apiRestrictions/ipRestriction/ipList";

        private readonly MexcV3ClientGeneralApi _baseClient;

        internal MexcV3ClientGeneralApiSubAccount(MexcV3ClientGeneralApi baseClient)
        {
            _baseClient = baseClient;
        }
    }
}
