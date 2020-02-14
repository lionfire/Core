using LionFire.Collections;
using LionFire.Persistence;
using LionFire.Persistence.Collections;
using LionFire.Referencing;
using LionFire.Structures;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Vos
{

    //public abstract class OBaseNameList<T> : RCollectionBase<INotifyingList<string>, string>
    //{
    //}

    //public class OBaseCollection<T> : OBaseNameList<T>
    //{
    //}

    //public class VosChildList<T> : OBaseNameList<T>
    //{
    //    #region Change events

    //    #endregion
    //    public IObserver<T> Removed { get; private set; }
    //    public IObserver<T> Added { get; private set; }

    //    public override int Count => throw new NotImplementedException();

    //    public override IEnumerator<string> GetEnumerator() => throw new NotImplementedException();
    //    public override void OnCollectionChangedEvent(INotifyCollectionChangedEventArgs<string> a) => throw new NotImplementedException();
    //    public override Task<bool> TryRetrieveObject() => throw new NotImplementedException();
    //}

    // how fat do i want this to be?  should I keep it to strings?  
    //public class OBaseChild
    //{
    //    public string Name { get; set; }
    //    public object Handle { get; set; }
    //    public Type Type { get; set; }
    //}

    /// <summary>
    /// A collection class designed for ease of use with manipulating a set of Vobs.
    /// By default, Delete operations are carried out immediately,
    /// and Create operations may reserve an Identifier and save a partially created object.
    /// 
    /// Fields:
    ///  - Vob
    /// </summary>
    /// <typeparam name="ChildType"></typeparam>
    public class Voc<ChildType> : INotifyingList<ChildType>, IVoc
        , IReadHandleCollection<ChildType>
        , INotifyCollectionChanged
        //, ICollection<ChildType>
        where ChildType : class, new()
    {
        #region Ontology

        #region Vob

        /// <summary>
        /// User can only set this once directly.  It may be modified by Move, Rename or Delete
        /// </summary>
        public Vob Vob {
            get => vob;
            set {
                if (vob == value) return;
                if (vob != default(Vob)) throw new AlreadySetException();
                vob = value;

                if (EffectiveAutoLoad)
                {
                    //Clear();
                    TryRetrieve();
                }

            }
        }
        private Vob vob;

        #endregion

        #endregion

        #region Construction

        public Voc(Vob vob)
        {
            this.Vob = vob;
        }

        #endregion


        public IEnumerable<string> ChildPaths => this.Select(vh => vh.Path);
        public IEnumerable<string> Names => this.Select(vh => LionPath.GetName(vh.Path));
        public IEnumerable<IReadHandle<ChildType>> Handles => this;
        //public IReadHandleCollection<ChildType> Handles => this;

        #region Parameters

        #region AutoLoad

        public static bool DefaultAutoLoad = true;

        public bool EffectiveAutoLoad => AutoLoad ?? DefaultAutoLoad;

        public bool? AutoLoad {
            get => autoLoad;
            set => autoLoad = value;
        }
        private bool? autoLoad;

        #endregion

        public bool? AutoSave { get; set; }

        #endregion

        #region List

        public void Clear() => throw new NotImplementedException();//if (Vob == null) return;//VobFilter filter = null;//Vob.DeleteChildren(filter);


        #endregion

        #region Misc

        public string ToStringStatus {
            get {
                if (Vob == null) return "null";
                //if (Count > 0)
                return Count.ToString() + " items";
            }
        }

        public override string ToString() => "[Voc: " + Vob.ToString() + " (" + ToStringStatus + ")]";

        #endregion


        #region Fields

        protected MultiBindableDictionary<string, IReadHandle<ChildType>> Dict {
            get => _dict;
            set {
                if (_dict == value) return;
                if (_dict != null)
                {
                    Dict.CollectionChangedNG -= dict_CollectionChangedNG;
                }
                _dict = value;
                if (_dict != null)
                {
                    Dict.CollectionChangedNG += dict_CollectionChangedNG;
                }
            }
        }
        private MultiBindableDictionary<string, IReadHandle<ChildType>> _dict;

        //protected MultiBindableCollection<IReadHandle<ChildType>> list = new MultiBindableCollection<IReadHandle<ChildType>>();

        protected IEnumerable<ChildType> Objects {
            get {
                if (Dict == null)
                {
                    TryRetrieve();
                }
                if (Dict != null)
                {
                    return Dict.Values.Select(vh => vh.Object);
                }
                else
                {
                    return empty;
                }
            }
        }

        private static readonly ChildType[] empty = new ChildType[] { };

        #endregion


        #region CRUD

        #region Move / Delete Methods

        public override void Move(Vob vobDestination) => throw new NotImplementedException();

        public override void Rename(string newName) => throw new NotImplementedException();

        public override void Delete() // Does not delete keyKeeper?
=> throw new NotImplementedException("TOPORT");//var children = Vob.GetVobChildrenOfType<ChildType>();//foreach (var child in children)//{//    child.TryDelete();//}

        #endregion

        #region Retrieval

        public void RefreshCollection() => TryRetrieve();

        public bool TryRetrieve()
        {
            //if (!Vob.Exists) return false; // FUTURE - test whether directory exists?  Only valid for directory-based filesystems?  

            var removals = new Dictionary<string, IReadHandle<ChildType>>();

            if (Dict == null) Dict = new MultiBindableDictionary<string, IReadHandle<ChildType>>();
            else { Dict.Clear(); }

            foreach (var kvp in Dict)
            {
                removals.Add(kvp.Key, kvp.Value);
            }

            var children = Vob.GetVobChildrenOfType<ChildType>();
            foreach (var vh in children)
            {
                if (removals.ContainsKey(vh.Key))
                {
                    removals.Remove(vh.Key);
                }
                else
                {
                    vh.TryEnsureRetrieved();
                    if (vh.Object as ChildType == null)
                    {
                        // REVIEW - why is this getting hit so many times?
                        //l.Trace(() => "[Voc] " + vh.Path + " - Got non-ChildType from Vob.GetVobChildrenOfType<"+typeof(ChildType).Name+">() - " + vh.Object.ToTypeNameSafe());
                        continue;
                    }
                    if (Dict.ContainsKey(vh.Key))
                    {
                        if (!object.ReferenceEquals(vh, Dict[vh.Key]))
                        { l.Warn("Multiple vh for " + vh.Key + " in " + this); Dict[vh.Key] = vh; }
                    }
                    else
                    {
                        Dict.Add(vh.Key, vh);
                    }
                }
            }

            foreach (var kvp in removals)
            {
                l.Trace("[VOC R] " + kvp.Key + " no longer exists, removing from Voc");
                Dict.Remove(kvp.Key);
                //list.Remove(kvp.Value);
            }

            // REVIEW - avoid Resets?  other events should be in place via dict.
            var ncc = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            MultiBindableEvents.RaiseCollectionChangedEventNonGeneric(collectionChangedNG, this, ncc);

            return true;
        }

        private void dict_CollectionChangedNG(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsAttachedToChildPropertyChanges)
            {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
                {
                    // TODO: Use weak events
                    l.TraceWarn("Voc: Got reset.  Not detaching/attaching to old IReadHandles.");
                    if (this.Dict.Count > 0)
                    {
                        foreach (var item in this.Dict.Values)
                        {
                            var inpc = item as IIReadHandle;
                            if (inpc != null) { inpc.ObjectChanged += OnChildObjectChanged; }
                            //if (inpc != null) { inpc.ObjectPropertyChanged += OnChildObjectPropertyChanged; }
                        }
                    }
                }
                else
                {
                    if (e.NewItems != null)
                    {
                        foreach (var item in e.NewItems)
                        {
                            var inpc = item as IIReadHandle;
                            if (inpc != null) { inpc.ObjectChanged += OnChildObjectChanged; }
                            //if (inpc != null) { inpc.ObjectPropertyChanged += OnChildObjectPropertyChanged; }
                        }
                    }
                    if (e.OldItems != null)
                    {
                        foreach (var item in e.OldItems)
                        {
                            var inpc = item as IIReadHandle;
                            if (inpc != null) { inpc.ObjectChanged -= OnChildObjectChanged; }
                            //if (inpc != null) { inpc.ObjectPropertyChanged -= OnChildObjectPropertyChanged; }
                        }
                    }
                }
            }

            // Pass-thru
            //var ncc = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item);
            MultiBindableEvents.RaiseCollectionChangedEventNonGeneric(collectionChangedNG, this, e);
        }

        private void OnChildObjectChanged(IReadHandle handle, string propertyName)
        {
            l.Debug("Voc got child object changed: " + propertyName + " for " + handle);
            childPropertyChanged?.Invoke(handle, propertyName);
        }
        //void OnChildObjectPropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    l.Debug("Voc got child property changed: " + e.PropertyName + " for " + sender);
        //    var ev = childPropertyChanged;
        //    if (ev != null) ev(sender, e);
        //}

        #endregion

        #endregion

        #region NotifyChildPropertyChanged

        public bool IsAttachedToChildPropertyChanges => childPropertyChanged != null;
        public event Action<object, string> ChildPropertyChanged {
            add {
                if (childPropertyChanged == null)
                {
                    foreach (var item in this.Dict.Values)
                    {
                        var inpc = item as IIReadHandle;
                        if (inpc != null) { inpc.ObjectChanged += OnChildObjectChanged; }
                        //if (inpc != null) { inpc.PropertyChanged -= OnChildObjectPropertyChanged; }
                    }
                }
                childPropertyChanged += value;
            }
            remove {
                childPropertyChanged -= value;
                if (childPropertyChanged == null)
                {
                    foreach (var item in this.Dict.Values)
                    {
                        var inpc = item as IIReadHandle;
                        if (inpc != null) { inpc.ObjectChanged -= OnChildObjectChanged; }
                        //if (inpc != null) { inpc.PropertyChanged -= OnChildObjectPropertyChanged; }
                    }
                }
            }
        }

        private event Action<object, string> childPropertyChanged;

        #endregion

        #region INotifyingList<T> Implementation

        public INotifyingList<FilterType> Filter<FilterType>() => throw new NotImplementedException();

        public ChildType[] ToArray() => Objects.ToArray();

        void ICollection<ChildType>.Add(ChildType item) => Add(item);
        public IReadHandle<ChildType> Add(ChildType item)
        {
            var vh = Vob.CreateChild(item);

            //Vob.GenerateStringKey();

            // dict Pass-thru events should be working, shouldn't need add event here.
            //var ncc = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item);
            //MultiBindableEvents.RaiseCollectionChangedEventNonGeneric(collectionChangedNG, this, ncc);
            Dict.Add(vh);
            //list.Add(vh);
            return vh;
        }

        public bool Contains(ChildType item) => Objects.Contains(item);

        public void CopyTo(ChildType[] array, int arrayIndex) => Objects.ToArray().CopyTo(array, arrayIndex);

        public override int Count => Dict.Count;

        public bool IsReadOnly => false;

        public bool Remove(ChildType item)
        {
            IEnumerable<KeyValuePair<string, IReadHandle<ChildType>>> e = Dict;

            foreach (var kvp in e.ToArray())
            {
                if (kvp.Value.Object == item)
                {
                    var vh = Dict[kvp.Key];
                    l.Warn("UNTESTED - removing item from Voc: " + vh.ToStringSafe());
                    vh.Delete();
                    Dict.Remove(kvp.Key);
                    return true;
                }
            }
            return false;
            //IKeyed<string> key
            //throw new NotImplementedException();
        }

        public IEnumerator<ChildType> GetEnumerator() => Objects.GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => Dict.Values.GetEnumerator(); // NOTE - different from generic GetEnumerator!

        public event NotifyCollectionChangedHandler<ChildType> CollectionChanged;
        protected void RaiseCollectionChanged(NotifyCollectionChangedEventArgs<ChildType> e)
        {
            var ev = CollectionChanged;
            if (ev != null) ev(e);
        }

        public int IndexOf(ChildType item)
        {
            for (int i = 0; i < Dict.Count; i++)
            {
                if (Dict.Values.ElementAt(i).Object.Equals(item)) return i;
            }
            return -1;
        }

        public void Insert(int index, ChildType item) => throw new NotImplementedException();

        public void RemoveAt(int index) => throw new NotImplementedException();

        public ChildType this[int index] {
            get => Dict.Values.ElementAt(index).Object;
            set {
                //list[index].Object;
                throw new NotImplementedException();
            }
        }

        #endregion

        #region Misc

        private static readonly ILogger l = Log.Get();

        #endregion

        event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged {
            add { collectionChangedNG += value; }
            remove { collectionChangedNG -= value; }
        }

        private event NotifyCollectionChangedEventHandler collectionChangedNG;



        public static string DefaultSubpath {
            get {
                if (defaultSubpath == null)
                {
                    defaultSubpath = typeof(ChildType).ToPluralName();
                }
                return defaultSubpath;
            }
        }
        private static string defaultSubpath;

        #region IReadHandleCollection<ChildType>, INotifyingList<IReadHandle<ChildType>>


        INotifyingList<FilterType> INotifyingList<IReadHandle<ChildType>>.Filter<FilterType>() => throw new NotImplementedException();

        IReadHandle<ChildType>[] INotifyingCollection<IReadHandle<ChildType>>.ToArray() => this.Dict.Values.ToArray();

        void ICollection<IReadHandle<ChildType>>.Add(IReadHandle<ChildType> item) => throw new NotSupportedException();

        void ICollection<IReadHandle<ChildType>>.Clear() => throw new NotSupportedException();

        bool ICollection<IReadHandle<ChildType>>.Contains(IReadHandle<ChildType> item) => Dict.Values.Contains(item);

        void ICollection<IReadHandle<ChildType>>.CopyTo(IReadHandle<ChildType>[] array, int arrayIndex) => Dict.Values.CopyTo(array, arrayIndex);

        int ICollection<IReadHandle<ChildType>>.Count => Dict.Count;

        bool ICollection<IReadHandle<ChildType>>.IsReadOnly => Dict.IsReadOnly;

        bool ICollection<IReadHandle<ChildType>>.Remove(IReadHandle<ChildType> item) => throw new NotSupportedException();

        IEnumerator<IReadHandle<ChildType>> IEnumerable<IReadHandle<ChildType>>.GetEnumerator() => Dict.Values.GetEnumerator();

        event NotifyCollectionChangedHandler<IReadHandle<ChildType>> INotifyCollectionChanged<IReadHandle<ChildType>>.CollectionChanged {
            add { vhCollectionChanged += value; }
            remove { vhCollectionChanged -= value; }
        }

       

        private NotifyCollectionChangedHandler<IReadHandle<ChildType>> vhCollectionChanged;

        int IList<IReadHandle<ChildType>>.IndexOf(IReadHandle<ChildType> item) => throw new NotImplementedException();//return Dict.Values.Find(item);

        void IList<IReadHandle<ChildType>>.Insert(int index, IReadHandle<ChildType> item) => throw new NotImplementedException();

        void IList<IReadHandle<ChildType>>.RemoveAt(int index) => throw new NotImplementedException();

        IReadHandle<ChildType> IList<IReadHandle<ChildType>>.this[int index] {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        #endregion

        public IReadHandle<ChildType> this[string name] => this.Vob[name].GetHandle<ChildType>();


        #region Filtering - TODO: Move this to a VocView, like WPF's CollectionView

        /// <summary>
        /// The default Predicate Filter will test against this if provided.  If you override the predicate filter, make sure to test for this if desired.
        /// </summary>
        public FlagCollection FlagFilter { get; set; }
        public Predicate<ChildType> PredicateFilter {
            get => predicateFilter ?? defaultPredicateFilter;
            set => predicateFilter = value;
        }
        private Predicate<ChildType> predicateFilter;

        private bool defaultPredicateFilter(ChildType child) => DefaultFilter(child, FlagFilter);

        #endregion

        #region (Static) Filter

        private static readonly Func<ChildType, FlagCollection, bool> predicateFlagFilter = DefaultFilter;

        private static bool HasFlags {
            get { if (!hasFlags.HasValue) { hasFlags = typeof(IHasFlagCollection).IsAssignableFrom(typeof(ChildType)); } return hasFlags.Value; }
            set => hasFlags = value; // Set to true manually if only certain derived classes of ChildType have flags.
        }

        IEnumerable<IReadHandle<ChildType>> IReadHandleCollection<ChildType>.Handles => throw new NotImplementedException();

        IReadHandle<ChildType> IReadHandleCollection<ChildType>.this[string name] => throw new NotImplementedException();

        private static bool? hasFlags;
        private static bool DefaultFilter(ChildType child, FlagCollection filter)
        {
            if (HasFlags && filter != null)
            {
                var obj = child as IHasFlagCollection;
                if (obj != null && !obj.FlagCollection.PassesFilter(filter)) return false;
            }
            return true;
        }

        public int IndexOf(IReadHandle<ChildType> item) => throw new NotImplementedException();
        public void Insert(int index, IReadHandle<ChildType> item) => throw new NotImplementedException();
        public void Add(IReadHandle<ChildType> item) => throw new NotImplementedException();
        public bool Contains(IReadHandle<ChildType> item) => throw new NotImplementedException();
        public void CopyTo(IReadHandle<ChildType>[] array, int arrayIndex) => throw new NotImplementedException();
        public bool Remove(IReadHandle<ChildType> item) => throw new NotImplementedException();

        #endregion

    }

    public interface IHasFlagCollection
    {
        FlagCollection FlagCollection { get; }
    }

    public class Voc : Voc<object>, INotifyingList<object>, IVoc
    {
        #region Retrieval

        //public override bool TryRetrieve()
        //{
        //    if (!Vob.Exists) return false;

        //    foreach (var vh in Vob.GetVobChildrenOfType<object>())
        //    {
        //        // Speed this up with a dictionary
        //        if (!list.Contains(vh))
        //        {
        //            list.Add(vh);
        //        }
        //    }

        //    foreach (var n in Vob.GetChildrenNamesOfType<object>())
        //    {
        //        // Speed this up with a dictionary
        //        if (!dict.Contains(n))
        //        {
        //            dict.Add(n, Vob[n].ToHandle<object>());
        //        }
        //    }


        //    return true;
        //}

        #endregion

        //#region Move / Delete Methods

        //public override void Move(Vob vobDestination)
        //{
        //    throw new NotImplementedException();
        //}

        //public override void Rename(string newName)
        //{
        //    throw new NotImplementedException();
        //}

        //public override void Delete()
        //{
        //    throw new NotImplementedException();
        //}

        //#endregion

        //#region INotifyingList<>

        //public INotifyingList<FilterType> Filter<FilterType>()
        //{
        //    throw new NotImplementedException();
        //}

        //public object[] ToArray()
        //{
        //    return list.ToArray();
        //}

        //public void Add(object item)
        //{

        //    list.Add(item);            
        //}

        //public bool Contains(object item)
        //{
        //    return list.Contains(item);
        //}

        //public void CopyTo(object[] array, int arrayIndex)
        //{
        //    list.CopyTo(array, arrayIndex);
        //}

        //public override int Count
        //{
        //    get { return list.Count; }
        //}

        //public bool IsReadOnly
        //{
        //    get { throw new NotImplementedException(); }
        //}

        //public bool Remove(object item)
        //{
        //    throw new NotImplementedException();
        //}

        //public IEnumerator<object> GetEnumerator()
        //{
        //    throw new NotImplementedException();
        //}

        //System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        //{
        //    throw new NotImplementedException();
        //}

        //public event NotifyCollectionChangedHandler<object> CollectionChanged;

        //public int IndexOf(object item)
        //{
        //    throw new NotImplementedException();
        //}

        //public void Insert(int index, object item)
        //{
        //    throw new NotImplementedException();
        //}

        //public void RemoveAt(int index)
        //{
        //    throw new NotImplementedException();
        //}

        //public object this[int index]
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //    set
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //#endregion

        #region Misc

        private static readonly ILogger l = Log.Get();

        #endregion

    }
}
