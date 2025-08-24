using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.FlexObjects
{
    public class FlexOptions
    {
        public Type? SingleType { get; set; }
        public bool IsSingleType => SingleType != null;
    }
}
