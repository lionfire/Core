using LionFire.Persistence.Filesystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Persistence.LiteFs.Referencing
{
    public class LiteFsReference<TValue> : FileReferenceBase<LiteFsReference<TValue>, TValue>
        where TValue : class
    {
        #region Scheme

        public override string Scheme => "litefs";

        #endregion

    }
}
