namespace LionFire.Execution.Jobs
{
    public interface IHasStartBlockers
    {
        BlockerCollection StartBlockers { get; }
    }
}
