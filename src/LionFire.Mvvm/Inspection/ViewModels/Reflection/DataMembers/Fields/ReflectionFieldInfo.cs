﻿//using LionFire.IO;
//using System.Reflection;

//namespace LionFire.Inspection;

//public class ReflectionFieldInfo : DataReflectionMemberInfo
//{
//    public ReflectionFieldInfo(FieldInfo fieldInfo) : base(fieldInfo)
//    {
//    }

//    public FieldInfo FieldInfo { get => (FieldInfo)MemberInfo; set => MemberInfo = value; }

//    public override InspectorNodeKind NodeKind => InspectorNodeKind.Data;

//    public override IODirection IODirection =>  IODirection.Read | (FieldInfo.IsInitOnly ? IODirection.Unspecified : IODirection.Write);

    
//    //public override INode Create(object obj) => new FieldVM(this, obj);
//}
