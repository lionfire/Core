using System;
#if NET35
using ManualResetEventSlim = System.Threading.ManualResetEvent; // REVIEW
#endif

namespace LionFire.Alerting
{
    public class AlertButton
    {
        public string Text { get; set; }
        public Action OnClicked { get; set; }


        public AlertButton() { }
        public AlertButton(string text, Action onClicked = null) { this.Text = text; this.OnClicked = onClicked; }
    }

}
