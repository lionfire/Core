using LionFire.IO;
using System.Reflection;

namespace LionFire.Mvvm.ObjectInspection;

public class ReflectionFieldInfo : DataReflectionMemberInfo
{
    public ReflectionFieldInfo(FieldInfo fieldInfo) : base(fieldInfo)
    {
    }

    public FieldInfo FieldInfo { get => (FieldInfo)MemberInfo; set => MemberInfo = value; }

    public override IODirection IODirection =>  IODirection.Read | (FieldInfo.IsInitOnly ? IODirection.Unspecified : IODirection.Write);

    public override IInspectorNode Create(object obj) => new FieldVM(this, obj);
}
