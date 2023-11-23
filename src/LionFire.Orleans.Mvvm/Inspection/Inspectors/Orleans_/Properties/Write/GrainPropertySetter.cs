using LionFire.Data.Async.Sets;

namespace LionFire.Inspection;

public class GrainPropertySetter : Setter<object>
{
    public GrainPropertySetter(GrainWritePropertyNode node)
    {
        Node = node;
    }
    public GrainWritePropertyNode Node { get; }

    public override Task<ISetResult<object>> SetImpl(object? value, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
