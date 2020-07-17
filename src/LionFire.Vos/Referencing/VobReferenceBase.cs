using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using LionFire.Referencing;

namespace LionFire.Vos
{
    public abstract class VobReferenceBase<TValue, TConcrete> : VobReferenceBase<TConcrete>, IVobReference, ITypedReference
     where TConcrete : VobReferenceBase<TValue, TConcrete>
    {
        public override Type Type => typeof(TValue);
    }

    public abstract class VobReferenceBase<TConcrete> : ReferenceBase<TConcrete>, IVobReference
        where TConcrete : VobReferenceBase<TConcrete>
    {
        // FUTURE: Vob reference inside here as a cache.
        //internal Vob Vob { get; set; }

        #region Construction and Implicit Construction

        #region Constructors

        public VobReferenceBase() { }

        public VobReferenceBase(string path, ImmutableList<KeyValuePair<string, string>> filters = null)
        {
            Path = path;
            Filters = filters;
        }
        public VobReferenceBase(IEnumerable<string> pathComponents, ImmutableList<KeyValuePair<string, string>> filters = null, bool? absolute = null)
            : this(VosPath.ChunksToString(pathComponents, absolute), filters)
        {
        }
        public VobReferenceBase(params string[] pathComponents)
            : this(VosPath.ChunksToString(pathComponents))
        {
        }
        public VobReferenceBase(ImmutableList<KeyValuePair<string, string>> filters = null, params string[] pathComponents)
            : this(VosPath.ChunksToString(pathComponents), filters)
        {
        }

        #endregion

        public static IReference TryGetFromString(string referenceString)
        {
            var scheme = referenceString.GetUriScheme();
            if (scheme != UriSchemeDefault)
            {
                return null;
            }
            return new VobReference(referenceString);
        }

        public static VobReference FromRootName(string rootName = VosConstants.DefaultRootName)
            => rootName == VosConstants.DefaultRootName ? new VobReference("/") : new VobReference("/../" + rootName);

        //public override TConcrete CloneWithPath(string newPath)
        //{
        //    base.CloneWithPath
        //    var result = this.Clone();
        //    result.path = newPath;
        //    return result;
        //}

        #endregion

        #region OLD

        //#region GetHandle

        //// Better/direct versions of the ObjectBus ToHandle

        //public VobHandle<object> GetHandle(/*object obj = null*/) => new VobHandle<object>(this);
        //public VobHandle<T> GetHandle<T>(/*T obj = default(T)*/) => new VobHandle<T>(this);
        ////public VobHandle<T> ToHandle<T>(this T obj) => throw new NotImplementedException("FUTURE: if obj != null, create a NamedObjectHandle and assign a random key");

        //public VobReadHandle<object> GetReadHandle(/*object obj = null*/) => new VobReadHandle<object>(this);
        //public VobReadHandle<T> GetReadHandle<T>(/*T obj = default(T)*/) => new VobReadHandle<T>(this);
        ////public VobReadOnlyHandle<T> ObjectToReadHandle<T>(this T obj) => throw new NotImplementedException("FUTURE: if obj != null, create a NamedObjectHandle and assign a random key");

        //#endregion

        #endregion

        #region IReference Implementation

        #region Scheme

        public const string UriSchemeDefault = "vos";
        public const string UriPrefixDefault = "vos:";

        public override IEnumerable<string> AllowedSchemes
        {
            get { yield return UriSchemeDefault; }
        }

        public override string Scheme => UriSchemeDefault;
        public static readonly string[] UriSchemes = new string[] { UriSchemeDefault };

        #endregion

        #region Key

        public override string Key
        {
            get
            {
                var sb = new StringBuilder();

                if (Path != null)
                {
                    sb.Append(Path);
                }

                (this).AppendFilterKey(VosFilters.Layer.ToString(), "|", sb);
                (this).AppendFilterKey(VosFilters.Location.ToString(), "^", sb);

                return sb.ToString();
            }
            protected set
            {
                if (Path != null) throw new AlreadySetException();
                //if (string.IsNullOrWhiteSpace(value))
                //{
                //    Reset();
                //    return;
                //}
                int index = 0;

                if (value[index] == LionPath.PathDelimiter)
                {
                    //index += PathDelimiter.Length; -- Keep the initial /
                    //Path = value.Substring(index);
                    Path = value.Substring(index, index + value.IndexOfAny(LionPath.Delimiters, index));
                }
                if (value[index] == VosPath.LayerDelimiter)
                {
                    throw new NotImplementedException("TODO: Reimplement Layer delimiter");
                    //index++;
                    //Package = value.Substring(index, index + value.IndexOfAny(VosPath.Delimiters, index));
                }
            }
        }

        #endregion

        #region Persister

        /// <summary>
        /// Leave blank to base the reference off of the default provider
        /// </summary>
        [SetOnce]
        public override string Persister // Set by Path
        {
            get =>  persister ;
             
            //set
            //{
            //    if (persister == value) return;
            //    if (persister != default) throw new AlreadySetException();
            //    persister = value;
            //}
        }
        private string persister;

        #endregion

        #region Path

        [SetOnce]
        public bool IsAbsolute
        {
            get => true; // TODO - support relative references?
            set { } // TODO
        }

        [SetOnce]
        public override string Path
        {
            get => path;
            set
            {
                if (path == value) return;
                if (path != default) throw new AlreadySetException();
                DoSetPath(value);
            }
        }
        private string path;

        public string[] PathChunks
        {
            get => Path.ToPathArray();
            set => Path = LionPath.FromPathArray(value);
        }
        protected override void InternalSetPath(string path) => DoSetPath(path);

        private void DoSetPath(string path)
        {
            this.path = path;
            persister = VosPath.GetRootNameForPath(path);
        }

        #endregion

        #endregion

        public abstract Type Type { get; }

        #region VosFilters

        public ImmutableList<KeyValuePair<string, string>> Filters { get; set; }

        //public string Package { get; set; }

        /// <summary>
        /// Mount name
        /// </summary>
        //public string Location { get; set; }

        #endregion

        public IVobReference Reference => this;

        public VobReference GetRoot() => new VobReference(VosPath.GetRootOfPath(this.Path));

        #region Misc

        public static bool operator ==(VobReferenceBase<TConcrete> left, IVobReference right) => left?.Key == right?.Key;
        public static bool operator ==(VobReferenceBase<TConcrete> left, VobReferenceBase<TConcrete> right) => left?.Key == right?.Key;
        public static bool operator !=(VobReferenceBase<TConcrete> left, IVobReference right) => left?.Key != right?.Key;
        public static bool operator !=(VobReferenceBase<TConcrete> left, VobReferenceBase<TConcrete> right) => left?.Key != right?.Key;

        public override string ToString() => String.Concat(UriPrefixDefault, Key);

        public override bool Equals(object obj)
        {
            VobReference other = obj as VobReference;
            if (other == null)
            {
                return false;
            }

            if (!Key.Equals(other.Key))
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode() => Key.GetHashCode();

        #endregion

    }

}
