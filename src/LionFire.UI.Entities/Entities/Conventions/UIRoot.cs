using LionFire.Collections;
using Microsoft.Extensions.Options;
using System.Linq;

namespace LionFire.UI.Entities
{
    public class UIRoot : UICollection, IUIRoot, IHasPathCache<string, IUIKeyed>
    {
        #region Dependencies

        public IOptionsMonitor<UIOptions> UIOptionsMonitor { get; }
        public UIOptions Options => UIOptionsMonitor.CurrentValue;

        #endregion

        public UIRoot(IOptionsMonitor<UIOptions> uiOptionsMonitor)
        {
            key = "";
            PathCache = new Collections.ConcurrentDictionaryCache<string, IUIKeyed>(p => this.QuerySubPath(p));

            UIOptionsMonitor = uiOptionsMonitor;
        }

        public ConcurrentDictionaryCache<string, IUIKeyed> PathCache { get; }


        public bool Active => this.Children.OfType<IActivated>().Where(a => a.Active).Any();
    }
}
