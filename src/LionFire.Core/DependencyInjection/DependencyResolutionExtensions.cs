using LionFire.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LionFire.DependencyInjection
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

        public static void ResolveDependencies(this object obj, IServiceProvider serviceProvider = null)
        {
            ValidationContext vc = null;

            obj.TryResolveDependencies(() => RefParameterHelpers.GetOrCreate<ValidationContext>(ref vc), serviceProvider);
            vc.EnsureValid();
        }

        public static bool TryResolveDependencies(this object obj, out UnsatisfiedDependencies unresolvedDependencies, IServiceProvider serviceProvider = null)
        {
            if (serviceProvider == null)
            {
                serviceProvider = (obj as IRequiresServices)?.ServiceProvider;
            }
            return _ResolveDependencies(obj, out unresolvedDependencies, serviceProvider, true);
        }
        public static ValidationContext TryResolveDependencies(this object obj, Func<ValidationContext> validationContext, IServiceProvider serviceProvider = null)
        {
            if (serviceProvider == null)
            {
                serviceProvider = (obj as IRequiresServices)?.ServiceProvider;
            }
            if (serviceProvider == null)
            {
                serviceProvider = InjectionContext.Current;
            }

            ValidationContext vc = null;
            _ResolveDependencies(obj, out UnsatisfiedDependencies unresolvedDependencies, serviceProvider, true);

            if (unresolvedDependencies != null && unresolvedDependencies.Count > 0)
            {
                foreach (var ud in unresolvedDependencies)
                {
                    if (vc == null) vc = validationContext();
                    vc.AddMissingDependencyIssue(ud.Description);
                }
            }
            return vc;
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

        public static async Task ResolveDependencies(this IEnumerable<object> objects, IServiceProvider serviceProvider)
        {
            var unresolvedDependencies = await objects.TryResolveDependencies(serviceProvider).ConfigureAwait(false);
            if (unresolvedDependencies != null && unresolvedDependencies.Count > 0)
            {
                throw new HasUnresolvedDependenciesException(unresolvedDependencies);
            }
        }

        #region Resolve Set

        public static Task<Dictionary<object, UnsatisfiedDependencies>> TryResolveDependencies(this IEnumerable<object> objects, IServiceProvider serviceProvider = null)
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
namespace LionFire
{
    public static class RefParameterHelpers
    {
        public static T GetOrCreate<T>(ref T obj) where T : class, new()
        {
            if (obj == null) obj = new T();
            return obj;
        }
    }
}