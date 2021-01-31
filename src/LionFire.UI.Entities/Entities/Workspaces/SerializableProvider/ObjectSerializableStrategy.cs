#nullable enable

namespace LionFire.UI.Workspaces
{
    public class ObjectSerializableStrategy : ISerializableStrategy
    {
        public bool CanGetSerializable(object obj, object? persistenceContext = null) => true; // TODO: false if the LionFire [Ignore] attribute is relevant to current context
        public object GetSerializable(object obj, object? persistenceContext = null) => obj;
    }

    // TODO: Children Executable Visitor: attach to object, then on onstarting/onstopping, crawl thru the hierarchy
    // TODO: Init call a global executablemanager to say that an executable is intializing, to give it a chance to add state listeners

}
