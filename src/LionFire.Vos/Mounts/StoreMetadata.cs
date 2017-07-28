using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.ObjectBus
{
    public class StoreMetadata
    {
        public static readonly string DefaultName = "(Store)";
        public string Description { get; set; }

        public string User { get; set; }
    }
}
