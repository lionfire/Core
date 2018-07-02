using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using LionFire.Validation;

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
}
