﻿using System.Reflection;

namespace LionFire.Mvvm.ObjectInspection;

public class GetsInfo : CustomMemberInfo
{
    public GetsInfo(string name, Type type) : base(name, type, IO.IODirection.Read)
    {
    }

    //public MethodInfo? Getter { get; init; }
    //public bool Preload { get; set; }

    //public MethodInfo? Setter { get; init; }
    //IAsyncGetsRx
}

//public class AsyncValueInfo : CustomMemberInfo
//{
//    public AsyncGetsInfo(string name, Type type) : base(name, type)
//    {
//    }

//    AsyncValue<>
//}