using LionFire.UI.Metadata;
using System.Reflection;

namespace LionFire.UI.Components.PropertyGrid;

public class FieldVM : DataMemberVM<FieldInfo, FieldInfoVM>
{
    public override FieldInfo DataMemberInfo => DataMemberInfoVM.FieldInfo;
    public override FieldInfoVM DataMemberInfoVM => (FieldInfoVM)MemberInfoVM;
    public override object? GetValue() => DataMemberInfo.GetValue(Target);
    public override Type DataType => DataMemberInfo.FieldType;
}
