using System.Linq;
using System.Threading.Tasks;

namespace LionFire.UI.Entities
{
    // REVIEW - not sure I like this class and the event handlers in PresenterEvents

    public class Presenter<TChildren> : UICollection<TChildren>, IPresenter
        where TChildren : IUIKeyed
    {
        
        public virtual void Show(string key) => PresenterEvents.OnShowing(Children[key]);
        public virtual void Hide(string key) => PresenterEvents.OnHiding(Children[key]);

    }
}
