#nullable enable
using System.Collections.Generic;
using System;

namespace LionFire.UI.Workspaces
{
    public class SerializableProvider : ISerializableProvider
    {
        public List<ISerializableStrategy> Strategies { get; set; }

        public object GetSerializable(object obj, object? persistenceContext = null)
        {
            foreach (var s in Strategies)
            {
                if (s.CanGetSerializable(obj, persistenceContext))
                {
                    return s.GetSerializable(obj, persistenceContext);
                }
            }
            throw new InvalidOperationException("No strategies returned a Serializable.  Did you try CanGerSerializable first?");
        }

        public bool CanGetSerializable(object obj
            , object? persistenceContext = null)
        {
            foreach (var s in Strategies)
            {
                if (s.CanGetSerializable(obj, persistenceContext)) return true;
            }
            return false;
        }
    }

    // TODO: Children Executable Visitor: attach to object, then on onstarting/onstopping, crawl thru the hierarchy
    // TODO: Init call a global executablemanager to say that an executable is intializing, to give it a chance to add state listeners

}
