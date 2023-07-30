using System.Reflection;

namespace LionFire.Mvvm.ObjectInspection;
public class FieldVM : DataMemberVM<FieldInfo, FieldInfoVM>
{
    public override FieldInfo DataMemberInfo => DataMemberInfoVM.FieldInfo;
    public override FieldInfoVM DataMemberInfoVM => (FieldInfoVM)MemberInfoVM;
    public override object? GetValue() => DataMemberInfo.GetValue(Target);
    public override Type DataType => DataMemberInfo.FieldType;
}
