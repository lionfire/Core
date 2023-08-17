namespace LionFire.Inspection;

/// <summary>
/// An open-ended implementation of ICustomObjectInspector that can be configured however the creator wishes.
/// </summary>
public class CustomObjectInspector : IInspectorObjectVM
{
    #region Relationships

    /// <inheritdoc/>
    public object Source { get; }

    #endregion

    #region Lifecycle

    public CustomObjectInspector(object sourceObject, List<INode> memberVMs)
    {
        Source = sourceObject;
        Members = memberVMs;
    }

    #endregion

    #region Data

    public List<INode> Members { get; } 

    #endregion
}