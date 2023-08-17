using LionFire.IO;
using System.Reflection;

namespace LionFire.Inspection;

public class ReflectionPropertyInfo : DataReflectionMemberInfo
{
    public ReflectionPropertyInfo(PropertyInfo propertyInfo) : base(propertyInfo)
    {
    }

    public PropertyInfo PropertyInfo { get => (PropertyInfo)MemberInfo; set => MemberInfo = value; }

    public override IODirection IODirection => PropertyInfo.GetIODirection();


    public override INode Create(object obj) => new PropertyVM(this, obj);
}


