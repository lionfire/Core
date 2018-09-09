using LionFire.ObjectBus;
using LionFire.Referencing;
using LionFire.Serialization;
using System;

namespace LionFire.Vos
{
    // TODO: Ensure set once on all properties
    // TODO: Make it freezable?  Or can only set stuff at ctor/factory time? (Otherwise, null values could be changed.  Perhaps set to empty strings/dedicated invalid values to "freeze")
    [SetOnce]
    [LionSerializable(SerializeMethod.ByValue)]
    public abstract class VReferenceBase : ReferenceBase, IVReference
    {
        #region Construction

        #region Copy From

        protected override void CopyFrom(IReference other, string newPath = null)
        {
            base.CopyFrom(other, newPath);
            if (other is IVReference vref)
            {
                Package = vref.Package;
                TypeName = vref.TypeName;
            }
        }

        #endregion

        #endregion


        #region Uri and Key

        public virtual string Uri => ToUriString();

        public string ToUriString()
        {
            var reference = this;
            return String.Format("{0}://{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}",
    reference.Scheme, reference.Host,
    /*{2}*/reference.Port == null ? "" : ReferenceConstants.PortSeparator, (reference.Port ?? ""),
   /*{4}*/ reference.Package == null ? "" : VReferenceConstants.LayerSeparator, (reference.Package ?? ""),
       /*{6}*/           reference.Location == null ? "" : VReferenceConstants.LocationSeparator, (reference.Location ?? ""),
                /*{8}*/reference.Path,
    reference.TypeName == null ? "" : VReferenceConstants.TypeNameSeparator, (reference.TypeName ?? "")
    );
        }

        #region Key

        [Ignore]
        public override string Key => ToString();

#if AOT
		object IKeyed.Key { get { return Key; } }
#endif


        //    public string Key
        //    {
        //        get
        //        {
        //            return String.Format("{0}://{1}{2}{3}{4}{5}{6}{7}{8}",
        //reference.Scheme, reference.Host,
        //reference.Port == null ? "" : ReferenceConstants.PortSeparator, (reference.Port ?? ""),
        //reference.Layer == null ? "" : ReferenceConstants.LayerSeparator, (reference.Layer ?? ""),
        //reference.Path,
        //reference.TypeName == null ? "" : ReferenceConstants.TypeNameSeparator, (reference.TypeName ?? "")
        //);
        //        }
        //        set
        //        {
        //            if(value.Contains(':'))
        //            {
        //                string[] schemeAndRemainder = value.Split(':');

        //                if(schemeAndRemainder.Length > 2)
        //                {
        //                    throw new ArgumentException("A valid uri has only one colon");
        //                }

        //                if(CanSetScheme)
        //                {
        //                this.Scheme = schemeAndRemainder[0];
        //                }
        //                else if(VerifyScheme)
        //                {
        //                    ValidateScheme(schemeAndRemainder[0]);
        //                }
        //                value = schemeAndRemainder[1];
        //            }

        //        }
        //    }

        #endregion

        #endregion

        #region Host

        #region Host

        [SerializeDefaultValue(false)]
        [SetOnce]
        public override string Host
        {
            get => host;
            set
            {
                if (host == value)
                {
                    return;
                }

                if (host != default(string))
                {
                    throw new AlreadySetException();
                }

                host = value;
            }
        }
        private string host;

        #endregion

        public bool IsLocalhost
        {
            get
            {
                if (Host == null)
                {
                    return true; // REVIEW: Return null instead
                }

                if (ReferenceConstants.IsLocalhost(Host.ToLowerInvariant()))
                {
                    return true;
                }

                // TODO INCOMPLETE: Determine whether it resolves to a cached list of host names / ip addresses

                return false;
            }
        }

        #region Port

        [SerializeDefaultValue(false)]
        [SetOnce]
        public override string Port
        {
            get => port;
            set
            {
                if (port == value)
                {
                    return;
                }

                if (port != default(string))
                {
                    throw new AlreadySetException();
                }

                port = value;
            }
        }
        private string port;

        #endregion

        #endregion

        #region Path

        [SetOnce]
        public override string Path
        {
            get => path;
            set
            {
                if (path == value)
                {
                    return;
                }

                if (path != default(string))
                {
                    throw new AlreadySetException();
                }

                path = value;
            }
        }
        private string path;

        public string[] GetPathArray() => LionPath.ToPathArray(Path);

        //// IDEA: MEMOPTIMIZE - some kind of linked array for paths: internally works with memory addresses as nodes instead of strings, like Vobs
        //public string[] PathChunks
        //{
        //} private List<string>

        public string Name => LionPath.GetName(Path);//var chunks = LionPath.ToPathArray(Path);//if(chunks==null || chunks.Length ==0) return null;//return chunks[chunks.Length - 1];
        #endregion

        #region Package, Location, Type

        [SerializeDefaultValue(false)]
        public string Package
        {
            get;
            set;
        }

        [SerializeDefaultValue(false)]
        public string Location
        {
            get;
            set;
        }
        /// <summary>
        /// Type.FullName (todo: a property with just Name)
        /// </summary>
		[SerializeDefaultValue(false)]
        [Ignore]
        public string TypeName
        {
            get => typeName;
            set
            {
                if (typeName == value)
                {
                    return;
                }

                typeName = value;
                type = null; // will recalculate on demand
            }
        }

        //[Ignore] // REVIEW: Make Type serializable instead of TypeName? To let serializers do whatever is best for them
        [SerializeDefaultValue(false)]
        public Type Type
        {
            get
            {
                if (type == null && typeName != null)
                {
                    type = Type.GetType(typeName);
                    if (type == null)
                    {
                        throw new Exception("Failed to resolve typeName: " + typeName);
                    }
                }
                return type;
            }
            set
            {
                if (type == value)
                {
                    return;
                }

                type = value;
                typeName = type.FullName;
            }
        }
        private string typeName;

        private Type type;

        #endregion

        #region IOBase

        public virtual IOBaseProvider DefaultObjectStoreProvider
        {
            get
            {
                l.Trace("UNTESTED - ReferenceBase.ObjectStoreProvider for Scheme: " + Scheme);
                return SchemeBroker.Instance[Scheme].FirstOrDefault();
            }
        }
        /// <summary>
        /// Can be set if there is only ever one OBase for this reference type
        /// </summary>
		public virtual IOBase ObjectStore => null;
        //public virtual IReference Resolve()
        //{
        //    return this;
        //}

        #endregion

        #region ToHandle

#if !AOT
        public virtual IHandle<T> GetHandle<T>(T obj = null)
            where T : class//, new()
=> HandleFactory<T>.GetHandle(this, obj);

#endif

        // OLD Extension methods
        //#if !AOT
        //        public static IHandle<T> ToHandle<T>(this IReference reference, T obj = null)
        //            where T : class//, new()
        //        {
        //            return HandleFactory<T>.GetHandle(reference, obj);
        //        }

        //#endif

        #endregion

    }
}
