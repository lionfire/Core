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
using LionFire.Extensions.ObjectBus;
using LionFire.Instantiating;
using LionFire.MultiTyping;
using LionFire.ObjectBus;
using LionFire.Ontology;
using LionFire.Referencing;
using LionFire.Structures;
using LionFire.Types;
using LionFire.Vos;
using Microsoft.Extensions.Logging;
using static LionFire.Vos.Mount;

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

    //public class PathTree<TValue>
    //    where TValue : class
    //{
    //    public TValue this[string subpath] => this[subpath];

    //    public TValue GetChild(string subpath) => GetChild(subpath.ToPathArray(), 0);

    //    public TValue QueryChild(string reference) => QueryChild(subpath.ToPathArray(), 0);

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
    public partial class Vob :
        //CachingHandleBase<Vob, Vob>,
        //CachingChildren<Vob>,
        //IHasHandle,
        //H, // TODO - make this a smarter handle.  The object might be a dynamically created MultiType for complex scenarios
        IReferencable,
#if AOT
		IParented
#else
        IParented<Vob>
#endif
        , IVob
    {

        #region Ontology

        #region Vos

        public VBase Vos => vos;
        private readonly VBase vos;

        #endregion

        #region IParented

#if AOT
        object IParented.Parent { get { return this.Parent; } set { throw new NotSupportedException(); } }
#endif

        #region Parent

        public Vob Parent
        {
            get => parent;
            set => throw new NotSupportedException();
        }
        private readonly Vob parent;

        #endregion

        #endregion

        public string Name => name;
        private readonly string name;

        public string Path => path;
        private readonly string path;

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
        
        #region Construction

        public Vob(VBase vos, Vob parent, string name)
        {
            if (vos == null)
            {
                throw new ArgumentNullException("vos");
            }

            if (GetType() == typeof(RootVob))
            {
                if (!String.IsNullOrEmpty(name))
                {
                    throw new ArgumentException("name must be null or empty for root");
                }
            }
            else
            {
                if (parent == null)
                {
                    throw new ArgumentNullException("parent must be set for all non-RootVob Vobs.");
                }

                if (name == null)
                {
                    throw new ArgumentNullException("name must not be null for non-root");
                }
            }

            //if (string.IsNullOrWhiteSpace(name) && this.GetType() != typeof(RootVob)) throw new ArgumentNullException("Name must be set for all non-RootVob Vobs.");

            this.vos = vos;
            this.parent = parent;
            this.name = name;

            path = LionPath.CleanAbsolutePathEnds(LionPath.Combine((parent == null ? "" : parent.Path), name));
            VobDepth = LionPath.GetAbsolutePathDepth(path);

            InitializeEffectiveMounts();
            //MountStateVersion = -1;
        }

#endregion

        #region Referencing

        #region GetReference<T>

        // FUTURE MEMOPTIMIZE - consider caching/reusing these references to save memory?
        public VosReference GetReference<T>() => new VosReference(Path) { Type = typeof(T) };

        #endregion

        #endregion

        #region Handles

        #region Get Handle

        public VobReadHandle<T> GetReadHandle<T>() => (VobReadHandle<T>)readHandles.GetOrAdd(typeof(T), t => CreateReadHandle(t));

        
        /// <seealso cref="CreateHandle(Type)"/>
        public VobHandle<T> GetHandle<T>() => (VobHandle<T>)handles.GetOrAdd(typeof(T), t => CreateHandle(t));
        public IVobHandle GetHandle(Type type) => (IVobHandle)handles.GetOrAdd(type, t => CreateHandle(t));

        public IVobHandle CreateHandle(Type type)
        {
            Type vhType = typeof(VobHandle<>).MakeGenericType(type);
            return (IVobHandle)Activator.CreateInstance(vhType, this);
        }
        internal IVobReadHandle CreateReadHandle(Type type)
        {
            Type vhType = typeof(VobReadHandle<>).MakeGenericType(type);
            return (IVobReadHandle)Activator.CreateInstance(vhType, this);
        }
        
        private ConcurrentDictionary<Type, object> handles = new ConcurrentDictionary<Type, object>();
        private readonly ConcurrentDictionary<Type, object> readHandles = new ConcurrentDictionary<Type, object>();

        #endregion

        #endregion
        
        #region IReferencable

        #region Reference

        public string Key => VosReference.Key;

        public VosReference VosReference
        {
            get
            {
                if (vosReference == null)
                {
                    vosReference = new VosReference(path);
                }
                return vosReference;
            }
            set
            {
                if (vosReference == value)
                {
                    return;
                }

                if (vosReference != default(IReference))
                {
                    throw new NotSupportedException("Reference can only be set once.  To relocate, use the Move() method.");
                }

                vosReference = value;
            }
        }
        private VosReference vosReference;

        public IReference Reference // TODO MEMORYOPTIMIZE: I think a base class has an IReference field
        {
            get => VosReference;
            set
            {
                if (value == null) { VosReference = null; return; }
                VosReference vr = value as VosReference;
                if (vr != null)
                {
                    VosReference = vr; return;
                }
                else
                {
                    //new VosReference(value); // FUTURE: Try converting
                    throw new ArgumentException("Reference for a Vob must be VosReference");
                }
            }
        }

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

        public override string ToString() => Path;

        private static ILogger l = Log.Get();
        //private static ILogger lSave = Log.Get("LionFire.Vos.Vob.Save");
        //private static ILogger lLoad = Log.Get("LionFire.Vos.Vob.Load");

        #endregion

    }
}