using System;

namespace Mexc.Net.Interfaces.Clients.GeneralApi
{
    /// <summary>
    /// Mexc general API endpoints
    /// </summary>
    public interface IMexcV3ClientGeneralApi : IDisposable
    {
        /// <summary>
        /// Endpoints related to requesting data for and controlling sub accounts
        /// </summary>
        public IMexcV3ClientGeneralApiSubAccount SubAccount { get; }
    }
}
