#if NET35
using ManualResetEventSlim = System.Threading.ManualResetEvent; // REVIEW
#endif

namespace LionFire.Alerting
{
    public class DialogResult
    {
        //public bool IsOk { get; set; }
        //public bool IsCanceled { get; set; }

        public string TextEntry { get; set; }
    }

}
