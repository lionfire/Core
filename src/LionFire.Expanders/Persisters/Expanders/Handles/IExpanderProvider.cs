using LionFire.Data.Gets;
using LionFire.Referencing;

namespace LionFire.Persisters.Expanders;

public interface IExpanderProvider : IGetsSync<IReference, IExpander>
{
    public IEnumerable<IExpander> Expanders { get; }
}
