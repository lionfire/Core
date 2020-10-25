using LionFire.UI;
using System.Threading.Tasks;

namespace LionFire.Shell
{

    public interface IConductor : IPresenterContainer, ICloseable
    {

        #region Show

        Task Show(ViewInstantiation instantiation);

        #endregion
        
    }
}
