using LionFire.DependencyInjection;
using LionFire.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace LionFire.Execution
{

    // TODO
    //public static class TaskExtensions
    //{
    //    /// <summary>
    //    /// Casts the result type of the input task as if it were covariant
    //    /// </summary>
    //    /// <typeparam name="T">The original result type of the task</typeparam>
    //    /// <typeparam name="TResult">The covariant type to return</typeparam>
    //    /// <param name="task">The target task to cast</param>
    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public static Task<TResult> AsTask<T, TResult>([NotNull] this Task<T> task)
    //        where T : TResult
    //        where TResult : class
    //    {
    //        return task.ContinueWith(t => t.Result as TResult);
    //    }
    //Task<TaskResult> GetResult()
    //{
    //    return Task.FromResult(new Result()).AsTask<Result, ResultBase>();
    //}
    //}

    //public class TaskResult { }
    //public class Result : TaskResult { }


    public interface IInitializable2
    {
        Task<ValidationContext> Initialize();
    }

    public interface IInitializable3
    {
        /// <summary>
        /// Attempt to initialize.  Should typically not throw. 
        /// </summary>
        /// <returns>Null on success, or a reason (such as a string or ValidationContext or Exception) on fail.</returns>
        Task<object> Initialize();
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
        /// <summary>
        /// Throw on TryInitializeAll, else return nothing
        /// </summary>
        public static async Task InitializeAll(this IEnumerable<IInitializable2> initializables, int maxRepetitions = int.MaxValue)
        {
            var result = await initializables.TryValidateAll(async i => await i.Initialize(), int.MaxValue).ConfigureAwait(false);
            if (result == null) return;
            if (result.Count() > 1)
            {
                throw new AggregateException(result.Select(r => new ValidationException(r)));
            }
            else if (result.Count() == 1)
            {
                throw new ValidationException(result.First());
            }
        }

        /// <summary>
        /// On a TryResolveAll failure, throws 1 or more ValidationExceptions
        /// </summary>
        public static async Task InitializeAll(this IEnumerable<IInitializable3> initializables, int maxRepetitions = int.MaxValue)
        {
            var result = await initializables.TryResolveAll(async i => await i.Initialize(), int.MaxValue).ConfigureAwait(false);
            if (result == null) return;
            if (result.Count() > 1)
            {
                throw new AggregateException(result.Select(r => new ValidationException(r)));
            }
            else if (result.Count() == 1)
            {
                throw new ValidationException(result.First());
            }
        }

        public static async Task<IEnumerable<ValidationContext>> TryInitializeAll(this IEnumerable<IInitializable2> initializables, int maxRepetitions = int.MaxValue)
        {
            return await initializables.TryValidateAll(async i => await i.Initialize(), int.MaxValue).ConfigureAwait(false);
        }

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
