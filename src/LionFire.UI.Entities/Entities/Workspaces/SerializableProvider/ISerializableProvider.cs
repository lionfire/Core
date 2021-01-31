#nullable enable
using System.Collections.Generic;

namespace LionFire.UI.Workspaces
{
    public interface ISerializableProvider : ISerializableStrategy
    {
        public List<ISerializableStrategy> Strategies { get; set; }
    }

    // TODO: Children Executable Visitor: attach to object, then on onstarting/onstopping, crawl thru the hierarchy
    // TODO: Init call a global executablemanager to say that an executable is intializing, to give it a chance to add state listeners

}
