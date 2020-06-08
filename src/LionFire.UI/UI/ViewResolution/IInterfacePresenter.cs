using System.Collections.Generic;
using System.Threading.Tasks;

namespace LionFire.UI
{
    // Compatible with Caliburn.Micro Bootstrapper

    public interface IInterfacePresenter
    {
        Task DisplayRootViewFor<TViewModel>(IDictionary<string, object> settings = null);
    }
}
