using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Blazor.ObjectExplorer
{
    public interface IObjectExplorerRoots
    {
        IReadOnlyDictionary<string, object> Roots { get; } 
    }
    
}
