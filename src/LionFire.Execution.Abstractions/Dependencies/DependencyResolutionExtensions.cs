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

        public static void ResolveDependencies(this object obj)
        {
            UnsatisfiedDependencies unresolvedDependencies;
            if (!obj.TryResolveDependencies(out unresolvedDependencies))
            {
                throw new HasUnresolvedDependenciesException();
            }
        }

        #endregion

        #region Resolve Set

        public static Task<Dictionary<object, UnsatisfiedDependencies>> TryResolveSet(this IEnumerable<object> objects)
        {
            int lastCount = int.MaxValue;
            var remaining = new List<object>(objects);
            do
            {
                Dictionary<object, UnsatisfiedDependencies> dict = new Dictionary<object, UnsatisfiedDependencies>();

                foreach (var obj in remaining)
                {
                    UnsatisfiedDependencies ud; // = (obj as IHasDependencies)?.UnsatisfiedDependencies;
                    if (!obj.TryResolveDependencies(out ud))
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
            } while (true);
        }

        #endregion

    }
}
