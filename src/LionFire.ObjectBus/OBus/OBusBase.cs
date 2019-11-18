using System;
using System.Collections.Generic;
using System.Linq;
using LionFire.Applications.Hosting;
using LionFire.Dependencies;
using LionFire.Referencing;
using LionFire.Persistence.Handles;
using Microsoft.Extensions.DependencyInjection;
using LionFire.Persistence;
using LionFire.ObjectBus.Handles;
using System.Reflection;
using LionFire.ObjectBus.Resolution;

namespace LionFire.ObjectBus
{

    /// <summary>
    /// Convenience base class
    /// </summary>
    /// <typeparam name="TConcrete"></typeparam>
    /// <typeparam name="TOBase"></typeparam>
    /// <typeparam name="TReference"></typeparam>
    public abstract class OBusBase<TConcrete, TOBase, TReference> : OBusBase<TConcrete>, IReadWriteHandleProvider<TReference>, IReadHandleProvider<TReference>
        where TConcrete : OBusBase<TConcrete, TOBase, TReference>, IOBus
        where TOBase : IOBase
        where TReference : IReference
    {
        #region Static

        static ConstructorInfo ReferenceConstructor;
        private static readonly IEnumerable<string> uriSchemes;
        //private static readonly IEnumerable<string> defaultUriScheme;

        static OBusBase()
        {
            try
            {
                ReferenceConstructor = typeof(TReference).GetConstructor(new Type[] { typeof(string) });
            }
            catch { }
            
            uriSchemes = ((IEnumerable<string>)typeof(TReference).GetField("UriSchemes", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(null)) ?? Enumerable.Empty<string>();
            //defaultUriScheme = uriSchemes?.FirstOrDefault();
        }

        #endregion

        #region Construction

        public OBusBase()
        {
            bool ctorPredicate(ConstructorInfo m)
            {
                var p = m.GetParameters();
                if (p.Length != 3) return false;
                if (p[0].ParameterType != typeof(IReference)) return false;
                if (p[1].ParameterType != typeof(IOBase)) return false;
                if (!p[2].ParameterType.IsConstructedGenericType) return false;
                if (p[2].ParameterType.GenericTypeArguments[0].GenericParameterPosition != 0) return false;
                return true;
            }

            foreach (var type in HandleTypes)
            {
                var ctor = type.GetConstructors().Where(ctorPredicate).FirstOrDefault();
                if (ctor != null)
                {
                    handleCtor = ctor;
                    break;
                }
            }
            foreach (var type in ReadHandleTypes)
            {
                var ctor = type.GetConstructors().Where(ctorPredicate).FirstOrDefault();
                if (ctor != null)
                {
                    readHandleCtor = ctor;
                    break;
                }
            }
        }

        #endregion

        #region Cached Reflection

        ConstructorInfo handleCtor;
        ConstructorInfo readHandleCtor;

        #endregion

        public override IEnumerable<string> UriSchemes => uriSchemes;

        public override IEnumerable<Type> ReferenceTypes
        {
            get
            {
                yield return typeof(TReference);
            }
        }

        public override IReference TryGetReference(string uri) => (IReference)ReferenceConstructor.Invoke(new object[] { ReferenceUriParsing.PathOnlyFromUri(uri, uriSchemes) });
        
        public IReadWriteHandleBase<T> GetReadWriteHandle<T>(TReference reference, T handleObject = default) => (IReadWriteHandleBase<T>)handleCtor.Invoke(new object[] { reference, TryGetOBase(reference), handleObject });
        //new OBaseHandle<T>(reference, DefaultOBase, handleObject);
        public IReadHandleBase<T> GetReadHandle<T>(TReference reference, T handleObject = default) => new OBaseReadHandle<T>(reference, DefaultOBase, handleObject);

    }

    /// <summary>
    /// Standard base class
    /// </summary>
    /// <typeparam name="TConcrete"></typeparam>
    public abstract class OBusBase<TConcrete> : IOBus
    where TConcrete : IOBus
    {

        public virtual IOBase SingleOBase => null;
        public virtual IOBase DefaultOBase => SingleOBase;

        //    public virtual T InstantiateObject<T>(Func<T> createDefault = null)
        //    {
        //        T result;

        //        if (createDefault != null)
        //        {
        //            result = createDefault();
        //        }
        //        else
        //        {
        //            result = (T)Activator.CreateInstance(typeof(T));
        //        }
        //        return result;
        //    }

        public virtual IEnumerable<Type> HandleTypes
        {
            get
            {
                yield return typeof(OBaseHandle<>);
            }
        }

        public virtual IEnumerable<Type> ReadHandleTypes
        {
            get
            {
                yield return typeof(OBaseReadHandle<>);
            }
        }

        #region IReferenceProvider

        public abstract IEnumerable<Type> ReferenceTypes { get; }

        public abstract IEnumerable<string> UriSchemes { get; }

        public abstract IReference TryGetReference(string uri);
        
        #endregion

        public abstract IOBase TryGetOBase(IReference reference);

        //public virtual bool IsValid(IReference reference) => reference != null && UriSchemes.Contains(reference.Scheme); // Not sure on usefulness of this
        //bool ICompatibleWithSome<string>.IsCompatibleWith(string stringUri) => stringUri != null && UriSchemes.Where(scheme => stringUri.StartsWith(scheme)).Any();

        /// <summary>
        /// Register as the singleton for all IReadWriteHandleProvider&lt;&gt;'s that this OBus implements
        /// </summary>
        /// <param name="sc"></param>
        /// <returns></returns>
        public IServiceCollection AddServices(IServiceCollection sc)
        {

            foreach (var type in ReferenceTypes)
            {
                {
                    var hpType = typeof(IReadWriteHandleProvider<>).MakeGenericType(type);
                    if (hpType.IsAssignableFrom(this.GetType()))
                    {
                        sc.AddSingleton(hpType, this);
                    }
                }
                {
                    var rhpType = typeof(IReadHandleProvider<>).MakeGenericType(type);
                    if (rhpType.IsAssignableFrom(this.GetType()))
                    {
                        sc.AddSingleton(rhpType, this);
                    }
                }
            }
            sc.TryAddEnumerableSingleton<IOBus, TConcrete>();
            sc.TryAddEnumerableSingleton<IReferenceProvider, TConcrete>();
            sc.TryAddEnumerableSingleton<IReadHandleProvider, TConcrete>();
            sc.TryAddEnumerableSingleton<IReadWriteHandleProvider, TConcrete>();
            return sc;
        }

        public virtual bool IsCompatibleWith(IReference obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (ReferenceTypes.Contains(obj.GetType()))
            {
                return true;
            }

            // TODO: If unbound reference type, resolve it here.

            return false;
        }

        public virtual IReadWriteHandleBase<T> GetReadWriteHandle<T>(IReference reference, T handleObject = default)
        {
            // TODO: If handle reuse is on, try to find existing handle.
            //var h = new OBusHandle<T>(reference, handleObject);

            var obase = TryGetOBase(reference) ?? throw new ObjectBusException("Couldn't resolve OBase for specified reference");
            var h = new OBaseHandle<T>(reference, obase, handleObject);

            //if (obase != null)
            //{
            //    h.OBase = obase;
            //}
            //else
            //{
            //    h.OBus = this;
            //}

            return h;
        }

        public virtual IReadHandleBase<T> GetReadHandle<T>(IReference reference, T handleObject = default)
        {
            // TODO: If handle reuse is on, try to find existing handle.

            // TODO: create read-only handle
            //var h = new OBusHandle<T>(reference);
            var obase = TryGetOBase(reference) ?? throw new ObjectBusException("Couldn't resolve OBase for specified reference");
            var h = new OBaseReadHandle<T>(reference, obase, handleObject);

            return h;
        }

        public virtual HC<T> GetCollectionHandle<T>(IReference reference)
        {
            throw new NotImplementedException();
            //var oboc = new OBoc<T, OBaseCollectionEntry>();
        }
        public virtual RC<T> GetReadCollectionHandle<T>(IReference reference)
        {
            throw new NotImplementedException();
        }
    }
    
}
