using LionFire.Mvvm.ObjectInspection;
using System.Reflection;

namespace LionFire.Mvvm.ObjectInspection;

public class IndexPropertyVM : DataMemberVM<PropertyInfo, IndexPropertyInfoVM>
{
    public override PropertyInfo DataMemberInfo => DataMemberInfoVM.PropertyInfo;
    public override IndexPropertyInfoVM DataMemberInfoVM => (IndexPropertyInfoVM)MemberInfoVM;
    public override object? GetValue() => throw new NotSupportedException();
    public override bool CanGetValue => false;
    public override Type DataType => DataMemberInfo.PropertyType;
}
