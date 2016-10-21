using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LionFire.Dependencies;

namespace LionFire.Dependencies
{
    public interface IHasDependencyProperties
    {
        IEnumerable<PropertyInfo> DependencyProperties { get; }
    }

    public static class DependencyResolutionExtensions
    {
        #region Properties

        public static IEnumerable<PropertyInfo> GetDependencyProperties(this object obj)
        {
            var hdp = obj as IHasDependencyProperties;
            if (hdp != null) return hdp.DependencyProperties;
            return obj.GetDependencyAttributeProperties();
        }

        public static IEnumerable<PropertyInfo> GetDependencyAttributeProperties(this object obj)
        {
            return obj.GetType().GetProperties().Where(_ => _.GetCustomAttribute(typeof(DependencyAttribute)) != null);
        }

        #endregion

        #region Methods

        public static void ValidateDependencies(this object obj)
        {
            // REVIEW - consider putting this on IHasDependencies and moving this namespace to LionFire
            UnsatisfiedDependencies unresolvedDependencies;
            if (!obj.AreDependenciesResolved(out unresolvedDependencies)) throw new HasUnresolvedDependenciesException(obj, unresolvedDependencies);
        }

        public static bool AreDependenciesResolved(this object obj, out UnsatisfiedDependencies unresolvedDependencies)
        {
            return _ResolveDependencies(obj, out unresolvedDependencies, null, false);
            //    UnsatisfiedDependencies results = new UnsatisfiedDependencies();
            //    foreach (var mi in obj.GetType().GetProperties().Where(_ => _.GetCustomAttribute(typeof(DependencyAttribute)) != null))
            //    {
            //        if (mi.GetValue(obj) == null)
            //        {
            //            results.Add(new UnsatisfiedDependency(mi));
            //        }
            //    }
            //    unresolvedDependencies = results.Count > 0 ? results : UnsatisfiedDependencies.Resolved;
            //    return results.Count == 0;
        }
        public static bool TryResolveDependencies(this object obj, out UnsatisfiedDependencies unresolvedDependencies, IServiceProvider serviceProvider = null)
        {
            if (serviceProvider == null)
            {
                serviceProvider = (obj as IRequiresServices)?.ServiceProvider;
            }
            return _ResolveDependencies(obj, out unresolvedDependencies, serviceProvider, true);
        }
        private static bool _ResolveDependencies(this object obj, out UnsatisfiedDependencies unresolvedDependencies, IServiceProvider serviceProvider, bool resolve)
        {
            //if (object.ReferenceEquals(UnsatisfiedDependencies.Resolved, unresolvedDependencies)) return true;

            //if (serviceProvider == null)
            //{
            //    return AreDependenciesResolved(obj, out unresolvedDependencies);
            //}

            UnsatisfiedDependencies results = new UnsatisfiedDependencies();
            foreach (var mi in obj.GetDependencyProperties())
            {
                if (mi.GetValue(obj) == null)
                {
                    if (resolve && serviceProvider != null)
                    {
                        var resolvedObject = mi.PropertyType == typeof(IServiceProvider) ? serviceProvider : serviceProvider.GetService(mi.PropertyType);
                        if (resolvedObject != null)
                        {
                            mi.SetValue(obj, resolvedObject);
                            continue;
                        }
                    }

                    results.Add(new UnsatisfiedDependency(mi));
                }
            }
            unresolvedDependencies = results.Count > 0 ? results : UnsatisfiedDependencies.Resolved;
            return results.Count == 0;
        }

        #endregion

        #region Resolve Set

        public static Task<Dictionary<object, UnsatisfiedDependencies>> TryResolveSet(this IEnumerable<object> objects, IServiceProvider serviceProvider = null)
        {
            int lastCount = int.MaxValue;
            var remaining = new List<object>(objects);
            do
            {
                Dictionary<object, UnsatisfiedDependencies> dict = new Dictionary<object, UnsatisfiedDependencies>();

                foreach (var obj in remaining.ToArray())
                {
                    UnsatisfiedDependencies ud; // = (obj as IHasDependencies)?.UnsatisfiedDependencies;
                    if (!obj.TryResolveDependencies(out ud, serviceProvider))
                    {
                        dict.Add(obj, ud);
                    }
                    else
                    {
                        remaining.Remove(obj);
                    }
                }

                var count = dict.Count;
                if (count == lastCount)
                {
                    return Task.FromResult(dict); // Fail to make progress.  Quit.
                }
                lastCount = count;
            } while (remaining.Count > 0);
            return Task.FromResult<Dictionary<object, UnsatisfiedDependencies>>(null);
        }

        #endregion

    }
}
