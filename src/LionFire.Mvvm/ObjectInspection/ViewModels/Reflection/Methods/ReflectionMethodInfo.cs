﻿using LionFire.IO;
using System.Reflection;

namespace LionFire.Inspection;

public class ReflectionMethodInfo : ReflectionMemberInfo
{
    public override IODirection IODirection => MethodInfo.GetIODirection();

    public ReflectionMethodInfo(MethodInfo methodInfo) : base(methodInfo)
    {
    }

    public MethodInfo MethodInfo { get => (MethodInfo)MemberInfo; set => MemberInfo = value; }
    public override IInspectorNode Create(object obj) => new MethodVM(this, obj);
}


