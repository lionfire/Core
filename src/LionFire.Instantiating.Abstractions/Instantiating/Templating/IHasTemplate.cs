namespace LionFire.Instantiating
{
    public interface IHasTemplate
    {
        // REVIEW - consider OnCreatedBy(ITemplate) instead of a set parameter?
        ITemplate Template { get; set; }
    }
}
