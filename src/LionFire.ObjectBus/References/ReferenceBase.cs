using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.Serialization;
using LionFire.Collections;
using LionFire.Meta;

namespace LionFire.ObjectBus
{

    public abstract class ReferenceBase<ConcreteType> : ReferenceBase
    {
        public new ConcreteType GetChild(string subPath)
        {
            return (ConcreteType)base.GetChild(subPath);
        }
    }

    // TODO: Ensure set once on all properties
    // TODO: Make it freezable?  Or can only set stuff at ctor/factory time? (Otherwise, null values could be changed.  Perhaps set to empty strings/dedicated invalid values to "freeze")
    [SetOnce]
	[LionSerializable(SerializeMethod.ByValue)]
    public abstract class ReferenceBase : IReference
	{            

        #region Copy From

        protected void CopyFrom(IReference other, string newPath = null)
        {
            this.Host = other.Host;
            this.Port = other.Port;
            this.Path = newPath ?? other.Path;
            this.Package = other.Package;
            this.TypeName = other.TypeName;
        }

        #endregion

        #region Scheme

        public abstract string Scheme {
			get;
			set;
		}

		public virtual bool CanSetScheme { get { return false; } }

		public static bool VerifyScheme = true;

        public virtual void ValidateScheme(string scheme)
        {
            if (scheme != Scheme)
            {
                throw new ArgumentException("Scheme '"
                    + (scheme ?? "null")
                    + "'not valid for this reference type: " + this.GetType().Name);
            }
        }

        #endregion

        #region Uri and Key

        public virtual string Uri {
			get {
				return ReferenceUtils.ToUriString(this);
			}
		}

        #region Key

        [Ignore]
        public virtual string Key {
			get {
				return this.ToString();
			}
			set {
                // Parse from key
				throw new NotImplementedException("set_Key");
			}
		}
        //#if AOT
		object IKeyed.Key { get { return Key; } }
        //#endif


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
        public string Host
        {
            get { return host; }
            set
            {
                if (host == value) return;
                if (host != default(string)) throw new AlreadySetException();
                host = value;
            }
        } private string host;

        #endregion

		public bool IsLocalhost {
			get {
				if(Host == null)
					return true;
				if(ReferenceUtils.LocalhostStrings.Contains(Host.ToLowerInvariant()))
					return true;

				// TODO INCOMPLETE: Determine whether it resolves to a cached list of host names / ip addresses

				return false;
			}
		}

        #region Port

        [SerializeDefaultValue(false)]
        [SetOnce]
        public string Port
        {
            get { return port; }
            set
            {
                if (port == value) return;
                if (port != default(string)) throw new AlreadySetException();
                port = value;
            }
        } private string port;

        #endregion

        #endregion

        #region Path

        [SetOnce]
        public virtual string Path
        {
            get { return path; }
            set
            {
                if (path == value) return;
                if (path != default(string)) throw new AlreadySetException();
                path = value;
            }
        } private string path;

		public string[] GetPathArray()
		{
			return VosPath.ToPathArray(Path);
		}

        //// IDEA: MEMOPTIMIZE - some kind of linked array for paths: internally works with memory addresses as nodes instead of strings, like Vobs
        //public string[] PathChunks
        //{
        //} private List<string>

		public string Name {
			get {
				return VosPath.GetName(Path);

				//var chunks = VosPath.ToPathArray(Path);
				//if(chunks==null || chunks.Length ==0) return null;
				//return chunks[chunks.Length - 1];
			}
		}
        #endregion
        
        #region Package, Location, Type

        [SerializeDefaultValue(false)]
        public string Package {
			get;
			set;
		}

		[SerializeDefaultValue(false)]
        public string Location {
			get;
			set;
		}
        /// <summary>
        /// Type.FullName (todo: a property with just Name)
        /// </summary>
		[SerializeDefaultValue(false)]
		[Ignore]
		public string TypeName {
			get { return typeName; }
			set {
				if(typeName == value)
					return;
				this.typeName = value;
				this.type = null; // will recalculate on demand
			}
		}
		
        //[Ignore] // REVIEW: Make Type serializable instead of TypeName? To let serializers do whatever is best for them
		[SerializeDefaultValue(false)]
        public Type Type {
			get {
				if(type == null && typeName != null) {
					type = Type.GetType(typeName);
					if(type == null) {
						throw new Exception("Failed to resolve typeName: " + typeName);
					}
				}
				return type;
			}
			set {
				if(type == value)
					return;
				this.type = value;
				this.typeName = type.FullName;
			}
		} private string typeName;

		private Type type;

        #endregion

        #region IOBase

        public virtual IOBaseProvider DefaultObjectStoreProvider {
			get {
				l.Trace("UNTESTED - ReferenceBase.ObjectStoreProvider for Scheme: " + Scheme);
				return SchemeBroker.Instance[Scheme].FirstOrDefault();
			}
		}
        /// <summary>
        /// Can be set if there is only ever one OBase for this reference type
        /// </summary>
		public virtual IOBase ObjectStore { get { return null; } }
        //public virtual IReference Resolve()
        //{
        //    return this;
        //}

        #endregion


        #region ToHandle

#if !AOT
        public virtual IHandle<T> GetHandle<T>( T obj = null)
            where T : class//, new()
        {
            return HandleFactory<T>.GetHandle(this, obj);
        }

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

        #region Children

        public virtual IReference GetChild(string subPath)
        {
            // Use ctor instead? Or reference factory?

            var result = (ReferenceBase)Activator.CreateInstance(this.GetType());
            result.CopyFrom(this, this.Path + String.Concat(VosPath.SeparatorChar.ToString(), subPath));
            return result;
        }

        //public IReference GetChildSubpath(params string[] subpath)
        public IReference GetChildSubpath(IEnumerable<string> subpath)
        {
            var sb = new StringBuilder();
            bool isFirst = true;
            foreach (var subpathChunk in subpath)
            {
                if (isFirst) isFirst = false;
                else { sb.Append("/"); }
                sb.Append(subpathChunk);
            }
            return GetChild(sb.ToString());
        }

        #endregion

        #region Misc

        #region Object Overrides

        public override bool Equals(object obj)
        {
            var other = obj as IReference;
            if (other == null)
                return false;
            return this.Key == other.Key;
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        #endregion

        private static ILogger l = Log.Get();

        #endregion

	}
}
