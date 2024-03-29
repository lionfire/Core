﻿using LionFire.Data;
using LionFire.Data.Async.Sets;
using LionFire.Persistence.Handles;

namespace LionFire.Persistence; // RENAME to LionFire.Data.Handles

public interface IContravariantWriteHandleBase<in T> : IHandleBase, IDefaultableWriteWrapper<T>, ISetter, IDiscardableValue
{
}
