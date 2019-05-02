#if FUTURE
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.LazyLoading
{
    public class LazyLoadingSetting
    {
        /// <summary>
        /// If true, use this for the lifetime of the requestor, which may represent an instance, or an entire class.
        /// </summary>
        public bool Permanent { get; set; }

        public bool LazyLoad { get; set; }
    }
}
#endif