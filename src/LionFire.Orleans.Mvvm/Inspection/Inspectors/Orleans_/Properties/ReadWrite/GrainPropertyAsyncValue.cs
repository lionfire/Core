using LionFire.Data.Async;
using LionFire.Data.Async.Gets;
using LionFire.Data.Async.Sets;
using System.Reflection;

namespace LionFire.Inspection;

public class GrainPropertyAsyncValue : AsyncValue<object>
{
    public GrainPropertyAsyncValue(GrainReadWritePropertyNode node)
    {
        Node = node;
    }

    public GrainReadWritePropertyNode Node { get; }

    public override async Task<ISetResult<object>> SetImpl(object? value, CancellationToken cancellationToken = default)
    {
        var resultObj = (Node.Info.SetMethod ?? throw new NotSupportedException()).Invoke(Node.Source, new object?[] { value })!;

        if (resultObj is Task<ISetResult<object>> sr) { return await sr.ConfigureAwait(false); }
        else if (resultObj is Task t) { await t.ConfigureAwait(false); return SetResult<object>.Success(value); }
        else if (resultObj is ValueTask vt) { await vt.ConfigureAwait(false); return SetResult<object>.Success(value); }
        else throw new NotSupportedException($"Return type: {resultObj?.GetType().FullName}");
    }


    protected override async ITask<IGetResult<object>> GetImpl(CancellationToken cancellationToken = default)
    {
        // DUPLICATE - GrainPropertyGetter
        // REVIEW - is there a cleaner way to get the result?

        var result = (Node.Info.GetMethod ?? throw new NotSupportedException()).Invoke(Node.Source, null);

        var type = (result ?? throw new ArgumentNullException("GetMethod returned null")).GetType();
        var mi = type.GetMethod("GetAwaiter", BindingFlags.Public | BindingFlags.Instance) ?? throw new ArgumentException("GetAwaiter missing on GetMethod result");
        var awaiter = (mi.Invoke(result, null))!; // returns TaskAwaiter<T>

        // ENH: Move semaphore to member variable. If in progress, wait for it to complete
        SemaphoreSlim semaphore = new(0, 1);

        IGetResult<object>? getResult = null;
        var miResult = awaiter.GetType().GetMethod("GetResult", BindingFlags.Public | BindingFlags.Instance) ?? throw new ArgumentException($"GetResult missing on {type.FullName}");

        awaiter.GetType().GetMethod("OnCompleted")!.Invoke(awaiter, new object[] { new Action(() =>
        {
            var value = miResult!.Invoke(awaiter, null);
            getResult = GetResult<object>.Success(value, extraFlags: TransferResultFlags.Retrieved);
            semaphore.Release();
        }) });
        
        await semaphore.WaitAsync().ConfigureAwait(false);
        return getResult!;
    }
}
