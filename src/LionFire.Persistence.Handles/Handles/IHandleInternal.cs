namespace LionFire.Persistence.Handles
{
    internal interface IHandleInternal<TValue> : IPersists
    {
        TValue ProtectedValue { get; set; }
        new PersistenceFlags Flags { set; }
        bool HasValue { get; }


    }
}
