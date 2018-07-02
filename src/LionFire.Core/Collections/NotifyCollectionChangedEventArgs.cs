#if MOVED
using System;
using System.Linq;
using System.Collections.Specialized;
using LionFire.Serialization;
using System.Collections;

namespace LionFire.Collections
{
    // Design note: : I tried out generic params on INotifyCollChanged<T>, as well as a two-tier SNotifyColl changed for services, but ended up going with a proxy approach to translate from a concrete (or local-only interface) to a remotable service interface.

    // OLD - Clean up past experiments

    //public delegate void NotifyCollectionChangedHandler<T> (NotifyCollectionChangedEventArgs<T> collection, NotifyCollectionChangedEventArgs<T> args);

    //#if AOT
    //	public delegate void NotifyCollectionChangedHandler2<TValue>(NotifyCollectionChangedEventArgs<TValue> e);
    //#endif

    //public delegate void SNotifyCollectionChangedHandler< T>(NotifyCollectionChangedEventArgs<T> e);

    //public class NotifyCollectionChangedEventArgs<T>
    //{
    //    public NotifyCollectionChangedEventArgs(){}
    //    public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action)
    //    {
    //        if(action != NotifyCollectionChangedAction.Reset) throw new ArgumentException("Must be reset action");
    //        this.Action = action;
    //    }

    //    public NotifyCollectionChangedAction Action { get; set; }
    //    public IEnumerable<T> NewItems { get; set; }
    //    public IEnumerable<T> OldItems { get; set; }
    //}

    //public interface NotifyCollectionChangedEventArgs<out T>
    //{
    //    T[] NewItems { get; }
    //    T[] OldItems { get; }
    //    NotifyCollectionChangedEventArgs ToNonGeneric();
    //}

    [LionSerializable(SerializeMethod.ByValue)]
    public class NotifyCollectionChangedEventArgs<T>
    //: NotifyCollectionChangedEventArgs<T>
    {
        public static NotifyCollectionChangedEventArgs<T> Added(params T[] items)
        {
            return new NotifyCollectionChangedEventArgs<T>(NotifyCollectionChangedAction.Add, items);
        }
        public static NotifyCollectionChangedEventArgs<T> Removed(params T[] items)
        {
            return new NotifyCollectionChangedEventArgs<T>(NotifyCollectionChangedAction.Remove, items);
        }

        public NotifyCollectionChangedEventArgs() { Action = NotifyCollectionChangedAction.Replace; }
        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action)
        {
            if (action != NotifyCollectionChangedAction.Reset) throw new ArgumentException("Must be reset action");
            this.Action = action;
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, T[] items)
        {
            if (action != NotifyCollectionChangedAction.Add
                && action != NotifyCollectionChangedAction.Remove) throw new ArgumentException("Must be add or remove action");

            this.Action = action;

            if (action == NotifyCollectionChangedAction.Add)
            {
                this.NewItems = items;
            }
            else if (action == NotifyCollectionChangedAction.Remove)
            {
                this.OldItems = items;
            }
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedEventArgs nonGeneric)
        {
            this.Action = nonGeneric.Action;
            if (nonGeneric.NewItems != null) this.NewItems = nonGeneric.NewItems.OfType<T>().ToArray();
            if (nonGeneric.OldItems != null) this.OldItems = nonGeneric.OldItems.OfType<T>().ToArray();
        }

        public NotifyCollectionChangedAction Action { get; set; }

        [LionSerializable(SerializeMethod.ByValue)]
        public T[] NewItems { get; set; }
        [LionSerializable(SerializeMethod.ByValue)]
        public T[] OldItems { get; set; }

        public NotifyCollectionChangedEventArgs ToNonGeneric()
        {
            NotifyCollectionChangedEventArgs nonGenericArgs;

            if (this.Action == NotifyCollectionChangedAction.Reset)
            {
                nonGenericArgs = new NotifyCollectionChangedEventArgs(this.Action);
            }
            else
            {
                nonGenericArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, NewItems == null ? (IList)new System.Collections.ArrayList() : NewItems.ToList(), OldItems == null ? (IList)new System.Collections.ArrayList() : OldItems.ToList());
            }

            return nonGenericArgs;
        }
    }

}

#endif