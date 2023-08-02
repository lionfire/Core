using System.Reflection;
using LionFire.ExtensionMethods.Metadata;
using LionFire.Metadata;

namespace LionFire.Mvvm.ObjectInspection;

public abstract class ReflectionMemberInfo : InspectorMemberInfo
{
    #region Relationships

    public MemberInfo MemberInfo { get; set; }

    #region Derived

    public override string Name => MemberInfo.Name;
    public override Type Type => MemberInfo switch
    {
        PropertyInfo pi => pi.PropertyType,
        FieldInfo fi => fi.FieldType,
        MethodInfo mi => mi.ReturnType,
        _ => throw new NotImplementedException(),
    };

    #endregion

    #endregion

    #region Lifecycle

    public ReflectionMemberInfo(System.Reflection.MemberInfo memberInfo)
    {
        MemberInfo = memberInfo;
        ReadRelevance = MemberInfo.GetRelevanceFlags(RelevanceAspect.Read);
        WriteRelevance = MemberInfo.GetRelevanceFlags(RelevanceAspect.Write);
    }

    #endregion

    #region OLD - GetValue  // REVIEW - Downcast, does this belong here?

    //public virtual object? GetValue(object obj)
    //{
    //    return MemberInfo switch
    //    {
    //        PropertyInfo pi => pi.GetValue(obj),
    //        FieldInfo fi => fi.GetValue(obj),
    //        //MethodInfo mi => mi.Invoke(obj, null),
    //        _ => throw new NotSupportedException(),
    //    };
    //}

    #endregion
}
