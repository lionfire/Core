using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Instantiating
{
    public interface IAffector<in AffectContextType> 
        where AffectContextType : class
    {
        object Affect(object obj, AffectContextType context = null);
    }
}
