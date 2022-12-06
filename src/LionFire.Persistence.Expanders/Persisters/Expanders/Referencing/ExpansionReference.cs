using LionFire.ExtensionMethods.Parsing;
using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Persisters.Expanders;


/// <remarks>
/// Path refers to the SubPath within the Target, which can be found at the SourceKey
/// </remarks>
public interface IExpansionReference : IReference
{
    /// <summary>
    /// Key for a Reference that points to the Source to look within
    /// </summary>
    public string SourceKey { get; }
}

/// <remarks>
/// Path refers to the SubPath within the Target, which can be found at the SourceKey
/// </remarks>
public interface IExpansionReference<TValue> : IReference<TValue>, IExpansionReference
{
}

public interface IParses<TInput, TOutput>
{
#if NET7_0_OR_GREATER
    abstract static TOutput? TryParse(TInput input);
#else
    static TOutput? TryParse(TInput input) => throw new NotImplementedException();
#endif
}

public class ExpansionReference : ExpansionReference<object>, IParses<string, ExpansionReference>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key">expand: scheme must be stripped, unless it indicates a nested expansion.  {targetScheme}:{restOfTargetUrl}:/{subpath}</param>
    public ExpansionReference(string key) : base(key)
    {
    }

}

/// <summary>
/// 
/// </summary>
/// <remarks>
/// Path refers to the SubPath within the Target, which can be found at the SourceKey
/// </remarks>
/// <typeparam name="TValue"></typeparam>
public class ExpansionReference<TValue> : ReferenceBase<ExpansionReference<TValue>>, IExpansionReference<TValue>
{
    #region Constants

    public const string DefaultScheme = "expand";
    public const string DefaultSchemePrefix = DefaultScheme + ":";
    public const string ExpansionDelimiter = ":";
    public const char ExpansionDelimiterChar = ':';

    public override string Scheme => DefaultScheme;

    #endregion

    #region Construction

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key">expand: scheme must be stripped, unless it indicates a nested expansion.  {targetScheme}:{restOfTargetUrl}:/{subpath}</param>
    public ExpansionReference(string key) { this.Key = key; }
    public ExpansionReference(string sourceKey, string targetPath) { this.Components = (sourceKey, targetPath); }

    public static (string sourceUri, string targetPath)? TryParseIntoComponents(string x)
    {
        if (!DefaultSchemePrefix.TryStripPrefix(ref x)) return null;

        var expansionDelimiterIndex = x.LastIndexOf(ExpansionDelimiterChar);

        bool detectedEscapedExpansionDelimiters = false;
        while (expansionDelimiterIndex > 0 && x[expansionDelimiterIndex - 1] == ExpansionDelimiterChar)
        {
            detectedEscapedExpansionDelimiters = true;
            expansionDelimiterIndex = x.Substring(0, expansionDelimiterIndex - 2).LastIndexOf(ExpansionDelimiterChar);
        }

        if (expansionDelimiterIndex == -1) return null;

        var path = x.Substring(expansionDelimiterIndex + 1);
        if (detectedEscapedExpansionDelimiters)
        {
            path = path.Replace($"{ExpansionDelimiterChar}{ExpansionDelimiterChar}", ExpansionDelimiter);
        }
        return (x.Substring(0, expansionDelimiterIndex), path);
    }

    public static ExpansionReference<TValue> Parse(string x) => TryParse(x) ?? throw new ArgumentException(InvalidFormatErrorMessage);

    public static ExpansionReference<TValue>? TryParse(string x)
    {
        var result = TryParseIntoComponents(x);
        if (result == null) return null;
        else return new ExpansionReference<TValue>(result.Value.sourceUri, result.Value.targetPath);
    }

    #endregion

    #region IExpansionReference

    /// <summary>
    /// Key for (IReference) Source
    /// </summary>
    public string SourceKey => Components.SourceUri;

    #endregion

    public override Type Type => typeof(TValue);

    public override IEnumerable<string> AllowedSchemes { get { yield return DefaultScheme; } }

    public const string InvalidFormatErrorMessage = "Must be in the format: expand:<SourceUrl>:<TargetPath>";
    public override string Key
    {
        get => SourceKey + ExpansionDelimiter + Path;
        protected set
        {
            var result = TryParseIntoComponents(value);
            if (result == null) throw new ArgumentException();

            Components = (result.Value.sourceUri, result.Value.targetPath);
        }
    }

    public override string Path
    {
        get => Components.Path;
        protected set => throw new NotSupportedException();
    }
    public (string SourceUri, string Path) Components { get; private set; }


    protected override void InternalSetPath(string path)
    {
        throw new NotSupportedException();
    }
}
