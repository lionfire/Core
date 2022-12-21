using LionFire.Persistence;
using LionFire.Referencing;

namespace LionFire.Persisters.Expanders;

internal static class ExpanderHandleHelpers
{
    public static async IAsyncEnumerable<(IExpander Expander, IReadHandle SourceReadHandle, string? ValidationFailReason)> ResolveAll(IExpanderProvider expanderProvider, IReference sourceReference)
    {
        foreach (var expander in expanderProvider.Expanders)
        {
            var sourceReadHandle = await expander.TryGetSourceReadHandle(sourceReference).ConfigureAwait(false);
            if (sourceReadHandle == null) { continue; }

            // TODO NEXT: SharpZipLibExpander to provide a SharpZipLibReadHandle

            if (sourceReadHandle is IValidatingReadHandle validating)
            {
                // TOTEST - implement IValidatingReadHandle.IsValid (maybe with a default implementation of deserialized entire thing, but optimized implementation of) verifying that the file (or whatever) is compatible
                var validationResult = await validating.IsValid();
                yield return (expander, sourceReadHandle, validationResult.IsValid ? null : validationResult.FailReason);
                continue;
            }
            // ENH: else fall back to Exists?
            // REVIEW: else fall back to Retrieve?

            yield return (expander, sourceReadHandle, null);
        }
    }
}
