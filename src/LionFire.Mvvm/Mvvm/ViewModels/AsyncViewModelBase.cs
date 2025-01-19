
using LionFire.Data.Async;
using LionFire.Metadata;

namespace LionFire.Mvvm;


/// <summary>
/// This simply adds ValueOptions to ViewModelBase
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class AsyncViewModelBase<T> : ViewModelRx<T>, IDependsOn<T>, IViewModel<T>
    where T : class
{
    #region Parameters

    [Relevance(RelevanceFlags.Technical | RelevanceFlags.Internal)]
    public ValueOptions? Options { get; set; }

    #endregion

    #region Construction

    public AsyncViewModelBase(T model) : base(model)
    {
    }

    #endregion
}

