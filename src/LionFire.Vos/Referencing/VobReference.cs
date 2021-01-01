using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using LionFire.ObjectBus;
using LionFire.Ontology;
using LionFire.Referencing;

namespace LionFire.Vos
{
    // TODO: Make VobReference inherit from VobReference<object>

    [TreatAs(typeof(IVobReference))]
    public class VobReference<TValue> : VobReferenceBase<VobReference<TValue>>, IVobReference<TValue>
    {
        public override Type Type => typeof(TValue);
        public override IVobReference<T> ForType<T>()
            => typeof(T) == typeof(TValue) ? (IVobReference<T>)this : new VobReference<T>(Path);

        #region Constructors

        public VobReference()
        {
        }

        public VobReference(params string[] pathComponents) : base(pathComponents)
        {
        }

        public VobReference(string path, ImmutableList<KeyValuePair<string, string>> filters = null) : base(path, filters)
        {
        }
       
        public VobReference(IEnumerable<string> pathComponents, ImmutableList<KeyValuePair<string, string>> filters = null, bool? absolute = null) : base(pathComponents, filters, absolute)
        {
        }

        public VobReference(ImmutableList<KeyValuePair<string, string>> filters = null, params string[] pathComponents) : base(filters, pathComponents)
        {
        }

        #region Implicit construction

        public static implicit operator VobReference<TValue>(string path) => new VobReference<TValue>(path);
        public static implicit operator VobReference<TValue>(Vob vob) => (VobReference<TValue>)vob.Reference;
        public static implicit operator VobReference<TValue>(ArraySegment<string> segments) => segments.Count == 0 ? "/" : new VobReference<TValue>(segments);

        #endregion

        #endregion

        #region Persister

        [SetOnce]
        public override string Persister
        {
            get => persister;
            set
            {
                if (persister == value) return;
                if (persister != default) throw new AlreadySetException();
                persister = value;
            }
        }
        private string persister;

        #endregion



    }

    [TreatAs(typeof(IVobReference))]
    public class VobReference : VobReference<object>
    {
        #region Constructors

        public VobReference()
        {
        }
        public VobReference(params string[] pathComponents) : base(pathComponents)
        {
        }

        public VobReference(string path, ImmutableList<KeyValuePair<string, string>> filters = null) : base(path, filters)
        {
        }

        public VobReference(IEnumerable<string> pathComponents, ImmutableList<KeyValuePair<string, string>> filters = null, bool? absolute = null) : base(pathComponents, filters, absolute)
        {
        }

        public VobReference(ImmutableList<KeyValuePair<string, string>> filters = null, params string[] pathComponents) : base(filters, pathComponents)
        {
        }

        //public VobReference(Type type, string path, ImmutableList<KeyValuePair<string, string>> filters = null)
        //{
        //    Type = type;
        //    Path = path;
        //    Filters = filters;
        //}

        //public static VobReference WithNewPath(VobReference cloneSource, string newPath)
        //=> new VobReference(cloneSource.Type, newPath, cloneSource.Filters);
        public static VobReference WithNewPath(VobReference cloneSource, string newPath)
            => new VobReference(newPath, cloneSource.Filters);

        #region Implicit construction

        public static implicit operator VobReference(string path) => new VobReference(path);
        public static implicit operator VobReference(Vob vob) => (VobReference)vob.Reference;
        public static implicit operator VobReference(ArraySegment<string> segments) => segments.Count == 0 ? "/" : new VobReference(segments);

        #endregion

        #endregion

    }

#if OLD
    /// <summary>
    /// </summary>
    /// <remarks>
    /// Persister corresponds to RootVob.RootName
    /// </remarks>
    public class VobReference : VobReferenceBase<VobReference>
    {
        public override IVobReference<T> ForType<T>() => new VobReference<T>(Path);

    #region Constructors

        public VobReference()
        {
        }
        public VobReference(params string[] pathComponents) : base(pathComponents)
        {
        }

        public VobReference(string path, ImmutableList<KeyValuePair<string, string>> filters = null) : base(path, filters)
        {
        }

        public VobReference(IEnumerable<string> pathComponents, ImmutableList<KeyValuePair<string, string>> filters = null, bool? absolute = null) : base(pathComponents, filters, absolute)
        {
        }

        public VobReference(ImmutableList<KeyValuePair<string, string>> filters = null, params string[] pathComponents) : base(filters, pathComponents)
        {
        }

        //public VobReference(Type type, string path, ImmutableList<KeyValuePair<string, string>> filters = null)
        //{
        //    Type = type;
        //    Path = path;
        //    Filters = filters;
        //}

        //public static VobReference WithNewPath(VobReference cloneSource, string newPath)
            //=> new VobReference(cloneSource.Type, newPath, cloneSource.Filters);
        public static VobReference WithNewPath(VobReference cloneSource, string newPath)
            => new VobReference(newPath, cloneSource.Filters);

    #endregion

    #region Implicit construction

        public static implicit operator VobReference(string path) => new VobReference(path);
        public static implicit operator VobReference(Vob vob) => (VobReference)vob.Reference;
        public static implicit operator VobReference(ArraySegment<string> segments) => segments.Count == 0 ? "/" : new VobReference(segments);

    #endregion

    #region Type

        public override Type Type => typeof(object);
        //public override Type Type { get; }
        //[SetOnce]
        //public Type Type
        //{
        //    get => type;
        //    set
        //    {
        //        if (type == value) return;
        //        if (type != default(Type)) throw new AlreadySetException();
        //        type = value;
        //    }
        //}
        //private Type type;

    #endregion


        public new VobReference this[string subPath] => this.GetChild(subPath);
    }
#endif
}
