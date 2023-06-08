﻿using LionFire.Structures;

namespace LionFire.Data.Async.Gets
{
    /// <summary>
    /// Like IReadHandle but without the Reference.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICachingResolves<out T> : IReadWrapper<T>, IGets<T>, IDefaultable
    {
    }
}
