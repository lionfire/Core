using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using LionFire.Assets;
using LionFire.Extensions.AssignFrom;
using LionFire.Collections;

namespace LionFire.ObjectBus
{
    public interface IVosContextResolver
    {
        Vob GetVobRoot(string path, VosContext context);
        Vob GetVobRoot(string path= null, string package = null, string location= null);
    }

    public class DefaultVosContextResolver : IVosContextResolver
    {
        public Vob GetVobRoot(string path, VosContext context)
        {
            return GetVobRoot(path, context.Package, context.Store);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">Leave null for root path</param>
        /// <param name="package"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public Vob GetVobRoot(string path= null, string package=null, string location=null)
        {
            Vob vob;

            if (package == null)
            {
                if (location != null)
                {
                    //l.Trace("GetVobRoot from Context: location specified but no package.");
                    vob = VosApp.Instance.Stores[location][path];
                }
                else // All null
                {
                    vob = VosContext.DefaultRoot[path];
                }

            }
            else // package != null
            {
                if (location == null)
                {
                    vob = VosApp.Instance.Packages[package][path];
                }
                else
                {
                    vob = VosApp.Instance.PackageStores[package][location][path];
                }
            }
            return vob;
        }

        private static ILogger l = Log.Get();

    }


    /// <summary>
    /// A context for VOS subpaths.  Typical assets have a path such as Map/Mapname.
    /// Optionally specify package and location to get a non-layered VobHandle, or else
    /// a layered VobHandle will be returned by Root.
    /// </summary>
    public class VosContext : IDisposable
    {
        #region Static

        // FUTURE: Fall back to global contextStack if thread stack is empty?
        private static ThreadLocal<Stack<VosContext>> contextStackThreadLocal = new ThreadLocal<Stack<VosContext>>(() => new Stack<VosContext>());
        private static Stack<VosContext> contextStack { get { return contextStackThreadLocal.Value; } }

        public static VosContext Current
        {
            get
            {
                var s = contextStack;
#if SanityChecks
                if (s == null)
                {
                    throw new UnreachableCodeException("contextStack == null");
                }
                //if (rootContext == null) throw new UnreachableCodeException("rootContext == null");
#endif
                return contextStack.Count > 0 ? contextStack.Peek() : rootContext;
            }
        }

        private static VosContext rootContext;

        static VosContext()
        {

#if AOT

//			System.Collections.Generic.Dictionary<int, System.Collections.Generic.Stack<LionFire.ObjectBus.VosContext>> d4 = null;

//						Dictionary<int, System.Collections.Generic.Stack<LionFire.ObjectBus.VosContext> > d1  = null;
//						NotifyCollectionChangedEventArgs<LionFire.ObjectBus.Mount> d3 = new NotifyCollectionChangedEventArgs<LionFire.ObjectBus.Mount> ();
//						LionFire.Collections.NotifyCollectionChangedHandler<LionFire.ObjectBus.Mount> d2 = null;
//						System.Threading.Interlocked.CompareExchange
//							<LionFire.Collections.NotifyCollectionChangedHandler > 
//								(ref d2, d2,d2);
			
			
			//			CompareExchange 
			//				(LionFire.Collections.NotifyCollectionChangedHandler`1<LionFire.ObjectBus.Mount>&,
			//				 LionFire.Collections.NotifyCollectionChangedHandler`1<LionFire.ObjectBus.Mount>,
			//				 LionFire.Collections.NotifyCollectionChangedHandler`1<LionFire.ObjectBus.Mount>)' 
#endif
			rootContext = new VosContext();
			
			DefaultRoot = V.ActiveData;

        }

        public static string DefaultPackage
        {
            get { return rootContext.Package; }
            set
            {
                rootContext.Package = value;
            }
        }

        public static string DefaultLocation
        {
            get { return rootContext.Store; }
            set
            {
                rootContext.Store = value;
            }
        }

        public static Vob DefaultRoot
        {
            get
            {
                return defaultRoot;
                //return rootContext.Root; 
            }
            set
            {
                //rootContext.Root = value;
                defaultRoot = value;
            }
        } private static Vob defaultRoot;

        #endregion

        #region Construction and Destruction

        public VosContext(string package = null, string location = null)
        {
            if (VosContext.Current != null)
            {
                // Alternative to using Overlay
                //this.AssignFrom(VosContext.Current, useICloneableIfAvailable: false);
                this.AssignFrom(VosContext.Current, AssignmentMode.Assign);
            }

            this.Package = package;
            this.Store = location;

            contextStack.Push(this);
        }

#if AOT
		private void AssignFrom(VosContext o, bool useICloneableIfAvailable = false)
		{
			this.DisableWritingToDisk = o.DisableWritingToDisk;
			this.IgnoreReadonly = o.IgnoreReadonly;
			this.Package = o.Package;

			this.resolver = o.Resolver;
			this.Root = o.Root;
			this.Store = o.Store;
		}
#endif

