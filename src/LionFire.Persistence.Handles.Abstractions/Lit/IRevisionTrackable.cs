namespace LionFire.Lit
{
    public interface IRevisionTrackable
    {
        string Commit { get; }
        string ParentCommit { get; }
    }



}
