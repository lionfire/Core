using LionFire.Dependencies;
using MorseCode.ITask;
using System.Collections.Generic;

namespace LionFire.Data.Async.Gets;

// Duplicate of AmbientGets

//public class DependencyResolves<TKey, TValue> : LazilyGets<TKey, TValue>, ILazilyGets<TValue>
//    where TKey : class
//    where TValue : class
//{
//    protected override ITask<IGetResult<TValue>> GetImpl() 
//        => LionFire.ExtensionMethods.Poco.Resolvables.IResolverPocoExtensions.Resolve<TKey, TValue>(Key).AsITask();
//}
