using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Referencing.Handles
{
    public interface IHandleProvider
    {
        H<T> GetHandle<T>(IReference reference/*, T handleObject = default(T)*/);
    }

    //[AutoRegister]
    public interface IHandleProvider<TReference>
        where TReference : IReference
    {
        H<T> GetHandle<T>(TReference reference);
    }

    
    //public static class IHandleProviderExtensions
    //{
    //    // Do these casts work, or
    //    //H GetHandle(IReference reference, T handleObject = default(T)); // Needed? and R version?
    //    public static H GetHandle(this IHandleProvider handleProvider, IReference reference/*, object handleObject = null*/) => (H)handleProvider.GetHandle<object>(reference/*, handleObject*/);
    //    public static RH GetReadHandle(this IHandleProvider handleProvider, IReference reference) => (RH)handleProvider.GetReadHandle<object>(reference);
    //}

}
