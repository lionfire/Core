// Retrieved from http://baumbartsjourney.wordpress.com/2009/06/01/an-alternative-to-observablecollection/
// on April 30, 2013, assume Public Domain license

// Jared:
//  - for insert methods, specify correct index (maybe for add methods too)
//  - Added IList<T> detection to avoid always recreating it

using System;
using System.Linq;
using System.Collections.Specialized;
using System.Collections.Generic;
#if !UNITY && WPF // TODO FIXME for WPF
using System.Windows.Data;
#endif
using System.ComponentModel;
using System.Collections;

namespace LionFire.Collections;

// RENAMED to IObservableList2 to avoid conflict with DynamicData. 
// DEPRECATED - use DynamicData instead
public interface IObservableList2<T> : IList<T>, INotifyCollectionChanged { }

public class ObservableList2<T> : List<T>, IObservableList2<T>, INotifyPropertyChanged
{
    public bool IsReadOnly { get { return ((IList)this).IsReadOnly; } }

    #region Constructors
    public ObservableList2()
    {
        IsNotifying = true;

        // Jared Commented this as it seems to throw a FieldAccessException in Unity
        //			// As a gimmick, I wanted to bind to the Count property, so I
        //			// use the «OnPropertyChanged event from the INotifyPropertyChanged
        //			// interface to notify about Count changes.
        //			CollectionChanged += new NotifyCollectionChangedEventHandler(
        //				delegate(object sender, NotifyCollectionChangedEventArgs e)
        //				{
        //				OnPropertyChanged("Count");
        //			}
        //			);
    }
    public ObservableList2(IEnumerable<T> list) : this()
    {
        this.AddRange(list);
    }
    #endregion

    #region Properties
    public bool IsNotifying { get; set; }
    #endregion

    #region Adding and removing items

    public new void Add(T item)
    {
        int index = base.Count;
        base.Add(item);
        NotifyCollectionChangedEventArgs e =
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index);

        OnCollectionChanged(e);
    }

    public new void AddRange(IEnumerable<T> collection)
    {
        IList<T> list = collection as IList<T>;
        if (list == null) list = collection.ToList<T>();

        int index = base.Count;
        base.AddRange(collection);
        NotifyCollectionChangedEventArgs e =
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list, index);
        //OnCollectionChanged(e); // does this work on CollectionViews?
        OnCollectionChangedMultiItem(e); // UNTESTED
    }

    public new void Clear()
    {
        base.Clear();
        NotifyCollectionChangedEventArgs e =
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
        OnCollectionChanged(e);
    }

    public new void Insert(int i, T item)
    {
        base.Insert(i, item);
        NotifyCollectionChangedEventArgs e =
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, i);
        OnCollectionChanged(e);
    }

    public new void InsertRange(int i, IEnumerable<T> collection) // UNTESTED? 
    {
        IList<T> list = collection as IList<T>;
        if (list == null) list = collection.ToList<T>();

        base.InsertRange(i, collection);
        NotifyCollectionChangedEventArgs e =
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (IList)list, i);
        //OnCollectionChanged(e);
        OnCollectionChangedMultiItem(e);
    }

    public new bool Remove(T item)
    {
        int index = base.IndexOf(item);
        bool result = base.Remove(item);
        if (result)
        {
            NotifyCollectionChangedEventArgs e =
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index);
            OnCollectionChanged(e);
        }
        return result;
    }

    public new void RemoveAll(Predicate<T> match)
    {
        List<T> backup = FindAll(match);
        base.RemoveAll(match);
        NotifyCollectionChangedEventArgs e =
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, backup);
        //OnCollectionChanged(e); // does this work on CollectionViews?
        OnCollectionChangedMultiItem(e); // UNTESTED
    }

    public new void RemoveAt(int i)
    {
        T backup = this[i];
        base.RemoveAt(i);
        NotifyCollectionChangedEventArgs e =
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, backup, i);
        OnCollectionChanged(e);
    }

    public new void RemoveRange(int index, int count)
    {
        List<T> backup = GetRange(index, count);
        base.RemoveRange(index, count);
        NotifyCollectionChangedEventArgs e =
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, backup);
        //OnCollectionChanged(e); // does this work on CollectionViews?
        OnCollectionChangedMultiItem(e); // UNTESTED
    }

    public new T this[int index]
    {
        get { return base[index]; }
        set
        {
            T oldValue = base[index];
            NotifyCollectionChangedEventArgs e =
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldValue);
            OnCollectionChanged(e);
        }
    }
    #endregion

    #region INotifyCollectionChanged Members

    public event NotifyCollectionChangedEventHandler CollectionChanged
    {
        add { collectionChanged += value; }
        remove { collectionChanged -= value; }
    }
    public NotifyCollectionChangedEventHandler collectionChanged;

    protected void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        var ev = collectionChanged;
        if (IsNotifying && ev != null)
            try
            {
                collectionChanged(this, e);
            }
            catch (System.NotSupportedException)
            {
                NotifyCollectionChangedEventArgs alternativeEventArgs =
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                OnCollectionChanged(alternativeEventArgs);
            }
    }

    // Retrieved from http://geekswithblogs.net/NewThingsILearned/archive/2008/01/16/listcollectionviewcollectionview-doesnt-support-notifycollectionchanged-with-multiple-items.aspx on March 12, 2014
    protected virtual void OnCollectionChangedMultiItem(NotifyCollectionChangedEventArgs e)
    {
        NotifyCollectionChangedEventHandler handlers = this.collectionChanged;
        if (handlers != null)
        {
            foreach (NotifyCollectionChangedEventHandler handler in handlers.GetInvocationList())
            {
#if !UNITY && !MONO && WPF
                if (handler.Target is CollectionView)
                    ((CollectionView)handler.Target).Refresh();
                else
#endif
                    handler(this, e);
            }
        }
    }

    #endregion

    #region INotifyPropertyChanged Members
    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    #endregion
}

