//using System.Reflection;
//using LionFire.ExtensionMethods.Metadata;
//using LionFire.IO;
//using LionFire.Metadata;

//namespace LionFire.Inspection;

//public abstract class ReflectionMemberInfo : INodeInfo
//{
//    #region Relationships

//    public MemberInfo MemberInfo { get; set; }

//    #region Derived

//    public  string Name => MemberInfo.Name;
//    public Type Type => MemberInfo switch
//    {
//        PropertyInfo pi => pi.PropertyType,
//        FieldInfo fi => fi.FieldType,
//        MethodInfo mi => mi.ReturnType,
//        _ => throw new NotImplementedException(),
//    };

//    #endregion

//    #endregion

//    #region Lifecycle

//    public ReflectionMemberInfo(System.Reflection.MemberInfo memberInfo)
//    {
//        MemberInfo = memberInfo;
//        ReadRelevance = MemberInfo.GetRelevanceFlags(RelevanceAspect.Read);
//        WriteRelevance = MemberInfo.GetRelevanceFlags(RelevanceAspect.Write);
//    }

//    #endregion

//    public RelevanceFlags ReadRelevance { get; }

//    public RelevanceFlags WriteRelevance { get; }
//    public virtual string? Order => null;

//    public abstract InspectorNodeKind NodeKind { get; }


//    public abstract InspectorNodeKind NodeKind { get; }
//    public abstract IEnumerable<string> Flags { get; }
//    public abstract IODirection IODirection { get; }

//    #region OLD - GetValue  // REVIEW - Downcast, does this belong here?

//    //public virtual object? GetValue(object obj)
//    //{
//    //    return MemberInfo switch
//    //    {
//    //        PropertyInfo pi => pi.GetValue(obj),
//    //        FieldInfo fi => fi.GetValue(obj),
//    //        //MethodInfo mi => mi.Invoke(obj, null),
//    //        _ => throw new NotSupportedException(),
//    //    };
//    //}

//    #endregion
//}
