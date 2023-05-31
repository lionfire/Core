using System.Reflection;

namespace LionFire.UI.Metadata;

public class MemberInfoVM
{
    public MemberInfoVM(MemberInfo memberInfo)
    {
        MemberInfo = memberInfo;
    }

    public string Name => MemberInfo.Name;

    public int Order { get; set; }

    public MemberInfo MemberInfo { get; set; }

    public virtual Type Type => MemberInfo switch
    {
        PropertyInfo pi => pi.PropertyType,
        FieldInfo fi => fi.FieldType,
        MethodInfo mi => mi.ReturnType,
        _ => throw new NotImplementedException(),
    };

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
}

public class DataMemberInfoVM : MemberInfoVM
{
    public DataMemberInfoVM(MemberInfo memberInfo) : base(memberInfo) { }
}

public class PropertyInfoVM : DataMemberInfoVM
{
    public PropertyInfoVM(PropertyInfo propertyInfo) : base(propertyInfo)
    {
    }

    public PropertyInfo PropertyInfo { get => (PropertyInfo)MemberInfo; set => MemberInfo = value; }
}
public class FieldInfoVM : DataMemberInfoVM
{
    public FieldInfoVM(FieldInfo fieldInfo) : base(fieldInfo)
    {
    }

    public FieldInfo FieldInfo { get => (FieldInfo)MemberInfo; set => MemberInfo = value; }
}

public class MethodInfoVM : MemberInfoVM
{
    public MethodInfoVM(MethodInfo methodInfo) : base(methodInfo)
    {
    }

    public MethodInfo MethodInfo { get => (MethodInfo)MemberInfo; set => MemberInfo = value; }

}


