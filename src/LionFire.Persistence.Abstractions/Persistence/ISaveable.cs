using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Persistence
{
    // Not sure I like this for next-gen.  Remove preview.  Rename to Put

    public interface ISaveable
    {
        //void Save(bool allowDelete = false, bool preview = false);
        Task Save(bool allowDelete = false);
    }
}
