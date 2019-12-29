using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LionFire.Collections;
using LionFire.Referencing;

namespace LionFire.Vos
{
#if DISABLED
    /// <remarks>
    /// "Vob Handle Collection" - maybe change the name or combine with IVoc
    /// Maybe make this a Dictionary&lt;name,VobReadHandle`T&gt;
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public interface IVohac<T>
        : INotifyingList<VobReadHandle<T>>
        where T : class, new()
    {
        VobReadHandle<T> this[string name] { get; }
        void RefreshCollection();

        //IEnumerable<string> ChildPaths { get; } // OLD Seems redundant. Why did I have this?
        IEnumerable<string> Names { get; } // RENAME Keys?
        IEnumerable<VobReadHandle<T>> Handles { get; } // RENAME Values?

        
    }
#endif

    public interface IVoc
    {
        Vob Vob { get; set; }

        bool? AutoLoad { get; set; }
        bool? AutoSave { get; set; }

#region Move / Delete Methods

        void Move(Vob vobDestination);
        void Rename(string newName);
        void Delete();

#endregion

    }

}
