using LionFire.Collections;
using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace LionFire.Persistence.Handles;

/// <summary>
/// A read-only notifying list
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IListHandle<T> : IReadHandle<Metadata<IEnumerable<IListing<T>>>>, INotifyCollectionChanged<T>
{
    void AutoLoad(bool enable);
}
