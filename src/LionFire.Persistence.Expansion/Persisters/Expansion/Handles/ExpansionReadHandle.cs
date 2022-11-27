using LionFire.Persistence;
using LionFire.Persistence.Handles;
using LionFire.Resolves;
using LionFire.Structures;
using MorseCode.ITask;

namespace LionFire.Persisters.Expansion;

public class ExpansionReadHandle<TValue> : ReadHandleBase<ExpansionReference<TValue>, TValue>, IReadHandle<TValue>
{
    public ExpansionReadHandle(ExpansionReference<TValue> input) : base(input)
    {
    }

    public ExpansionReadHandle(ExpansionReference<TValue> input, TValue? preresolvedValue = default) : base(input, preresolvedValue)
    {
    }

    string IKeyed<string>.Key => Reference.Key;

    protected override ITask<IResolveResult<TValue>> ResolveImpl()
    {
        throw new NotImplementedException();
    }
}
