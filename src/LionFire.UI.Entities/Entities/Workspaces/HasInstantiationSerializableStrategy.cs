#nullable enable
using LionFire.Instantiating;
using LionFire.Ontology;

namespace LionFire.UI.Workspaces
{
    public class HasInstantiationSerializableStrategy : ISerializableStrategy
    {
        public bool CanGetSerializable(object obj, object? persistenceContext = null) => obj is IHas<IInstantiation>;
        public object GetSerializable(object obj, object? persistenceContext = null)
            => ((IHas<IInstantiation>)obj).Object;
    }

    // TODO: Children Executable Visitor: attach to object, then on onstarting/onstopping, crawl thru the hierarchy
    // TODO: Init call a global executablemanager to say that an executable is intializing, to give it a chance to add state listeners

}
