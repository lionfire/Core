namespace LionFire.Mvvm.ObjectInspection;

/// <summary>
/// An open-ended implementation of ICustomObjectInspector that can be configured however the creator wishes.
/// </summary>
public class CustomObjectInspector : ICustomObjectInspector
{
    #region Relationships

    /// <inheritdoc/>
    public object Source { get; }

    #endregion

    #region Lifecycle

    public CustomObjectInspector(object sourceObject, List<IMemberVM> memberVMs)
    {
        Source = sourceObject;
        MemberVMs = memberVMs;
    }

    #endregion

    #region Data

    public List<IMemberVM> MemberVMs { get; } 

    #endregion
}