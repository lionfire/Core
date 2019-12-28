using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.MultiTyping;
using LionFire.Referencing;
using LionFire.Types;

namespace LionFire.Vos
{
    public interface IVob :
        //IReadOnlyMultiTyped, 
        IReferencable
    {
        string Name { get; }
        IVob Parent { get; }

        #region Cache

        string Path { get; }

        #endregion

        T GetNext<T>(bool skipOwn = false);
        T GetOwn<T>() where T : class;

    }
}
