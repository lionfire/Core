using System;
//using System.Windows.Threading;
#if NET35
using ManualResetEventSlim = System.Threading.ManualResetEvent; // REVIEW
#endif

namespace LionFire.Alerting
{
    [Flags]
    public enum AlertFlags
    {
        None = 0,
        Nonmodal = 1 << 0,
        Modal = 1 << 1,
        MustAcknowledge = 1 << 2,
        InternalError = 1 << 3,
        RecommendQuit = 1 << 4,
        GameTip = 1 << 5,
        TextEntry = 1 << 6,
        CanMinimize = 1 << 7,
    }

}
