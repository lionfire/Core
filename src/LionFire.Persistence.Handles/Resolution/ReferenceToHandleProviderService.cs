using LionFire.DependencyInjection;
using LionFire.Referencing;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace LionFire.Persistence.Handles
{
    /// <summary>
    /// 
    /// </summary>
    /// <design>
    /// Thoughts:
    ///  - should be fast
    ///  - having multiple resolvers per IReference type and/or URI Scheme can provide flexibility:
    ///    - overloading URI Schemes with multiple implementations
    ///    - caching layer -- or should that be done at the OBase configuration level?
    ///  - On the flip side, I may always want one IReference to predictably resolve to one handle.
    ///  - CreateHandle (new) vs ToHandle (probably/possibly reused)
    /// 
    /// 
    /// </design>
    public class ReferenceToHandleService : IReferenceToHandleService
    {
        public static IReferenceToHandleService Instance 
            => DependencyContext.Current.GetServiceOrSingleton<IReferenceToHandleService, ReferenceToHandleService>();


        public IHandleProvider GetHandleProvider(IReference input) 
            => (IHandleProvider)DependencyContext.Current.GetServiceOrSingleton(typeof(IHandleProvider<>).MakeGenericType(input.GetType()));
        
        //if (handleProviders.TryGetValue(input.GetType(), out IHandleProvider result))
        //{
        //    return result;
        //}

        public IReadHandleProvider GetReadHandleProvider(IReference input)
            => (IReadHandleProvider)DependencyContext.Current.GetServiceOrSingleton(typeof(IReadHandleProvider<>).MakeGenericType(input.GetType()));

        public ICollectionHandleProvider GetCollectionHandleProvider(IReference input)
            => (ICollectionHandleProvider)DependencyContext.Current.GetServiceOrSingleton(typeof(ICollectionHandleProvider<>).MakeGenericType(input.GetType()));

        /// ///////////////////


        //ConcurrentDictionary<Type,  List<IHandleProvider>> handleProviders = new ConcurrentDictionary<Type, List<IHandleProvider>>();
        //ConcurrentDictionary<Type,  List<IReadHandleProvider>> readHandleProviders = new ConcurrentDictionary<Type, List<IReadHandleProvider>>();

        //ConcurrentDictionary<Type, SortedList<double, IHandleProvider>> handleProvidersRanked = new ConcurrentDictionary<Type, SortedList<double, IHandleProvider>>();
        //ConcurrentDictionary<Type, SortedList<double, IReadHandleProvider>> readHandleProvidersRanked = new ConcurrentDictionary<Type, SortedList<double, IReadHandleProvider>>();

        //public void Reset()
        //{
        //    handleProviders.Clear();
        //    readHandleProviders.Clear();
        //}

        //public IReferenceToHandleProviderProvider HPP(IReference input)
        //{
        //    DependencyContext.Current.GetServiceOrSingleton<>()
        //}


        //public IReadHandleProvider GetReadHandleProvider(IReference input) => throw new System.NotImplementedException();

        //public IEnumerable<IHandleProvider> GetHandleProviders(IReference input)=>handleProviders
        //public IEnumerable<IReadHandleProvider> GetReadHandleProviders(IReference input);

    }

    
}
