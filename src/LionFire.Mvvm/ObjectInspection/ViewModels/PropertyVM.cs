using System.Reflection;
using System.Runtime.Serialization;

namespace LionFire.Mvvm.ObjectInspection;

public class PropertyVM : DataMemberVM<PropertyInfo, PropertyInfoVM>
{
    public override PropertyInfo DataMemberInfo => DataMemberInfoVM.PropertyInfo;
    public override PropertyInfoVM DataMemberInfoVM => (PropertyInfoVM)MemberInfoVM;
    public override object? GetValue()
    {
        if (GetException != null) return GetException;
        try
        {
            return DataMemberInfo.GetValue(Target);
        }
        catch(Exception ex)
        {
            GetException = ex;
            return ex;
        }
    }
    public override Type DataType => DataMemberInfo.PropertyType;
}

public class AsyncPropertyInfoVM : DataMemberInfoVM
{
    public Type PropertyType { get; init; }

    public MethodInfo? Getter { get; init; }
    public bool Preload { get; set; }

    public MethodInfo? Setter { get; init; }
}

public class AsyncPropertyVM : DataMemberVM<PropertyInfo, PropertyInfoVM>
{
    public object Source { get; init; }

    public AsyncPropertyInfoVM AsyncPropertyInfoVM { get; set; }

    public override PropertyInfo DataMemberInfo => throw new NotImplementedException();

    public override PropertyInfoVM DataMemberInfoVM => throw new NotImplementedException();

    public override Type DataType => AsyncPropertyInfoVM.PropertyType;

    public override object? GetValue()
    {
        throw new NotImplementedException();
    }
}