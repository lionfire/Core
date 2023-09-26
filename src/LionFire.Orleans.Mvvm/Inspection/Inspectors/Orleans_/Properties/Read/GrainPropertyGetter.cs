using LionFire.Data.Async.Gets;
using System.Reflection;

namespace LionFire.Inspection;

public class GrainPropertyGetter : GetterRxO<object>
{
    public GrainPropertyGetter(GrainReadPropertyNode node)
    {
        Node = node;
    }
    public GrainReadPropertyNode Node { get; }

    protected override async ITask<IGetResult<object>> GetImpl(CancellationToken cancellationToken = default)
    {
        // DUPLICATE - GrainPropertyAsyncValue
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
            getResult = new GetResult<object> { Value = value, Flags = TransferResultFlags.Success | (value == null ? TransferResultFlags.NotFound : TransferResultFlags.Found) | TransferResultFlags.Retrieved };
            semaphore.Release();
        }) });

        await semaphore.WaitAsync().ConfigureAwait(false);
        return getResult!;
    }
}
