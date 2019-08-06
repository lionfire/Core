namespace LionFire.Persistence.Resolution
{
    // REVIEW - if this only holds the ReferenceResolutionService, consider collapsing it.  Otherwise, maybe this could hold OBase-specific options.
    public class ReferenceResolutionPolicy
    {
        // TODO: SetOnce
        public ReferenceResolutionService ReferenceResolutionService { get; set; }
    }

    public class ReferenceResolutionPolicy<TOptions>: ReferenceResolutionPolicy
    {
        public TOptions Options { get; set; }
    }

}
