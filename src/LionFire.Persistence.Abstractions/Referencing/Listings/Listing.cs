using LionFire.Results;

namespace LionFire.Referencing;

public class Listing<T>
{
    #region Construction

    public Listing() { }
    public Listing(string name, Type type = null, string mimeType = null, bool directory = false)
    {
        Name = name;
        Type = type;
        MimeType = mimeType;
        IsDirectory = directory;
    }
    //public Listing(string name, string typeName, string mimeType = null, bool directory = false)
    //{
    //    Name = name;
    //    //Type = type;
    //    MimeType = mimeType;
    //    IsDirectory = directory;
    //}
    public static implicit operator Listing<T>(string name) => new Listing<T> { Name = name };

    #endregion

    public string Name { get; private set; }

    public string MimeType { get; }
    public bool IsDirectory { get; }

    public IReference UnderlyingReference { get; set; }
    public string RawName { get; set; }

    public Type Type { get; set; }

    public Type DataType
    {
        get => dataType ?? UnwrapType(Type);
        set => dataType = value;
    }
    private Type dataType;

    public Type? UnwrapType(Type? type)
    {
        if (type == null) return null;

        if (typeof(IValueResult).IsAssignableFrom(type))
        {
            var genericType = type.GetInterfaces().Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IValueResult<>)).FirstOrDefault();
            if (genericType != null) { return genericType.GenericTypeArguments[0]; }
        }
        return type;
    }

    public override string ToString() => this.ToXamlAttribute();

}
