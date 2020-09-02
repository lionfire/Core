using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using LionFire.ObjectBus;
using LionFire.Ontology;
using LionFire.Referencing;

namespace LionFire.Vos
{


    public class VobReference<TValue> : VobReferenceBase<VobReference<TValue>>
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

        public VobReference(IEnumerable<string> pathComponents, ImmutableList<KeyValuePair<string, string>> filters = null) : base(pathComponents, filters)
        {
        }

        public VobReference(ImmutableList<KeyValuePair<string, string>> filters = null, params string[] pathComponents) : base(filters, pathComponents)
        {
        }

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

        public override Type Type => typeof(TValue);

    }

    /// <summary>
    /// </summary>
    /// <remarks>
    /// Persister corresponds to RootVob.RootName
    /// </remarks>
    public class VobReference : VobReferenceBase<VobReference>
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

        public VobReference(Type type, string path, ImmutableList<KeyValuePair<string, string>> filters = null)
        {
            Type = type;
            Path = path;
            Filters = filters;
        }

        public static VobReference WithNewPath(VobReference cloneSource, string newPath) 
            => new VobReference(cloneSource.Type, newPath, cloneSource.Filters);

        #endregion

        #region Implicit construction

        public static implicit operator VobReference(string path) => new VobReference(path);
        public static implicit operator VobReference(Vob vob) => (VobReference)vob.Reference;
        public static implicit operator VobReference(ArraySegment<string> segments) => segments.Count == 0 ? "/" : new VobReference(segments);

        #endregion

        #region Type

        public override Type Type { get; }
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


        public VobReference this[string subPath] => this.GetChild(subPath);
    }

}