        public void Dispose()
        {
            if (object.ReferenceEquals(this, rootContext))
            {
                throw new InvalidOperationException("Disposing root context is not allowed.");
            }

            var vosContext = contextStack.Peek();
            if (Object.ReferenceEquals(vosContext, this))
            {
                contextStack.Pop();
            }
            else
            {
                throw new InvalidOperationException("Cannot dispose context when it is not on top of the context stack.");
            }
        }

        #endregion

        private void UpdateRoot()
        {
            this.Root = Resolver.GetVobRoot(null, this);
        }

        #region Resolver

        public IVosContextResolver Resolver
        {
            get
            {
                if (resolver == null)
                {
                    return defaultResolver;
                }
                return resolver;
            }
            set
            {
                if (resolver == value) return;
                if (resolver != default(IVosContextResolver)) throw new NotSupportedException("Resolver can only be set once.");
                resolver = value;
            }
        } private IVosContextResolver resolver;

        // Move to some DI hub?
        public static IVosContextResolver DefaultResolver { get { return defaultResolver; } }
        private static IVosContextResolver defaultResolver = new DefaultVosContextResolver();

        #endregion

        #region Context Properties

        #region Root

        public Vob Root
        {
            get
            {
                if (root == null)
                {
                    UpdateRoot();
                }
                return root;
            }
            private set { root = value; }
        } private Vob root;

        #endregion

        #region Package

        public string Package
        {
            get { return package; }
            set
            {
                if (package == value) return;
                package = value;
                UpdateRoot();
            }
        } private string package;

        #endregion

        #region Store

        public string Store
        {
            get { return store; }
            set
            {
                if (store == value) return;
                store = value;
                UpdateRoot();
            }
        } private string store;

        #endregion



        //#region PopulatingArchive

        ///// <summary>
        ///// Location name
        ///// </summary>
        //public string PopulatingArchive
        //{
        //    get { return populatingArchive; }
        //    set 
        //    {
        //        if (populatingArchive == locked && value != null) throw new AlreadyException("Property is frozen");
        //        if (populatingArchive != value && populatingArchive != null) throw new AlreadyException("Already set");
        //        if (value == null)
        //        {
        //            populatingArchive = locked;
        //        }
        //        else
        //        {
        //            populatingArchive = value;
        //        }
        //    }
        //} private string populatingArchive;
        //private const string locked = "((frozen))";

        //#endregion

        public bool DisableWritingToDisk { get; set; }

        #region IgnoreReadonly

        public bool IgnoreReadonly
        {
            get { return ignoreReadonly; }
            set
            {
                //#if DEV
                ignoreReadonly = value;
                //#else
                //                if (value) l.Trace("IgnoreReadonly ignoring setting to true, as it is not supported in this build.");
                //#endif
            }
        }
        //#if DEV
        private bool ignoreReadonly;
        //#endif

        #endregion

        #endregion

        #region Misc

        private static ILogger l = Log.Get();

        #endregion
    }
    public static class VosContextExtensions
    {
        #region Assets

#if !AOT
        public static AssetReference<T> ToAssetReference<T>(this string name) // MOVE?
            where T : class, IAsset
        {
            return AssetPaths.AssetPathFromAssetTypePath<T>(name);
        }
#endif

        public static Vob ToAssetVob(this string assetPath)
        {
            return VosContext.Current.Root[assetPath];
        }
        
        public static IVobHandle ToAssetVobHandle(this string assetPath, Type type)
		{
			if (VosContext.Current == null)
			{
				throw new UnreachableCodeException("VosContext.Current == null");
			}
			if (VosContext.Current.Root == null)
			{
				throw new UnreachableCodeException("VosContext.Current.Root == null");
			}
            // REvIEW - should this be in a asset subpath????
			return VosContext.Current.Root[assetPath].ToHandle(type);
		}
        public static VobHandle<T> ToAssetVobHandle<T>(this string assetPath)
            where T : class
        {
            if (VosContext.Current == null)
            {
                throw new UnreachableCodeException("VosContext.Current == null");
            }
            if (VosContext.Current.Root == null)
            {
                throw new UnreachableCodeException("VosContext.Current.Root == null");
            }
            return VosContext.Current.Root[assetPath].ToHandle<T>();
        }

        #endregion

        public static VobHandle<T> ToCurrentContext<T>(this VobHandle<T> h)
            where T : class
        {
            var ctx = VosContext.Current;

            var store = ctx.Store ?? h.EffectiveStore;
            var package = ctx.Package ?? h.EffectivePackage;

            if (h.EffectivePackage == package && h.EffectiveStore == store) {
                l.TraceWarn("OPTIMIZE - don't change Vob since store/pkg match");
                return h;
            }

            var subPath = h.Vob.GetPackageStoreSubPath();
            var vob = ctx.Root[subPath];
            return vob.ToHandle<T>();
        }

        private static ILogger l = Log.Get();

    }
}

