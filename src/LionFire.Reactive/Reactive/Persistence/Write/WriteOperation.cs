namespace LionFire.Reactive.Persistence;

public class WriteOperation<TKey, TValue>
{
    private readonly TaskCompletionSource<object> _tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);

    public TKey Key { get; }
    public TValue Value { get; }
    public Task Completion => _tcs.Task;

    public WriteOperation(TKey key, TValue value)
    {
        Key = key;
        Value = value;
    }

    public void Complete() => _tcs.TrySetResult(null);
    public void CompleteWithError(Exception ex) => _tcs.TrySetException(ex);
}
