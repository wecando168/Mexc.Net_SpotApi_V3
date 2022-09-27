namespace Mexc.Net.Enums
{
    /// <summary>
    /// V3 Spot Socket Account Order Type
    /// </summary>
    public enum MexcV3SpotSocketAccountOrderMakerOrTaker
    {
        /// <summary>
        /// 主动成交(1)
        /// </summary>
        isMaker = 1,

        /// <summary>
        /// 仅挂单，等待被动成交(0)
        /// </summary>
        isTaker = 0
    }
}
