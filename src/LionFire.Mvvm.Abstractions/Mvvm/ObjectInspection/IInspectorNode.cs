﻿using LionFire.Data.Async.Gets;
using DynamicData;
using DynamicData.Cache;
using DynamicData.Cache.Internal;
using System.Reactive.Linq;

namespace LionFire.Inspection;

///// <summary>
///// 
///// </summary>
///// <remarks>
///// 
///// </remarks>
//public interface IInspectorMember : INode
//{
//    InspectorChildKind Kind { get; }
//    IInspectorMemberInfo? Info { get; }

//    /// <summary>
//    /// sync: object
//    /// async: IGetter, ISetter, IValue
//    /// </summary>
//    object Source { get; }
//}

//public enum InspectorChildKind
//{
//    Unspecified = 0,
//    Member = 1 << 0,
//    Item = 1 << 1,
//    Relationship = 1 << 2,
//}
