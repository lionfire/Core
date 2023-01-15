using System.Collections.Generic;
using LionFire.Referencing;
using LionFire.Serialization;

namespace LionFire.Persistence.Filesystem;

public static class StringUtilsX // MOVE
{
    public static string TrimStartString(this string path, string potentialPrefix)
    {
        if (path.StartsWith(potentialPrefix))
        {
            path = path.Substring(potentialPrefix.Length);
        }
        return path;
    }
}

public interface IFileReference : IReference { }
public interface IFileReference<out TValue> : IFileReference
{
}

public class FileReference : FileReference<object>, IReferencable<FileReference>
{
    public FileReference Reference => this;

    public FileReference<T> ForType<T>() => new FileReference<T>(Path);

    public FileReference() { }

    /// <summary>
    /// (Does not support URIs (TODO))
    /// </summary>
    /// <param name="path"></param>
    public FileReference(string path) : base(CleanPath(path))
    {
    }

    public static string CleanPath(string path)
    {
        path = path.TrimStartString("file:");
        return path;
    }

    public static implicit operator FileReference(string path) => ToReference(path);
    public static new FileReference ToReference(string path) => new FileReference(path);

}

[LionSerializable(SerializeMethod.ByValue)]
public class FileReference<TValue> : FileReferenceBase<FileReference<TValue>, TValue>, IFileReference<TValue>
{
    
    #region Scheme

    public static class Constants
    {
        public const string UriScheme = "file";
    }
    public override string Scheme => Constants.UriScheme;
    public override string UriPrefixDefault => "file:///";
    public override string UriSchemeColon => "file:";

    public static readonly IEnumerable<string> UriSchemes = new string[] { Constants.UriScheme };

    #endregion

    #region Construction and Implicit Construction

    public FileReference() { }

    /// <summary>
    /// (Does not support URIs (TODO))
    /// </summary>
    /// <param name="path"></param>
    public FileReference(string path) : base(path)
    {
    }

    //public LocalFileReference(IReference reference)
    //{
    //    ValidateCanConvertFrom(reference);
    //    CopyFrom(reference);
    //}

    public static implicit operator FileReference<TValue>(string path) => ToReference(path);

    public static FileReference<TValue> ToReference(string path) => new FileReference<TValue>(path);

    public static FileReference<TValue> ReferenceFromKey(string path) => ToReference(path);

    public static string ToReferenceKey(string path) => path;

    #endregion

    #region Conversion

    public static void ValidateCanConvertFrom(IReference reference)
    {

        if (reference.Scheme != Constants.UriScheme)
        {
            throw new ReferenceException("UriScheme not supported");
        }
    }

    public static FileReference ConvertFrom(IReference parent)
    {
        var fileRef = parent as FileReference;

        if (fileRef == null && parent.Scheme == Constants.UriScheme)
        {
            fileRef = new FileReference(parent.Path);
        }

        return fileRef;
    }

    #endregion

}

public static class FileReferenceExtensions
{
    public static FileReference ToFileReference(this string path) => new FileReference(path);
}
