using LionFire.IO;
using LionFire.Metadata;

namespace LionFire.Inspection.Nodes;

public interface INodeInfo
{
    string? Name { get; }
    string DisplayName { get => Name ?? (Type?.Name ?? $"{{{this.GetType().Name}}}"); }

    /// <summary>
    /// If Order is null, Name should be considered instead.
    /// 
    /// If it is a number, it will be cast to a float. Nummbers will appear before alphabetical orders.
    /// Negative numbers will appear at the end.
    /// 
    /// Example order:
    /// 
    /// 0
    /// 1
    /// 2
    /// 00000003
    /// Axx
    /// axx
    /// Bxx
    /// bxx
    /// -3
    /// -0000002.1
    /// -0002.05
    /// -1
    /// </summary>
    string? Order { get; }

    InspectorNodeKind NodeKind { get; }

    IEnumerable<string> Flags { get; }

    IODirection IODirection { get; }

    /// <summary>
    /// Output type
    /// </summary>
    Type? Type { get; }

    /// <summary>
    /// Informational flags about the Type of the member
    /// </summary>
    IEnumerable<string> TypeFlags
    {
        // TODO: Change PropertyType to IEnumerable<Flag>
        get
        {
            var type = Type;
            if (type == null) yield break;

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

    IEnumerable<Type>? InputType { get => null; }

    RelevanceFlags ReadRelevance { get => RelevanceFlags.Unspecified; } 

    RelevanceFlags WriteRelevance { get => RelevanceFlags.Unspecified; }

}
