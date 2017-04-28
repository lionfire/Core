using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.ObjectBus
{
    // FUTURE

    public class OBusContext
    {
        public static OBusContext Current {
            get; set;
        } = new OBusContext();

        /// <summary>
        /// If true, automatically manage file extensions (or extensions in filesystem-like data stores) when saving and loading depending on the serializer that gets used.  If false, file names will exactly match the name specified by the user.
        /// </summary>
        public bool AutoFileExtensions = true;

    }
}
