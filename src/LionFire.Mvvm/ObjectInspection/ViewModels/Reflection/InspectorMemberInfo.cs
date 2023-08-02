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
    public string TypeFlags
    {
        // TODO: Change PropertyType to IEnumerable<Flag>
        get
        {
            var type = Type;
            var genericTypeDefinition = type.IsGenericType ? type.GetGenericTypeDefinition() : null;
            if (genericTypeDefinition == null) return "";

            if (genericTypeDefinition == typeof(IObservable<>))
            {
                return "Observable";
            }
            else if (genericTypeDefinition == typeof(IAsyncEnumerable<>))
            {
                return "AsyncEnumerable";
            }
            else if (genericTypeDefinition.Name == "SettablePropertyAsync`2" || genericTypeDefinition.Name == "PropertyAsync`2")
            {
                return "Async";
            }
            else
            {
                return "";
            }
        }
    }

    public RelevanceFlags ReadRelevance { get; set; }

    public RelevanceFlags WriteRelevance { get; set; }

    public abstract IMemberVM Create(object obj);
}


