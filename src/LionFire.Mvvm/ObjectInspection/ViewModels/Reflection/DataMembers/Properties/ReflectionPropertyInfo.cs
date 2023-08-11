using LionFire.IO;
using System.Reflection;

namespace LionFire.Mvvm.ObjectInspection;

public class ReflectionPropertyInfo : DataReflectionMemberInfo
{
    public ReflectionPropertyInfo(PropertyInfo propertyInfo) : base(propertyInfo)
    {
    }

    public PropertyInfo PropertyInfo { get => (PropertyInfo)MemberInfo; set => MemberInfo = value; }

    public override IODirection IODirection => PropertyInfo.GetIODirection();


    public override IInspectorNode Create(object obj) => new PropertyVM(this, obj);
}


