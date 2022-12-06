using LionFire.ExtensionMethods;
using LionFire.Persistence;
using LionFire.Persistence.Handles;
using LionFire.Referencing;
using LionFire.Resolvers;
using LionFire.Resolves;
using LionFire.Structures;
using MorseCode.ITask;

namespace LionFire.Persisters.Expanders;

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

    public static IResolveResult<TValue> SourceProviderNotAvailable = new RetrieveResult<TValue>
    {
        Flags = PersistenceResultFlags.ProviderNotAvailable | PersistenceResultFlags.Fail,
    };

    public bool IsResolved => resolved != null;



    protected override async ITask<IResolveResult<TValue>> ResolveImpl()
    {
        if (!IsResolved) { await TryResolveSourceAndExpander(); }

        if (SourceReadHandle == null) { return SourceProviderNotAvailable; }

        // TODO ENH - mount Zip file for read-write?  Or keep open? Locking?

        //Expander.Retrieve(SourceReadHandle, );

        var sourceResolveResult = await SourceReadHandle.Resolve().ConfigureAwait(false);

        if (sourceResolveResult.IsSuccess() == false)
        {
            return new RetrieveResult<TValue>()
            {
                Flags = PersistenceResultFlags.Fail | PersistenceResultFlags.InnerFail,
                InnerResult = sourceResolveResult,
            };
        }
        if (!sourceResolveResult.HasValue)
        {
            return sourceResolveResult.IsSuccess() switch
            {
                true => RetrieveResult<TValue>.SuccessNotFound,
                false => throw new UnreachableCodeException(),
                null => RetrieveResult<TValue>.NotFound,
            };
        }


        throw new NotImplementedException("Exp resolv");
    }

    #endregion
}

