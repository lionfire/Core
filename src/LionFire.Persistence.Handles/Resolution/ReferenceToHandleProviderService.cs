using LionFire.Dependencies;
using LionFire.Referencing;
using Microsoft.Extensions.Options;
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
        public static IReferenceToHandleService Current => DependencyLocator.Get<IReferenceToHandleService>();

        //IOptionsFactory<NamedHandleProviderConfig> optionsFactory;
        IServiceProvider ServiceProvider { get; }
        public ReferenceToHandleService(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        //public ReferenceToHandleService(IOptionsFactory<NamedHandleProviderConfig> optionsFactory)
        //{
        //    this.optionsFactory = optionsFactory;
        //}
#warning TODO: If this method is okay, do the same generic parameter for the others
        public IReadHandleProvider<TReference> GetReadHandleProvider<TReference>(TReference input) //, IServiceProvider serviceProvider = null)
            where TReference : IReference
        {
            // REVIEW - is this IReference check needed? What is performance?
            //if (typeof(TReference) == typeof(IReference))
            //{
            //// Question: Handle named providers here, or let each provider type do it?
            //return DependencyLocator.Get<IReadHandleProvider>(typeof(IReadHandleProvider<>).MakeGenericType(input.GetType()));
            //}
            //else
            {
                return DependencyLocator.Get<IReadHandleProvider<TReference>>(typeof(IReadHandleProvider<TReference>), ServiceProvider);

            }
        }
        public IReadHandleProvider GetReadHandleProvider(IReference input)
        {
            // Question: Handle named providers here, or let each provider type do it?
            return DependencyLocator.Get<IReadHandleProvider>(typeof(IReadHandleProvider<>).MakeGenericType(input.GetType()));
        }

        public IReadWriteHandleProvider GetReadWriteHandleProvider(IReference input)
            => DependencyLocator.Get<IReadWriteHandleProvider>(typeof(IReadWriteHandleProvider<>).MakeGenericType(input.GetType()));

        //if (handleProviders.TryGetValue(input.GetType(), out IReadWriteHandleProvider result))
        //{
        //    return result;
        //}


        //public IReadHandleProvider GetReadHandleProvider<TReference>(TReference input)
        //    where TReference: IReference
        //{
        //    DependencyLocator.Get<IReadHandleProvider<TReference>>(typeof(IReadHandleProvider<TReference>));
        //}

        public IWriteHandleProvider GetWriteHandleProvider(IReference input)
                    => DependencyLocator.Get<IWriteHandleProvider>(typeof(IWriteHandleProvider<>).MakeGenericType(input.GetType()));

        public ICollectionHandleProvider GetCollectionHandleProvider(IReference input)
            => DependencyLocator.Get<ICollectionHandleProvider>(typeof(ICollectionHandleProvider<>).MakeGenericType(input.GetType()));

        public IReadHandleCreator GetReadHandleCreator(IReference input) => throw new NotImplementedException();
        public IReadHandleCreator<TReference> GetReadHandleCreator<TReference>(TReference input) where TReference : IReference => throw new NotImplementedException();
        public IReadWriteHandleProvider<TReference> GetReadWriteHandleProvider<TReference>(TReference input) where TReference : IReference => throw new NotImplementedException();
        public IReadWriteHandleCreator GetReadWriteHandleCreator(IReference input) => throw new NotImplementedException();
        public IReadWriteHandleCreator<TReference> GetReadWriteHandleCreator<TReference>(TReference input) where TReference : IReference => throw new NotImplementedException();
        public IWriteHandleProvider<TReference> GetWriteHandleProvider<TReference>(TReference input) where TReference : IReference => throw new NotImplementedException();
        public IWriteHandleCreator GetWriteHandleCreator(IReference input) => throw new NotImplementedException();
        public IWriteHandleCreator<TReference> GetWriteHandleCreator<TReference>(TReference input) where TReference : IReference => throw new NotImplementedException();

        /// ///////////////////


        //ConcurrentDictionary<Type,  List<IReadWriteHandleProvider>> handleProviders = new ConcurrentDictionary<Type, List<IReadWriteHandleProvider>>();
        //ConcurrentDictionary<Type,  List<IReadHandleProvider>> readHandleProviders = new ConcurrentDictionary<Type, List<IReadHandleProvider>>();

        //ConcurrentDictionary<Type, SortedList<double, IReadWriteHandleProvider>> handleProvidersRanked = new ConcurrentDictionary<Type, SortedList<double, IReadWriteHandleProvider>>();
        //ConcurrentDictionary<Type, SortedList<double, IReadHandleProvider>> readHandleProvidersRanked = new ConcurrentDictionary<Type, SortedList<double, IReadHandleProvider>>();

        //public void Reset()
        //{
        //    handleProviders.Clear();
        //    readHandleProviders.Clear();
        //}

        //public IReferenceToHandleProviderProvider HPP(IReference input)
        //{
        //    DependencyLocator.Get<><>()
        //}


        //public IReadHandleProvider GetReadHandleProvider(IReference input) => throw new System.NotImplementedException();

        //public IEnumerable<IReadWriteHandleProvider> GetHandleProviders(IReference input)=>handleProviders
        //public IEnumerable<IReadHandleProvider> GetReadHandleProviders(IReference input);

    }


}
