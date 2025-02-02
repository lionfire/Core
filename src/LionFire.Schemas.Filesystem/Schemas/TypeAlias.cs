namespace LionFire.Schemas;


/// <summary>
/// A convenient name for a Type
/// </summary>
public class TypeAlias
{

    #region Identity

    /// <summary>
    /// Namespace + real type name
    /// </summary>
    public string? Type { get; set; }

    #region Derived

    public string? Namespace => Type?.Substring(0, Type.LastIndexOf('.'));
    public string? Name => Type?.Substring(Type.LastIndexOf('.') + 1);

    #endregion

    #endregion

    #region Parameter

    public string? Alias { get; set; }

    #endregion
}


