using System.Reflection;

namespace LionFire.UI.Metadata;

public class FieldInfoVM : DataMemberInfoVM
{
    public FieldInfoVM(FieldInfo fieldInfo) : base(fieldInfo)
    {
    }

    public FieldInfo FieldInfo { get => (FieldInfo)MemberInfo; set => MemberInfo = value; }
    public override bool CanRead => true;
    public override bool CanWrite => !FieldInfo.IsInitOnly;
}


