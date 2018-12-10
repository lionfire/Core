using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Referencing
{
    public interface IReadHandleProvider
    {
        RH<T> GetReadHandle<T>(IReference reference);
    }
    public interface IHandleProvider : IReadHandleProvider
    {
        H<T> GetHandle<T>(IReference reference/*, T handleObject = default(T)*/);
    }

    public static class IHandleProviderExtensions
    {
        // Do these casts work, or
        //H GetHandle(IReference reference, T handleObject = default(T)); // Needed? and R version?
        public static H GetHandle(this IHandleProvider handleProvider, IReference reference/*, object handleObject = null*/) => (H)handleProvider.GetHandle<object>(reference/*, handleObject*/);
        public static RH GetReadHandle(this IHandleProvider handleProvider, IReference reference) => (RH)handleProvider.GetReadHandle<object>(reference);
    }

}
