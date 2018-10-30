using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Vos
{
    public class VocMetadata
    {
        public Type DefaultType;

        /// <summary>
        /// If not set, only type allowed is DefaultType
        /// </summary>
        public List<Type> TypesAllowed;

    }
}
