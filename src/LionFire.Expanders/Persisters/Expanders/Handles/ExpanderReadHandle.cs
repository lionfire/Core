using LionFire.ExtensionMethods;
using LionFire.ExtensionMethods.Poco.Resolvables;
using LionFire.Persistence;
using LionFire.Persistence.Handles;
using LionFire.Persistence.Persisters;
using LionFire.Referencing;
using LionFire.Resolvers;
using LionFire.Data.Async.Gets;
using LionFire.Structures;
using MorseCode.ITask;

namespace LionFire.Persisters.Expanders;

// REVIEW - add TSource as generic parameter?
public class ExpanderReadHandle<TValue> : ReadHandleBase<ExpansionReference<TValue>, TValue>, IReadHandle<TValue>, IExpanderReadHandle
{
    #region Dependencies

    // REVIEW - Do I really want to carry ExpanderProvider for the lifetime of the handle?  Maybe rename this to ReresolvableSourceExpansionReadHandle
    public IExpanderProvider ExpanderProvider { get; }

    //public IReferenceToHandleService ReferenceToHandleService { get; } // OLD

    #endregion

    #region Relationships

    public IReference SourceReference { get; }

    // ENH: Make disposable, keep archive open until it's disposed
    public IReadHandle? TargetReadHandle { get; set; }

    #endregion

    #region Lifecycle

    public ExpanderReadHandle(IExpanderProvider expanderProvider, IReference sourceReference, ExpansionReference<TValue> input) : base(input)
    {
        ExpanderProvider = expanderProvider;
        //ReferenceToHandleService = referenceToHandleService;
        SourceReference = sourceReference;
    }

    #endregion

    #region State

    public (IExpander Expander, IReadHandle SourceReadHandle)? resolved { get; private set; }

    #region Derived

    /// <summary>
    /// Type of Value depends on what the IExpander desires (typically byte[] or Stream)
    /// </summary>
    public IReadHandle? SourceReadHandle => resolved?.SourceReadHandle;

    public IExpander? Expander => resolved?.Expander;

    #endregion

    #endregion

    #region Resolution

    public async Task<(IExpander Expander, IReadHandle ReadHandle)?> TryResolveSourceAndExpander()
    {
        if (resolved == null)
        {
            await foreach (var r in ResolveAll())
            {
                if (r.ValidationFailReason == null && r.Expander != null && r.SourceReadHandle != null)
                {
                    resolved = (r.Expander, r.SourceReadHandle);
                    break;
                }
            }
        }
        return resolved;
    }
    public IAsyncEnumerable<(IExpander Expander, IReadHandle SourceReadHandle, string? ValidationFailReason)> ResolveAll()
    {
        return ExpanderHandleHelpers.ResolveAll(ExpanderProvider, SourceReference);
    }


    //async Task ResolveSourceReadHandleIfNeeded()
    //{
    //    if (SourceReadHandle != null) { return; }

    //    var readHandleProvider = ReferenceToHandleService.GetReadHandleProvider(SourceReference) ?? throw new Exception($"Failed to get IReadHandleProvider for {SourceReference}");

    //    // TODO: replace byte[] with proper type

    //    Expander ??= await ResolveExpander();

    //    SourceReadHandle = readHandleProvider.GetReadHandle<byte[]>(SourceReference) ?? throw new Exception($"Failed to get IReadHandle<{typeof(TValue).Name}> for {SourceReference}");
    //}

    #endregion


    #region IExpansionReadHandle

    public void Reset() => resolved = null;

    #endregion

    #region Implementation

    public static IGetResult<TValue> SourceProviderNotAvailable = new RetrieveResult<TValue>
    {
        Flags = TransferResultFlags.ProviderNotAvailable | TransferResultFlags.Fail,
    };

    public bool IsResolved => resolved != null;



    protected override async ITask<IGetResult<TValue>> ResolveImpl()
    {
        (IExpander Expander, IReadHandle SourceReadHandle)? resolved = this.resolved ?? await TryResolveSourceAndExpander();
        if (resolved == null || resolved.Value.SourceReadHandle == null) { return SourceProviderNotAvailable; }

                

        #region Target

        // Example for sourceResolveResult.Value: ZipFile
        
        var targetResolveResult = await resolved.Value.Expander.RetrieveTarget<TValue>(resolved.Value.SourceReadHandle, Reference.Path).ConfigureAwait(false);

        //var targetResolveResult = await resolved.Value.SourceReadHandle.Value.Retrieve<TValue>(new PathReference(Reference.Path));
        if (targetResolveResult.IsSuccess() == false)
        {
            return new RetrieveResult<TValue>()
            {
                Flags = TransferResultFlags.Fail | TransferResultFlags.InnerFail,
                // REVIEW - why can't I cast? Changed InnerResult to object
                //InnerResult = (IGetResult<object>)(IRetrieveResult<object>)(IRetrieveResult<TValue>)targetResolveResult,
                InnerResult = targetResolveResult,
            };
        }
        if (!targetResolveResult.HasValue)
        {
            return targetResolveResult.IsSuccess() switch
            {
                true => RetrieveResult<TValue>.SuccessNotFound,
                false => throw new UnreachableCodeException(),
                //null => RetrieveResult<TValue>.NotFound,
            };
        }

        return new RetrieveResult<TValue>(targetResolveResult.Value, TransferResultFlags.Success | TransferResultFlags.Found);

        #endregion

    }

    #endregion
}
public interface IExpanderReadHandle<TValue> : IReadHandle<TValue>, IReadPersister<PathReference>
{

}

