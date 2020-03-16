using LionFire.Persistence;

namespace LionFire.Instantiating
{
    public interface IHasTemplate
    {
        // REVIEW - consider OnCreatedBy(ITemplate) instead of a set parameter?
        ITemplate Template { get; set; }
    }
    public interface IHasRTemplate
    {
        IReadHandleBase<ITemplate> RTemplate { get; }
    }
}
