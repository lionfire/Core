using LionFire.Blazor.Components;
using LionFire.Vos;
using System.Collections.Generic;

namespace LionFire.Hosting
{
    public class ObjectExplorerRoots : IObjectExplorerRoots 
    {
        IReadOnlyDictionary<string, object> IObjectExplorerRoots.Roots => roots;
        protected Dictionary<string, object> roots = new();

        public ObjectExplorerRoots(IVos vos)
        {
            roots.Add("vos", vos);
        }
    }
}
