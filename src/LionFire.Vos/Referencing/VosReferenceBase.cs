using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using LionFire.Referencing;

namespace LionFire.Vos
{
    public abstract class VosReferenceBase<TValue, TConcrete> : VosReferenceBase<TConcrete>, IVosReference
     where TConcrete : VosReferenceBase<TValue, TConcrete>
    {
    }

    public abstract class VosReferenceBase<TConcrete> : ReferenceBase<TConcrete>, IVosReference
        where TConcrete : VosReferenceBase<TConcrete>
    {
        // FUTURE: Vob reference inside here as a cache.
        //internal Vob Vob { get; set; }

        #region Construction and Implicit Construction

        #region Constructors

        public VosReferenceBase() { }


        public VosReferenceBase(string path, ImmutableList<KeyValuePair<string, string>> filters = null)
        {
            Path = path;
            Filters = filters;
        }
        public VosReferenceBase(IEnumerable<string> pathComponents, ImmutableList<KeyValuePair<string, string>> filters = null)
            : this(LionPath.Combine(pathComponents), filters)
        {
        }
        public VosReferenceBase(params string[] pathComponents)
            : this(LionPath.Separator + LionPath.Combine(pathComponents))
        {
        }
        public VosReferenceBase(ImmutableList<KeyValuePair<string, string>> filters = null, params string[] pathComponents)
            : this(LionPath.Separator + LionPath.Combine(pathComponents), filters)
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
            return new VosReference(referenceString);
        }

        public static VosReference FromRootName(string rootName = VosConstants.DefaultRootName)
            => rootName == VosConstants.DefaultRootName ? new VosReference("/") : new VosReference("/../" + rootName);

        public override TConcrete CloneWithPath(string newPath)
        {
            var result = this.Clone();
            result.path = newPath;
            return result;
        }

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

                this.AppendFilterKey(VosFilters.Layer.ToString(), "|", sb);
                this.AppendFilterKey(VosFilters.Location.ToString(), "^", sb);

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

        #region ProviderName

        /// <summary>
        /// Leave blank to base the reference off of the default provider
        /// </summary>
        [SetOnce]
        public override string Persister
        {
            get => providerName;
            set
            {
                if (providerName == value) return;
                if (providerName != default) throw new AlreadySetException();
                providerName = value;
            }
        }
        private string providerName;

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
                path = value;
            }
        }
        private string path;

        public string[] PathChunks
        {
            get => Path.ToPathArray();
            set => Path = LionPath.FromPathArray(value);
        }
        protected override void InternalSetPath(string path) => Path = path;

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

        #region Misc

        public static bool operator ==(VosReferenceBase<TConcrete> left, IVosReference right) => left?.Key == right?.Key;
        public static bool operator ==(VosReferenceBase<TConcrete> left, VosReferenceBase<TConcrete> right) => left?.Key == right?.Key;
        public static bool operator !=(VosReferenceBase<TConcrete> left, IVosReference right) => left?.Key != right?.Key;
        public static bool operator !=(VosReferenceBase<TConcrete> left, VosReferenceBase<TConcrete> right) => left?.Key != right?.Key;

        public override string ToString() => String.Concat(UriPrefixDefault, Key);

        public override bool Equals(object obj)
        {
            VosReference other = obj as VosReference;
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
