using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LionFire.Collections;
using LionFire.Referencing;

namespace LionFire.Vos
{
    // TODO: Rethink this interface

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
