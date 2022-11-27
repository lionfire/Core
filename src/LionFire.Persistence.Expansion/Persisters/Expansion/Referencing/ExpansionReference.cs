using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Persisters.Expansion;


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

public class ExpansionReference : ExpansionReference<object>
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

    #endregion

    #region IExpansionReference

    /// <summary>
    /// Key for (IReference) Source
    /// </summary>
    public string SourceKey { get; protected set; }

    #endregion

    public override Type Type => typeof(TValue);

    public override IEnumerable<string> AllowedSchemes { get { yield return DefaultScheme; } }

    public override string Key
    {
        get => SourceKey + ExpansionDelimiter + Path;
        protected set
        {
            var expansionSubPathIndex = value.LastIndexOf(ExpansionDelimiter);
            if (expansionSubPathIndex == -1)
            {
                SourceKey = value;
                Path = LionPath.PathDelimiter.ToString();
            }
            else
            {
                SourceKey = value.Substring(0, expansionSubPathIndex);
                Path = value.Substring(expansionSubPathIndex + 1);
            }
        }
    }

    public override string Path { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }

    protected override void InternalSetPath(string path)
    {
        throw new NotSupportedException();
    }
}
