using LionFire.Metadata;
using System.Reflection;
using LionFire.ExtensionMethods.Metadata;

namespace LionFire.Mvvm.ObjectInspection;

public interface IMemberInfoVM
{
    string Name { get; }
    Type Type { get; }

    public int Order { get; set; }

    public virtual bool CanRead => false;
    public virtual bool CanWrite => false;
}

public class CustomMemberInfoVM : IMemberInfoVM
{
    public CustomMemberInfoVM(string name, Type type)
    {
        Name = name;
        Type = type;
    }

    public string Name { get; }
    public Type Type { get; }

    public int Order { get; set; }

    public virtual bool CanRead => false;
    public virtual bool CanWrite => false;
}

public class MemberInfoVM : IMemberInfoVM
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

    #region Downcast // REVIEW - does this belong here?

    public virtual object? GetValue(object obj)
    {
        return MemberInfo switch
        {
            PropertyInfo pi => pi.GetValue(obj),
            FieldInfo fi => fi.GetValue(obj),
            //MethodInfo mi => mi.Invoke(obj, null),
            _ => throw new NotSupportedException(),
        };
    }

    #endregion


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


