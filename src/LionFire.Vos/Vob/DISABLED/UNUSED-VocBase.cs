#if false
using LionFire.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Vos
{
    ///// <summary>
    ///// Why is there a base class?
    ///// </summary>
    //public abstract class VocBase : IVoc
    //{
    #region CRUD

    #region Retrieve

    public abstract bool TryRetrieve();

    #endregion

    #region Move / Delete Methods

    public abstract void Move(Vob vobDestination);
    public abstract void Rename(string newName);
    public abstract void Delete();

    #endregion

    #endregion
    //}

}
#endif