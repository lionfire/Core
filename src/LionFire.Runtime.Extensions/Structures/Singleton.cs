using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Structures
{
   // Derived from http://csharpindepth.com/Articles/General/Singleton.aspx implementation #6

    public sealed class Singleton<T>
        where T: new()
    {
    
        public static T Instance { get { return lazyInstance.Value; } }
        private static readonly Lazy<T> lazyInstance = new Lazy<T>(() => new T());

        private Singleton()
        {
        }
    }

    public sealed class ManualSingleton<T>
    {

        public static T Instance { get; set; }
        
    }

    
}
