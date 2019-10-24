using LionFire.MultiTyping;
using LionFire.ObjectBus;
using LionFire.Ontology;
using LionFire.Referencing;
using LionFire.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Vos
{

    // RENAME to VobReference?
    //[JsonConvert(typeof(VosReferenceSerializationConverter))] FUTURE
    [LionSerializable(SerializeMethod.ByValue)]
    public class VosReference : LocalReferenceBase, IHas<IOBase>, ITypedReference
    {
        #region Scheme

        public const string UriSchemeDefault = "vos";
        public const string UriPrefixDefault = "vos://"; // TODO CHANGE to "vos:"
        public static readonly string[] UriSchemes = new string[] { UriSchemeDefault };
        public override IEnumerable<string> AllowedSchemes => UriSchemes;

        [Ignore]
        [Assignment(AssignmentMode.Ignore)]
        public override string Scheme
        {
            get { return UriSchemeDefault; }
        }

        #endregion

        IOBase IHas<IOBase>.Object => VosOBaseProvider.Instance.DefaultOBase;

        #region Construction and Implicit Construction

        public VosReference() { }

        public VosReference(string path)
        {
            this.Path = path;
        }
        public VosReference(params string[] pathComponents)
        {
            string path;
            for (int i = 0; i < pathComponents.Length; i++)
            {
                string chunk = pathComponents[i];
                pathComponents[i] = String.Concat(LionPath.SeparatorChar, chunk.TrimStart(LionPath.SeparatorChar));

                //if (i == pathComponents.Length - 1)
                //{
                //    // TODO: Clean trailing 2 slashes? probably don't need this.
                //}
            }

            path = String.Concat(pathComponents);
            this.Path = path;
        }

        public static implicit operator VosReference(string path)
        {
            return new VosReference(path);
        }

        #endregion

        #region Key

        public override string Key
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (Host != null)
                {
                    sb.Append(LionPath.HostDelimiter);
                    sb.Append(Host);
                }
                if (Port != null)
                {
                    sb.Append(":");
                    sb.Append(Port);
                }
                if (Path != null)
                {
                    sb.Append(Path);
                }
                if (Package != null)
                {
                    sb.Append("|");
                    sb.Append(Package);
                }
                if (Location != null)
                {
                    sb.Append("^");
                    sb.Append(Location);
                }
                return sb.ToString();
            }
        }
        protected void SetKey(string key)
        {
            if (String.IsNullOrWhiteSpace(key))
            {
                Reset();
                return;
            }
            int index = 0;
            if (key.StartsWith(LionPath.HostDelimiter))
            {
                index += LionPath.HostDelimiter.Length;
                Host = key.Substring(index, index + key.IndexOfAny(LionPath.Delimiters, index));
            }
            if (key[index] == LionPath.PortDelimiter)
            {
                index++;
                Port = key.Substring(index, index + key.IndexOfAny(LionPath.Delimiters, index));
            }
            if (key[index] == LionPath.PathDelimiter)
            {
                //index += PathDelimiter.Length; -- Keep the initial /
                //Path = value.Substring(index);
                Path = key.Substring(index, index + key.IndexOfAny(LionPath.Delimiters, index));
            }
            if (key[index] == VosPath.LayerDelimiter)
            {
                throw new NotImplementedException("TODO: Reimplement Layer delimiter");
                //index++;
                //Package = key.Substring(index, index + key.IndexOfAny(VosPath.Delimiters, index));
            }
        }

        #endregion


        private void Reset()
        {
            //this.Host = null;
            //this.Port = null;
            this.Path = null;
            this.Package = null;
            this.TypeName = null;
        }

        #region Convenience

        #region ObjectStoreProvider

        public override IOBus DefaultObjectStoreProvider
        {
            get
            {
                return VosOBaseProvider.Instance;
            }
        }

        public override IOBase ObjectStore
        {
            get
            {
                return VosOBaseProvider.Instance.DefaultOBase;
            }
        }


        #endregion

        public VBase Vos { get { return VosOBaseProvider.Instance.DefaultVos; } }

        public Vob Vob
        {
            get
            {
                return this.Vos[Path];
            }
        }

        

#if !AOT
        public override IHandle<T> GetHandle<T>(T obj)
        //where TValue : class//, new()
        {
            var vh = GetVobHandle<T>();
            if (obj != null)
            {
                vh.Object = obj;
            }
            return vh;
        }

#endif

        public VobHandle<T> GetVobHandle<T>()
            where T : class//, new()
        {
            //return new VobHandle<TValue>(this.GetVob());// OLD 
            //return this.GetVob().GetHandle<TValue>();
            return this.Vob.GetHandle<T>();
        }

        #endregion

        #region Misc

        public override string ToString()
        {
            //return String.Concat(UriPrefixDefault, Path);
            return String.Concat(UriPrefixDefault, Key);
        }

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

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        #endregion

    }

    //public static class VosReferenceExtensions
    //{
    //    public static Vob GetVob(this VosReference vosReference)
    //    {
    //        if (vosReference == null) throw new ArgumentNullException("vosReference");
    //        return Vos.Default[vosReference.Path];
    //    }
    //}
}
