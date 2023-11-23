using LionFire.Referencing;

using LionFire.Data.Async.Gets;
using LionFire.Resolvers;

namespace LionFire.Persisters.Expanders;

public class ExpanderProvider : IGetsSync<IReference, IExpander>, IExpanderProvider
{
    public IEnumerable<IExpander> Expanders => expanders;
    IEnumerable<IExpander> expanders;

    List<IExpander>? expandersWithoutSelectionOptions;

    public ExpanderProvider(IEnumerable<IExpander> expanders)
    {
        this.expanders = expanders;
        foreach (var expander in expanders)
        {
            bool gotOptions = false;
            if (expander is ISupportsFileExtensions sfe)
            {
                gotOptions = true;
                foreach (var ext in sfe.FileExtensions)
                {
                    FileExtensionResolver ??= new();
                    FileExtensionResolver.Dictionary.TryAdd(ext, expander);
                }
            }

            if (gotOptions)
            {
                (expandersWithoutSelectionOptions ??= new()).Add(expander);
            }
        }
    }

    protected DictionaryResolver<string, IExpander>? FileExtensionResolver { get; set; }

    public IGetResult<IExpander> Resolve(IReference reference)
    {
        // FUTURE: Configurable or more complex resolution order. For now it is just file extension, but in the future it could involve schema or other.

        if (FileExtensionResolver != null)
        {
            var ext = Path.GetExtension(reference.Path)?.TrimStart('.');
            if (ext != null)
            {
                var extResolved = FileExtensionResolver.Resolve(ext);
                if (extResolved.HasValue)
                {
                    return extResolved;
                }
            }
        }
        return NotFoundGetResult<IExpander>.Instance;
    }
}
