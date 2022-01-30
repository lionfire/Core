#define ConcurrentHandles
#define WARN_VOB
//#define INFO_VOB
#define TRACE_VOB

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.Collections;
using LionFire.DependencyInjection;
using LionFire.Extensions.ObjectBus;
using LionFire.Instantiating;
using LionFire.MultiTyping;
using LionFire.ObjectBus;
using LionFire.Ontology;
using LionFire.Referencing;
using LionFire.Structures;
using LionFire.Types;
using LionFire.Vos;
using LionFire.Vos.Internals;
using LionFire.Vos.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LionFire.Vos
{

    // ----- Mount notes -----
    //        //public INotifyingReadOnlyCollection<Mount> Mounts { get { return mounts; } }
    //        ////private MultiBindableDictionary<string, Mount> mountsByPath = new MultiBindableDictionary<string, Mount>();
    //        //private MultiBindableCollection<Mount> mounts = new MultiBindableCollection<Mount>();

    //        // Disk location: ValorDir/DataPacks/Official/Valor1/Core.zip
    //        // /Valor/Packages/Nextrek/Maps/Bronco.Map
    //        // /Valor/Packages/Nextrek/Maps/Bronco/Settings/Vanilla
    //        // /Valor/Packages/Nextrek/Maps/Bronco/Settings/INL
    //        // Disk location: ValorDir/DataPacks/Official/Expansion1.zip
    //        // Disk location: ValorDir/DataPacks/Official/Maps/MapPack1.zip
    //        // /Valor/Packages/Nextrek/Maps/Chaos.Map
    //        // /Valor/Packages/Nextrek/Maps/Chaos/Settings/Cibola
    //        // Disk location: ValorDir/DataPacks/Official/Maps/MapPack2.zip

    //        // Disk location: ValorDir/DataPacks/Official/Expansion1/Core.zip
    //        // Disk location: ValorDir/DataPacks/Official/Expansion2/Core.zip

    //        // Disk location: ValorDir/DataPacks/Downloaded/Vanilla2.zip
    //        // Disk location: ValorDir/DataPacks/Mods/TAMod/Core.1.0.zip
    //        // Disk location: ValorDir/DataPacks/Mods/TAMod/Core.1.1.zip
    //        // Disk location: ValorDir/DataPacks/Mods/TAMod/MapPack1.zip

    //        // /Valor/Packages/Nextrek/Maps/Bronco/Settings/Vanilla2

    //        // Data Packs Menu:
    //        //   - Core
    //        //      - Base
    //        //   - Custom
    //        //      - Vanilla2

    //        // Disk location: ValorDir/Data (rooted at Valor/Packages)/Nextrek/Maps/Bronco/Settings/Vanilla3

    //        // Disk location: ValorDir/Data

    //        // 3 mounts:
    //        // Valor/Data | file:///c:/Program Files/LionFire/Valor/Data
    //        // Valor/Data | file:///c:/Program Files/LionFire/Valor/DataPacks

    // ---------------- Other notes
    //    // vos://host/path/to/node%Type
    //    // vos://host/path/to/.node.Type.ext
    //    // vos://host/path/to/node%Type[instanceName]
    //    // vos://host/path/to/node/.TYPEs/instanceName
    //    // vos://host/path/to/node%Type[] - all instances, except main instance

    //public class PathTree<T>
    //    where T : class
    //{
    //    public T this[string subpath] => this[subpath];

    //    public T GetChild(string subpath) => GetChild(subpath.ToPathArray(), 0);

    //    public T QueryChild(string reference) => QueryChild(subpath.ToPathArray(), 0);

    //}

    /// <summary>
    /// 
    /// VOB:
    ///  knows where it is (has reference)
    ///  keeps track of its children (is an active node in a VosBase)
    ///  is invalidated when mounts change
    ///  has handles to objects in multiple layers
    ///  has handles to its multi-type objects 
    ///   - can be locked to a uni-type object
    /// 
    /// Dynamic Object.  
    /// 
    /// Brainstorm:
    /// 
    /// Dob: created automatically as a multitype object (if needed)
    ///     UnitType (references components of types)
    /// Pob: Partial object.  Object piece.  Can have subchildren
    ///       - Version (optional decorator, but may be required by app)
    ///          - Could be implemented as a field, then implement the read-only IMultiType to provide OBus access to it
    ///       - Personal notes (saved in personal layer)
    ///       
    ///  - For child objects: is it an 'is a' or 'has a' relationship?  Containment vs reference / decorators
    ///     - allow nested subobjects?????
    ///  
    ///  Anyobject support: hooray for POCO
    /// 
    ///  Can Proxies/mixins help?
    ///   - load an object, get a mixin back?  Doesn't seem so hard now that I think I've done it
    /// </summary>
    /// <remarks>
    /// Handle overrides:
    ///  - (TODO) Object set/get.
    ///    - get_Object - for Vob{T} this will return the object as T, if any.  Otherwise, it may return a single object, or a MultiType object
    ///    - set_Object - depending on the situation, it may assign into a MultiType object
    /// </remarks>
    public partial class Vob : // RENAME - MinimalVob?
                               //CachingHandleBase<Vob, Vob>,
                               //CachingChildren<Vob>,
                               //IHasHandle,
                               //H, // TODO - make this a smarter handle.  The object might be a dynamically created MultiType for complex scenarios
        IReferencable
#if AOT
		IParented
#else
        , IParentable<IVob>
#endif
        , IVob
        , IVobInternals
       , IMultiTypable
        , IHierarchyOfKeyedOnDemand<IVob> // TODO
    //, SReadOnlyMultiTyped // FUTURE?
    {

        #region Identity

        public string Name => name;
        private readonly string name;

        #region Relationships

        #region Parent

#if AOT
        object IParented.Parent { get { return this.Parent; } set { throw new NotSupportedException(); } }
#endif

        #region Parent

        public IVob Parent
        {
            get => parent;
            set => throw new NotSupportedException();
        }
        private readonly IVob parent;

        #endregion

        #endregion

        #endregion

        #endregion

        #region Construction

        public Vob(IVob parent, string name)
        {
            if (this is RootVob rootVob)
            {
#if DEBUG
                if (!string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException("Name must be null or empty for RootVobs."); // Redundant check
#endif
            }
            else
            {
                if (parent == null)
                {
                    throw new ArgumentNullException($"'{nameof(parent)}' must be set for all non-RootVob Vobs.");
                }

                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentNullException($"'{nameof(name)}' must not be null or empty for a non-root Vob");
                }
            }

            this.parent = parent;
            this.name = name ?? "";
        }

        //public Vob(VBase vos, Vob parent, string name) : this(parent,name)
        //{
        //    if (vos == null)
        //    {
        //        throw new ArgumentNullException("vos");
        //    }
        //    this.vbase = vos;
        //}

        #endregion

        #region Derived

        public string Key => VobReference.Key;

        #region Path

        public virtual string Path => LionPath.GetTrimmedAbsolutePath(LionPath.Combine(parent == null ? "" : parent.Path, name));

        internal int Depth => LionPath.GetAbsolutePathDepth(Path);

        public IEnumerable<string> PathElements
        {
            get
            {
                if (Parent != null)
                {
                    foreach (var pathElement in Parent.PathElements)
                    {
                        yield return pathElement;
                    }
                }

                if (!String.IsNullOrEmpty(Name))
                {
                    yield return Name;
                }
                else
                {
                    if (Parent == null)
                    {
                        // yield nothing
                    }
                    else
                    {
                        yield return null; // Shouldn't happen.
                        //yield return String.Empty; // REVIEW - what to do here?
                    }
                }
            }
        }

        public IEnumerable<string> PathElementsReverse
        {
            get
            {
                yield return Name;

                if (Parent != null)
                {
                    foreach (var pathElement in Parent.PathElementsReverse)
                    {
                        yield return pathElement;
                    }
                }
            }
        }
        #endregion

        #region Reference

        public VobReference VobReference
        {
            get
            {
                if (vobReference == null)
                {
                    vobReference = new VobReference(Path);
                }
                return vobReference;
            }
            set
            {
                if (vobReference == value)
                {
                    return;
                }

                if (vobReference != default(IReference))
                {
                    throw new NotSupportedException("Reference can only be set once.  To relocate, use the Move() method.");
                }

                vobReference = value;
            }
        }
        private VobReference vobReference;


        public IVobReference Reference => VobReference;
        IReference IReferencable.Reference // TODO MEMORYOPTIMIZE: I think a base class has an IReference field
        {
            get => VobReference;
            //set
            //{
            //    if (value == null) { VobReference = null; return; }
            //    VobReference vr = value as VobReference;
            //    if (vr != null)
            //    {
            //        VobReference = vr;
            //        return;
            //    }
            //    else
            //    {
            //        //new VobReference(value); // FUTURE: Try converting
            //        throw new ArgumentException("Reference for a Vob must be VobReference");
            //    }
            //}
        }

        #endregion

        IRootVob IVob.Root => Root;
        public virtual RootVob Root
        {
            get
            {
                IVob vob;
                for (vob = this; vob.Parent != null; vob = vob.Parent) ;
                return vob as RootVob;
            }
        }

        #endregion

        public object FlexData { get; set; }

        #region MultiTyped

        // Non-inhereted extensions

        public IMultiTyped MultiTyped
        {
            get
            {
                if (multiTyped == null)
                {
                    multiTyped = new MultiType();
                }
                return multiTyped;
            }
        }
        protected MultiType multiTyped;

        #endregion

        #region VobNodes

        #region Vob Nodes: Own data

        IEnumerable<KeyValuePair<Type, IVobNode>> IVobInternals.VobNodesByType => vobNodesByType;
        protected ConcurrentDictionary<Type, IVobNode> vobNodesByType;

        VobNode<T> IVobInternals.TryAcquireOwnVobNode<T>()
           where T : class
        {
            var collection = vobNodesByType;
            if (collection == null) return null;
            if (collection.TryGetValue(typeof(T), out IVobNode v)) return (VobNode<T>)v;
            return null;
        }

        T DefaultFactory<T>(params object[] parameters)
        {
            var serviceProvider = this.GetService<IServiceProvider>();

            if (serviceProvider != null)
            {
                {
                    var factory = this.GetService<IInjectingFactory<T>>();
                    if (factory != null)
                    {
                        return factory.Create(serviceProvider, parameters);
                    }
                }
                {
                    var factory = this.GetService<IFactory<T>>();
                    if (factory != null)
                    {
                        return factory.Create(parameters);
                    }
                }
            }

            return (T)Activator.CreateInstance(typeof(T), parameters);
        }

        #region VobNode Value Factory

        TInterface DefaultVobNodeValueFactory<TInterface>(IVobNode vobNode)
        {
            {
                var factory = this.GetService<IFactory<TInterface>>();
                if (factory != null)
                {
                    return factory.Create();
                }
            }
            {
                var func = this.GetService<Func<Vob, TInterface>>();
                if (func != null)
                {
                    return func(this);
                }
            }
            {
                var func = this.GetService<Func<TInterface>>();
                if (func != null)
                {
                    return func();
                }
            }

            if (!typeof(TInterface).IsAbstract && !typeof(TInterface).IsInterface)
            {
                return ActivatorUtilities.CreateInstance<TInterface>(ServiceProviderForVobNode(vobNode));
            }

            throw new NotSupportedException($"No IFactory<TInterface> or Func<IVob, TInterface> or Func<TInterface> service registered and type is abstract or interface -- cannot create TInterface of type: {typeof(TInterface).Name}");
        }

        TInterface DefaultVobNodeValueFactory<TInterface, TImplementation>(IVobNode vobNode)
            where TImplementation : TInterface
            => ActivatorUtilities.CreateInstance<TImplementation>(ServiceProviderForVobNode(vobNode));

        IServiceProvider ServiceProviderForVobNode(IVobNode vobNode) => new ServiceProviderWrapper(this.GetServiceProvider(), factoryServices(vobNode));
        IDictionary<Type, object> factoryServices(IVobNode vobNode)
        {
            return new Dictionary<Type, object>
            {
                [typeof(Vob)] = vobNode.Vob,
                [typeof(IVobNode)] = vobNode
            };
        }

        #endregion

        VobNode<TInterface> IVobInternals.TryAddOwnVobNode<TInterface>(Func<IVobNode, TInterface> valueFactory)
        {
            if (vobNodesByType == null) vobNodesByType = new ConcurrentDictionary<Type, IVobNode>();
            var already = vobNodesByType.ContainsKey(typeof(TInterface));
            VobNode<TInterface> result = null;
            if (!already)
            {
                already = !vobNodesByType.TryAdd(typeof(TInterface),
                     result = (VobNode<TInterface>)Activator.CreateInstance(typeof(VobNode<>).MakeGenericType(typeof(TInterface)),
                    this, valueFactory ?? DefaultVobNodeValueFactory<TInterface>));
            }
            // if (already) throw new AlreadyException($"Already contains {typeof(TInterface).Name}"); // ENH: Create a separate AddOwnVobNode extension method to throw this
            return already ? null : result;
        }

        VobNode<TInterface> IVobInternals.AcquireOrAddOwnVobNode<TInterface>(Func<IVobNode, TInterface> valueFactory)
        {
            if (vobNodesByType == null) vobNodesByType = new ConcurrentDictionary<Type, IVobNode>();
            return (VobNode<TInterface>)vobNodesByType.GetOrAdd(typeof(TInterface),
                t => (IVobNode)Activator.CreateInstance(typeof(VobNode<>).MakeGenericType(t),
                this, valueFactory ?? DefaultVobNodeValueFactory<TInterface>));
        }

        VobNode<TInterface> IVobInternals.AcquireOrAddOwnVobNode<TInterface, TImplementation>(Func<IVobNode, TInterface> valueFactory)
        //where TInterface : class
        //where TImplementation : TInterface
        {
            if (vobNodesByType == null) vobNodesByType = new ConcurrentDictionary<Type, IVobNode>();
            return (VobNode<TInterface>)vobNodesByType.GetOrAdd(typeof(TInterface),
                t => (IVobNode)Activator.CreateInstance(typeof(VobNode<>).MakeGenericType(t),
                this, valueFactory ?? DefaultVobNodeValueFactory<TInterface, TImplementation>));
        }

        /// <summary>
        /// Get Value from own VobNode
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T AcquireOwn<T>() where T : class => Acquire<T>(maxDepth: 0).Take(1).FirstOrDefault();
        //{
        //    var node = ((IVobInternals)this).TryAcquireOwnVobNode<T>();
        //    if (node != null) return node.Value;
        //    return default;
        //}

        public IEnumerable<T> Acquire<T>(int minDepth = 0, int maxDepth = -1)
            where T : class
        {
            IVob vob = this;
            int depth = 0;
            for (int skip = minDepth; skip > 0 && vob != null; skip--)
            {
                vob = vob.Parent;
                depth++;
            }

            for (IVobNode<T> node = vob.TryGetOwnVobNode<T>(); node != null && (maxDepth < 0 || depth <= maxDepth); node = node.ParentVobNode)
            {
                if (node?.Value != null) yield return node.Value;

                if (node?.ParentVobNode != null)
                {
                    depth += node.Vob.Depth - node.ParentVobNode.Vob.Depth;
                }
            }
        }

        public IEnumerable<T> AcquireMany<T>() where T : class => Acquire<IEnumerable<T>>().SelectMany(e => e);

        #endregion

        #region Vob Nodes: Inheritance

        public IEnumerable<IVob> ParentEnumerable
        {
            get
            {
                yield return this;
                for (var vob = Parent; vob != null; vob = vob.Parent) yield return vob;
            }
        }

        VobNode<T> IVobInternals.TryAcquireNextVobNode<T>(int minDepth, int maxDepth)
            where T : class
        {
            var depth = 0;
            var vobEnumerable = ParentEnumerable;

            vobEnumerable.Skip(minDepth);
            depth += minDepth;

            foreach (var vob in vobEnumerable)
            {
                if (maxDepth >= 0 && depth > maxDepth) break;
                var vobNode = ((IVobInternals)vob).TryAcquireOwnVobNode<T>();
                if (vobNode != null) return vobNode;
            }

            //var vob = ParentEnumerable.ElementAt(skipOwn ? 1 : 0);
            //while (vob != null)
            //{
            //    var vobNode = ((IVobInternals)vob).TryGetOwnVobNode<T>();
            //    if (vobNode != null) return vobNode;
            //    vob = vob.Parent;
            //}
            return null;
        }

        ContextedVobNode<T> IVobInternals.TryGetNextContextedVobNode<T>(int minDepth) where T : class
            => new ContextedVobNode<T>(this, ((IVobInternals)this).TryAcquireNextVobNode<T>(minDepth));

        /// <param name="addAtRoot">True: if missing, will add at root. False: if missing, add at local Vob</param>
        /// <returns></returns>
        public VobNode<TInterface> AcquireOrAddNextVobNode<TInterface, TImplementation>(Func<IVobNode, TInterface> factory = null, bool addAtRoot = true)
            where TImplementation : TInterface
            where TInterface : class
        {
            IVob vob;
            for (vob = this; vob != null; vob = vob.Parent)
            {
                var vobNode = vob.TryGetOwnVobNode<TInterface>();
                if (vobNode != null) return vobNode;
            }
            return (addAtRoot ? vob : this).GetOrAddOwnVobNode<TInterface, TImplementation>(factory);
        }

        public T AcquireNext<T>(int minDepth = 0, int maxDepth = -1)
            where T : class
            => this.TryGetNextVobNode<T>(minDepth: minDepth, maxDepth: maxDepth)?.Value ?? default;

#if TODO // If needed
        public int NextVobNodeRelativeDepth
        {
            get
            {
                int i = 0;
                var vob = this;
                while (vob != null)
                {
                    if (vob.VobNode != null) return i;
                    vob = vob.Parent;
                    i--;
                }
                throw new VosException("Missing NextVobNode"); // Should not happen - RootVob should always have a VobNode.
            }
        }
#endif

        #endregion

        #region Vob Nodes: Descendants

#if TODO // Maybe
        public IEnumerable<VobNode> ChildVobNodes
        {
            get
            {
                foreach (var child in Children.Select(kvp => kvp.Value))
                {
                    if (child.VobNode != null) yield return child.VobNode;

                    foreach (var childVobNode in child.ChildVobNodes)
                    {
                        yield return childVobNode;
                    }
                }
            }
        }
#endif

        #endregion

        #endregion

        #region Referencing

        #region GetReference<T>

        // TODO: Move to extension method?
        // FUTURE MEMOPTIMIZE - consider caching/reusing these references to save memory?
        /// <summary>
        /// Get typed reference
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IVobReference<T> GetReference<T>() => new VobReference<T>(Path) { Persister = string.IsNullOrEmpty(Root.RootName) ? null : Root.RootName };
        // OPTIMIZE: new VosRelativeReference(this)
        // TODO: Use VobReference<T>?  Providers need to be wired up to DI

        #endregion

        #endregion

        #region Misc

        public override bool Equals(object obj)
        {
            var other = obj as Vob;
            if (other == null)
            {
                return false;
            }
#if DEBUG
            if (Path == other.Path && this != other) { l.TraceWarn("Two Vobs with same path but !="); }
            if (Path == other.Path && !object.ReferenceEquals(this, other)) { l.TraceWarn("Two Vobs with same path but !ReferenceEquals"); }
#endif
            return Path == other.Path;
        }

        public override int GetHashCode() => Path.GetHashCode();

        public override string ToString() => Reference?.ToString();

        private static readonly ILogger l = Log.Get();

        #endregion

    }

}