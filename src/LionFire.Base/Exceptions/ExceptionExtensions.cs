using LionFire.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LionFire.ExtensionMethods
{
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Return null if it cannot be unwrapped.
        /// 
        /// Default unwraps:
        ///  - TargetInvocationException.InnerException
        ///  - AggregateException: InnerException or InnerExceptions
        ///  - IPotentiallyTemporaryError: InnerException
        ///  
        /// Other ideas:
        ///  - Exception.InnerException - if you know your outer exceptions can always be discarded
        /// </summary>
        public static List<Func<Exception, Exception>> Unwrappers = new List<Func<Exception, Exception>>
        {
            e => (e as TargetInvocationException)?.InnerException,
            e => (e as AggregateException)?.InnerExceptions?.SingleOrDefault(),
            e => (e as AggregateException)?.InnerException,
            e => e is IPotentiallyTemporaryError ? e.InnerException : null,
        };

        /// <summary>
        /// Unwrap the exception using strategies in Unwrappers.
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static Exception Unwrap(this Exception ex)
        {
            var unwrapped = ex;
            int lastSuccess = Unwrappers.Count;
            for (int i = 0; true; i++)
            {
                if (lastSuccess == i) break;
                if (i >= Unwrappers.Count) { i = 0; }

                var newUnwrapped = Unwrappers[i](unwrapped);

                if (newUnwrapped == null) continue;
                unwrapped = newUnwrapped;
                lastSuccess = i;
            }
            return unwrapped;
        }
    }
}
