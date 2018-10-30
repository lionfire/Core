using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Referencing
{
    public interface IHandleProvider
    {
        /// <summary>
        /// Informational
        /// </summary>
        IEnumerable<Type> HandleTypes { get; }

        // REVIEW - is handleObject really helpful here?
        H<T> GetHandle<T>(IReference reference, T handleObject = default(T));
        R<T> GetReadHandle<T>(IReference reference);
    }

    public static class IHandleProviderExtensions
    {
        public static H GetHandle(this IHandleProvider handleProvider, IReference reference, object handleObject = null) => (H)handleProvider.GetHandle<object>(reference, handleObject);
        public static R GetReadHandle(this IHandleProvider handleProvider, IReference reference) => (R)handleProvider.GetReadHandle<object>(reference);
    }

}
