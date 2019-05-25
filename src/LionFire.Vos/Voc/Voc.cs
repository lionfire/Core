#if VOC
using LionFire.Collections;
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
    
    public abstract class OBaseNameList<T> : RCollectionBase<INotifyingList<string>, string>
    {
    }

    //public class OBaseCollection<T> : OBaseNameList<T>
    //{
    //}

    

    public class VosChildList<T> : OBaseNameList<T>
    {
        #region Change events

        #endregion
        public IObserver<T> Removed { get; private set; }
        public IObserver<T> Added { get; private set; }

        public override int Count => throw new NotImplementedException();

        public override IEnumerator<string> GetEnumerator() => throw new NotImplementedException();
        public override void OnCollectionChangedEvent(INotifyCollectionChangedEventArgs<string> a) => throw new NotImplementedException();
        public override Task<bool> TryRetrieveObject() => throw new NotImplementedException();
    }

    // how fat do i want this to be?  should I keep it to strings?  
    public class OBaseChild
    {
        public string Name { get; set; }
        public object Handle { get; set; }
        public Type Type { get; set; }
    }

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
        , IVohac<ChildType>
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
        public IEnumerable<VobReadHandle<T>> Handles => this;

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

        protected MultiBindableDictionary<string, VobHandle<ChildType>> Dict {
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
        private MultiBindableDictionary<string, VobHandle<ChildType>> _dict;

        //protected MultiBindableCollection<VobHandle<ChildType>> list = new MultiBindableCollection<VobHandle<ChildType>>();

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

        public override bool TryRetrieve()
        {
            //if (!Vob.Exists) return false; // FUTURE - test whether directory exists?  Only valid for directory-based filesystems?  

            var children = Vob.GetVobChildrenOfType<ChildType>();

            var removals = new Dictionary<string, VobHandle<ChildType>>();

            if (Dict == null) Dict = new MultiBindableDictionary<string, VobHandle<ChildType>>();
            else { Dict.Clear(); }

            foreach (var kvp in Dict)
            {
                removals.Add(kvp.Key, kvp.Value);
            }

            foreach (var vh in Vob.GetVobChildrenOfType<ChildType>())
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
                    l.TraceWarn("Voc: Got reset.  Not detaching/attaching to old VobHandles.");
                    if (this.Dict.Count > 0)
                    {
                        foreach (var item in this.Dict.Values)
                        {
                            var inpc = item as IVobHandle;
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
                            var inpc = item as IVobHandle;
                            if (inpc != null) { inpc.ObjectChanged += OnChildObjectChanged; }
                            //if (inpc != null) { inpc.ObjectPropertyChanged += OnChildObjectPropertyChanged; }
                        }
                    }
                    if (e.OldItems != null)
                    {
                        foreach (var item in e.OldItems)
                        {
                            var inpc = item as IVobHandle;
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

        private void OnChildObjectChanged(IHandle handle, string propertyName)
        {
            l.Debug("Voc got child object changed: " + propertyName + " for " + handle);
            var ev = childPropertyChanged;
            if (ev != null) ev(handle, propertyName);
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
                        var inpc = item as IVobHandle;
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
                        var inpc = item as IVobHandle;
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
        public VobHandle<ChildType> Add(ChildType item)
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
            IEnumerable<KeyValuePair<string, VobHandle<ChildType>>> e = Dict;

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

        #region IVohac<ChildType>, INotifyingList<VobHandle<ChildType>>

        public IVohac<ChildType> Handles => this;

        INotifyingList<FilterType> INotifyingList<VobHandle<ChildType>>.Filter<FilterType>() => throw new NotImplementedException();

        VobHandle<ChildType>[] INotifyingCollection<VobHandle<ChildType>>.ToArray() => this.Dict.Values.ToArray();

        void ICollection<VobHandle<ChildType>>.Add(VobHandle<ChildType> item) => throw new NotSupportedException();

        void ICollection<VobHandle<ChildType>>.Clear() => throw new NotSupportedException();

        bool ICollection<VobHandle<ChildType>>.Contains(VobHandle<ChildType> item) => Dict.Values.Contains(item);

        void ICollection<VobHandle<ChildType>>.CopyTo(VobHandle<ChildType>[] array, int arrayIndex) => Dict.Values.CopyTo(array, arrayIndex);

        int ICollection<VobHandle<ChildType>>.Count => Dict.Count;

        bool ICollection<VobHandle<ChildType>>.IsReadOnly => Dict.IsReadOnly;

        bool ICollection<VobHandle<ChildType>>.Remove(VobHandle<ChildType> item) => throw new NotSupportedException();

        IEnumerator<VobHandle<ChildType>> IEnumerable<VobHandle<ChildType>>.GetEnumerator() => Dict.Values.GetEnumerator();

        event NotifyCollectionChangedHandler<VobHandle<ChildType>> INotifyCollectionChanged<VobHandle<ChildType>>.CollectionChanged {
            add { vhCollectionChanged += value; }
            remove { vhCollectionChanged -= value; }
        }

        event NotifyCollectionChangedHandler<VobReadHandle<ChildType>> INotifyCollectionChanged<VobReadHandle<ChildType>>.CollectionChanged {
            add {
                throw new NotImplementedException();
            }

            remove {
                throw new NotImplementedException();
            }
        }

        private NotifyCollectionChangedHandler<VobHandle<ChildType>> vhCollectionChanged;

        int IList<VobHandle<ChildType>>.IndexOf(VobHandle<ChildType> item) => throw new NotImplementedException();//return Dict.Values.Find(item);

        void IList<VobHandle<ChildType>>.Insert(int index, VobHandle<ChildType> item) => throw new NotImplementedException();

        void IList<VobHandle<ChildType>>.RemoveAt(int index) => throw new NotImplementedException();

        VobHandle<ChildType> IList<VobHandle<ChildType>>.this[int index] {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        #endregion

        public VobHandle<ChildType> this[string name] => this.Vob[name].GetHandle<ChildType>();


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

        IEnumerable<VobReadHandle<ChildType>> IVohac<ChildType>.Handles => throw new NotImplementedException();

        VobReadHandle<ChildType> IList<VobReadHandle<ChildType>>.this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        VobReadHandle<ChildType> IVohac<ChildType>.this[string name] => throw new NotImplementedException();

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

        VobReadHandle<ChildType>[] INotifyingCollection<VobReadHandle<ChildType>>.ToArray() => throw new NotImplementedException();
        public int IndexOf(VobReadHandle<ChildType> item) => throw new NotImplementedException();
        public void Insert(int index, VobReadHandle<ChildType> item) => throw new NotImplementedException();
        public void Add(VobReadHandle<ChildType> item) => throw new NotImplementedException();
        public bool Contains(VobReadHandle<ChildType> item) => throw new NotImplementedException();
        public void CopyTo(VobReadHandle<ChildType>[] array, int arrayIndex) => throw new NotImplementedException();
        public bool Remove(VobReadHandle<ChildType> item) => throw new NotImplementedException();
        IEnumerator<VobReadHandle<ChildType>> IEnumerable<VobReadHandle<ChildType>>.GetEnumerator() => throw new NotImplementedException();

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
#endif