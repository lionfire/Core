#nullable enable
using LionFire.Persistence;
using LionFire.Results;

namespace LionFire.Referencing;

public static class Listing
{
    public static bool IsListingType(Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Metadata<>))
        {
            var metadataInnerType = type.GetGenericArguments()[0];
            return metadataInnerType.IsGenericType && metadataInnerType.GetGenericTypeDefinition() == typeof(Listing<>);
        }
        return false;
    }

    public static Type? GetListingType<TValue>()
    {
        if (typeof(TValue).IsGenericType)
        {
            var genericType = typeof(TValue).GetGenericTypeDefinition();
            if (genericType == typeof(Metadata<>))
            {
                var metadataType = typeof(TValue).GetGenericArguments()[0];
                if (metadataType.IsGenericType && metadataType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    var enumerableType = metadataType.GetGenericArguments()[0];
                    if (enumerableType.IsGenericType && enumerableType.GetGenericTypeDefinition() == typeof(Listing<>))
                    {
                        var listingType = enumerableType.GetGenericArguments()[0];
                        return listingType;
                    }
                }
            }
        }
        return null;
    }
}

// REVIEW: Consider replacing Listing<T> with Listing, and doing type filtering another way
public class Listing<T>
{
    #region Construction

    public Listing() { }
    public Listing(string name)
    {
        Name = name;
    }
    public Listing(string name, Type type, string mimeType = null, bool directory = false) : this(name)
    {
        Type = type;
        MimeType = mimeType;
        IsDirectory = directory;
    }

    public static Listing<T> CreateDirectoryListing(string name) => new Listing<T>(name, type: null, directory: true);

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
    public bool IsDirectory { get; set; }

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
