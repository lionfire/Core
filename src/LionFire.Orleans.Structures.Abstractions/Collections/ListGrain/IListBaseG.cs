﻿using LionFire.Collections.Async;

namespace LionFire.Orleans_.Collections;

/// <summary>
/// (Don't inherit from this directly when implementing a class)
/// </summary>
/// <typeparam name="TItem"></typeparam>
public interface IListBaseG<TItem>
    : IGrainWithStringKey
    , ICollectionG<TItem>
    , IAsyncListBase<TItem>
    , ICreatesG<TItem>
{
}
