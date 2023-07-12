namespace LionFire.Persistence.Handles;

internal interface IHandleInternal<TValue> : IPersistsInternal
{
    //TValue ProtectedValue { get; set; } // Use IStagesValue<TValue>.StagedValue instea
    new PersistenceFlags Flags { set; } // Use IPersistsInternal.Flags instead
    bool HasValue { get; } // Use IStagesValue<TValue>.StagedValue or ILazilyGets.HasValue
}
