namespace LionFire.Schemas;

/// <summary>
/// A bundle of related
/// - Types
/// - TypeAliases
/// </summary>
public class TypeSchemaBundle
{
    public static string Namespace = "https://schemas.lionfire.ca/2025/ui";

    public string? DefaultAssembly { get; set; }

    #region Parameters

    public static Func<Type, string> DefaultAliasFactory { get; set; } = (Type type) =>
    {
        if (type.IsInterface && type.Name.StartsWith("I") && type.Name.Length >= 2 && char.IsUpper(type.Name[1]))
        {
            return type.Name.Substring(1);
        }
        return type.Name;
    };

    public Func<Type, string> GetDefaultAlias { get; set; } = DefaultAliasFactory;

    #endregion

    public TypeSchemaBundle(params Type[] types)
    {
        Types = new(types);
    }

    #region Properties

    public List<Type> Types { get; set; } = new List<Type>();
    public static List<TypeAlias>? Aliases { get; set; }

    #endregion

    #region Methods

    public void AddDefaultAliases()
    {
        Aliases ??= new();
        foreach (var type in Types)
        {
            Aliases.Add(new TypeAlias { Type = type.FullName, Alias = GetDefaultAlias(type) });
        }
    }

    #endregion
}

