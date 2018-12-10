using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Referencing
{    
    public interface ICollectionHandleProvider : IReadCollectionHandleProvider
    {
        C<T> GetCollectionHandle<T>(IReference reference);
    }

    public static class ICollectionHandleProviderExtensions
    {
        // Do these casts work, or
        //H GetCollectionHandle(IReference reference, T handleObject = default(T)); // Needed? and R version?
        public static C GetCollectionHandle(this ICollectionHandleProvider handleProvider, IReference reference) => (C)handleProvider.GetCollectionHandle<object>(reference);
        
    }
}
