#nullable enable

namespace LionFire.UI.Workspaces
{

    // FUTURE - REVIEW - how to register for types?  Interface types vs concrete (TreatAs)?  Is this redundant?  Should concrete types provide their own serialization (yes)?

    //public interface ISerializableStrategy<TValue> { }

    public interface ISerializableStrategy
    {
        bool CanGetSerializable(object obj, object? persistenceContext = null);
        object GetSerializable(object obj, object? persistenceContext = null);
    }

    // TODO: Children Executable Visitor: attach to object, then on onstarting/onstopping, crawl thru the hierarchy
    // TODO: Init call a global executablemanager to say that an executable is intializing, to give it a chance to add state listeners

}
