using System.Reflection;

namespace LionFire.Mvvm.ObjectInspection;
public class FieldVM : DataReflectionMember<FieldInfo, ReflectionFieldInfo>
{
    public FieldVM(ReflectionMemberInfo info, object source) : base(info, source)
    {
    }

    public override FieldInfo DataMemberInfo => ReflectionMemberInfo.FieldInfo;
    
    //public override object? GetValue() => DataMemberInfo.GetValue(Source);
    //public override Type DataType => DataMemberInfo.FieldType;
}
