using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using LionFire.ObjectBus;
using LionFire.Ontology;

namespace LionFire.Vos
{


    public class VosReference<TValue> : VosReferenceBase<VosReference<TValue>>
    {
        #region Constructors

        public VosReference()
        {
        }

        public VosReference(params string[] pathComponents) : base(pathComponents)
        {
        }

        public VosReference(string path, ImmutableList<KeyValuePair<string, string>> filters = null) : base(path, filters)
        {
        }

        public VosReference(IEnumerable<string> pathComponents, ImmutableList<KeyValuePair<string, string>> filters = null) : base(pathComponents, filters)
        {
        }

        public VosReference(ImmutableList<KeyValuePair<string, string>> filters = null, params string[] pathComponents) : base(filters, pathComponents)
        {
        }

        #endregion

        public override Type Type => typeof(TValue);

    }

    /// <summary>
    /// </summary>
    /// <remarks>
    /// Persister corresponds to RootVob.RootName
    /// </remarks>
    public class VosReference : VosReferenceBase<VosReference>
    {


        #region Constructors

        public VosReference(params string[] pathComponents) : base(pathComponents)
        {
        }

        public VosReference(string path, ImmutableList<KeyValuePair<string, string>> filters = null) : base(path, filters)
        {
        }

        public VosReference(IEnumerable<string> pathComponents, ImmutableList<KeyValuePair<string, string>> filters = null) : base(pathComponents, filters)
        {
        }

        public VosReference(ImmutableList<KeyValuePair<string, string>> filters = null, params string[] pathComponents) : base(filters, pathComponents)
        {
        }

        public VosReference(Type type, string path, ImmutableList<KeyValuePair<string, string>> filters = null)
        {
            Type = type;
            Path = path;
            Filters = filters;
        }

        public VosReference(VosReference cloneSource, string newPath)
        {
            Type = cloneSource.Type;
            Path = newPath;
            Filters = cloneSource.Filters;
        }

        #endregion

        #region Implicit construction

        public static implicit operator VosReference(string path) => new VosReference(path);
        public static implicit operator VosReference(Vob vob) => (VosReference)vob.Reference;

        #endregion

        #region Type

        public override Type Type { get; }
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
    }

}
