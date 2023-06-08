using System.Reflection;

namespace LionFire.UI.Metadata;

public class MethodInfoVM : MemberInfoVM
{
    public MethodInfoVM(MethodInfo methodInfo) : base(methodInfo)
    {
    }

    public MethodInfo MethodInfo { get => (MethodInfo)MemberInfo; set => MemberInfo = value; }

}


