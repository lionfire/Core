using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Collections.Concurrent;


// REVIEW

// Source: http://stackoverflow.com/a/11034999/208304 "Haven't really tested it (performance- or reliability-wise). YMMV."
// - Jared: Added a few things

public class ConcurrentHashSet<T> : IDisposable, IEnumerable<T>
{
    private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
    private readonly HashSet<T> _hashSet;

    #region Construction

    public ConcurrentHashSet() { _hashSet = new HashSet<T>(); }
    public ConcurrentHashSet(IEqualityComparer<T> comparer) { _hashSet = new HashSet<T>(comparer); }

    #endregion

    #region Implementation of ICollection<T> ...ish

    public bool Add(T item)
    {
        try
        {
            _lock.EnterWriteLock();
            return _hashSet.Add(item);
        }
        finally
        {
            if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
        }
    }

    public void Clear()
    {
        try
        {
            _lock.EnterWriteLock();
            _hashSet.Clear();
        }
        finally
        {
            if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
        }
    }

    public bool Contains(T item)
    {
        try
        {
            _lock.EnterReadLock();
            return _hashSet.Contains(item);
        }
        finally
        {
            if (_lock.IsReadLockHeld) _lock.ExitReadLock();
        }
    }
    //public T TryGet(T item) // See http://stackoverflow.com/questions/7760364/how-to-retrieve-actual-item-from-hashsett
    

    public bool Remove(T item)
    {
        try
        {
            _lock.EnterWriteLock();
            return _hashSet.Remove(item);
        }
        finally
        {
            if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
        }
    }

    public int Count
    {
        get
        {
            try
            {
                _lock.EnterReadLock();
                return _hashSet.Count;
            }
            finally
            {
                if (_lock.IsReadLockHeld) _lock.ExitReadLock();
            }
        }
    }
    #endregion

    #region Dispose
    public void Dispose()
    {
        if (_lock != null) _lock.Dispose();
    }

    #endregion

    public T[] ToArray()
    {
        try
        {
            _lock.EnterReadLock();
            return _hashSet.ToArray();
        }
        finally
        {
            if (_lock.IsReadLockHeld) _lock.ExitReadLock();
        }
    }

    /// <summary>
    /// (Threadsafe -- makes a copy)
    /// </summary>
    /// <returns></returns>
    public IEnumerator<T> GetEnumerator()
    {
        return this.ToArray().OfType<T>().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
