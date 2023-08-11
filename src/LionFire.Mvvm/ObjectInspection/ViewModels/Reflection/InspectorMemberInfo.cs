using LionFire.IO;
using LionFire.Metadata;
using System.Reflection;

namespace LionFire.Mvvm.ObjectInspection;

public abstract class InspectorMemberInfo : IInspectorMemberInfo
{
    #region Relationships

    public abstract string Name { get; }
    public abstract Type Type { get; }

    #endregion    

    public abstract IODirection IODirection { get; }// => IODirection.Unspecified;

    public int Order { get; set; }

    public MemberKind MemberKind { get; set; }

    /// <summary>
    /// Informational flags about the Type of the member
    /// </summary>
    public IEnumerable<string> TypeFlags
    {
        // TODO: Change PropertyType to IEnumerable<Flag>
        get
        {
            var type = Type;
            var genericTypeDefinition = type.IsGenericType ? type.GetGenericTypeDefinition() : null;
            if (genericTypeDefinition == null) yield break;

            if (genericTypeDefinition == typeof(IObservable<>))
            {
                yield return "Observable";
            }
            else if (genericTypeDefinition == typeof(IAsyncEnumerable<>))
            {
                yield return "AsyncEnumerable";
            }
            else if (genericTypeDefinition.Name == "SettablePropertyAsync`2" || genericTypeDefinition.Name == "PropertyAsync`2")
            {
                yield return "Async";
            }
        }
    }

    public RelevanceFlags ReadRelevance { get; set; }

    public RelevanceFlags WriteRelevance { get; set; }

    public abstract IInspectorNode Create(object obj);
}


