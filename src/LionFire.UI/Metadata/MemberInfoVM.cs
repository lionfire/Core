using System.Reflection;

namespace LionFire.UI.Metadata;

public class MemberInfoVM
{
    #region Relationships

    public MemberInfo MemberInfo { get; set; }

    #region Derived

    public string Name => MemberInfo.Name;
    public virtual Type Type => MemberInfo switch
    {
        PropertyInfo pi => pi.PropertyType,
        FieldInfo fi => fi.FieldType,
        MethodInfo mi => mi.ReturnType,
        _ => throw new NotImplementedException(),
    };

    #endregion

    #endregion

    #region Lifecycle

    public MemberInfoVM(MemberInfo memberInfo)
    {
        MemberInfo = memberInfo;
    }

    #endregion

    public int Order { get; set; }

    public virtual bool CanRead => false;
    public virtual bool CanWrite => false;

    public virtual object? GetValue(object obj)
    {
        return MemberInfo switch
        {
            PropertyInfo pi => pi.GetValue(obj),
            FieldInfo fi => fi.GetValue(obj),
            //MethodInfo mi => mi.Invoke(obj, null),
            _ => throw new NotImplementedException(),
        };
    }

    /// <summary>
    /// Display text for the Type of the member
    /// </summary>
    public string DisplayType
    {
        get
        {
            var type = Type;
            var genericTypeDefinition = type.IsGenericType ? type.GetGenericTypeDefinition() : null;
            if (genericTypeDefinition == null) return type.Name;

            if (genericTypeDefinition == typeof(IObservable<>))
            {
                return type.GetGenericArguments()[0].Name;
            }
            else if (genericTypeDefinition == typeof(IAsyncEnumerable<>))
            {
                return type.GetGenericArguments()[0].Name;
            }
            else if (genericTypeDefinition.Name == "SettablePropertyAsync`2" || genericTypeDefinition.Name == "PropertyAsync`2")
            {
                return type.GetGenericArguments()[1].Name;
            }
            return type.Name;
        }
    }

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

    public RelevanceFlags ReadRelevance
    {
        get => readRelevance ??= MemberInfo.GetRelevanceFlags(RelevanceAspect.Read);
        set => readRelevance = value;
    }
    private RelevanceFlags? readRelevance;

    public RelevanceFlags WriteRelevance
    {
        get => writeRelevance ??= MemberInfo.GetRelevanceFlags(RelevanceAspect.Write);
        set => writeRelevance = value;
    }
    private RelevanceFlags? writeRelevance;
}


