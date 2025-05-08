namespace LionFire.Validation
{
    public enum ValidationKind
    {
        Unspecified,
        Deserialized = 1,

    }

    // ENH Ideas:
    // Validate(ExecutionState.Starting)
    // Looks on class for [RequiredForState(ExecutionState.Starting)]
    //  -- also traverses properties on child object [ChildrenRequiredForState(ExecutionState.Starting)]
}
