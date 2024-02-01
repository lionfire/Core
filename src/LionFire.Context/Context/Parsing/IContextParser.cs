
namespace LionFire.Context.Parsing;

public interface IContextParser
{
    IContext Parse(IEnumerable<string> tags);
}
