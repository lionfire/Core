using System.Reflection;

namespace LionFire.UI.Metadata;

public class PropertyInfoVM : DataMemberInfoVM
{
    public PropertyInfoVM(PropertyInfo propertyInfo) : base(propertyInfo)
    {
    }

    public PropertyInfo PropertyInfo { get => (PropertyInfo)MemberInfo; set => MemberInfo = value; }
    public override bool CanRead => PropertyInfo.CanRead;
    public override bool CanWrite => PropertyInfo.CanWrite;

}


