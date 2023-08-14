using System.Reflection;

namespace LionFire.Inspection;

/// <summary>
/// For Fields and Properties
/// </summary>
public abstract class DataReflectionMemberInfo : ReflectionMemberInfo
{
    public DataReflectionMemberInfo(System.Reflection.MemberInfo memberInfo) : base(memberInfo) { }
}


