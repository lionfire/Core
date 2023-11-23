
namespace LionFire.Data.Async.Gets;

public interface IObservableGetterVM
{
    bool IsResolving { get; } // => !gets.Value.AsTask().IsCompleted;
}
