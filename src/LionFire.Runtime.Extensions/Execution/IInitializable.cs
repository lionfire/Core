using LionFire.DependencyInjection;
using LionFire.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Execution
{
    
    public interface IInitializable2
    {
        Task<ValidationContext> Initialize();
    }

    public interface IInitializable
    {
        /// <summary>
        /// Attempt to initialize, returning true on success, false if initialization can be attempted again.
        /// </summary>
        /// <returns>True if successful, false if not, such as the case when dependencies are not available yet.  See IHasDependencies to indicate missing dependencies.</returns>
        Task<bool> Initialize();
    }

    public static class IInitializableExtensions
    {
        public static async Task InitializeAll(this IEnumerable<IInitializable> initializables)
        {
            await initializables.RepeatAllUntilTrue(obj => ((IInitializable)obj).Initialize);

            #region Scraps

            //        var msg = $"No progress made on initializing {componentsRequiringInit} remaining components: " + stillNeedsInitialization.Select(c => c.ToString()).Aggregate((x, y) => x + ", " + y);

            //if (uds.Count > 0)
            //{
            //    msg += " Missing dependencies: ";
            //    foreach (var kvp in uds)
            //    {
            //        if (kvp.Value.Count == 0) continue;
            //        msg += $"Object of type {kvp.Key.GetType().Name} needs: ";
            //        bool isFirst = true;
            //        foreach (var d in kvp.Value)
            //        {
            //            if (isFirst) isFirst = false; else msg += ", ";
            //            msg += d.Description;
            //        }
            //    }
            //}

            //        //UnsatisfiedDependencies ud;
            //        //if (!component.TryResolveDependencies(out ud, ServiceProvider))
            //        //{
            //        //    uds.Add(component, ud);
            //        //    stillNeedsInitialization.Add(component);
            //        //}
            //        //else 

            #endregion

        }
    }
}
