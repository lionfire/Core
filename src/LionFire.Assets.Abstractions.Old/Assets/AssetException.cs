using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.Serialization;

namespace LionFire.Assets
{
    public class AssetException : Exception
    {
        public AssetException() { }
        public AssetException(string message) : base(message) { }
        public AssetException(string message, Exception inner) : base(message, inner) { }
     
    }
}
