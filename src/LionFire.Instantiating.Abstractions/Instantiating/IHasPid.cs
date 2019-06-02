namespace LionFire.Instantiating
{
    public interface IHasPid
    {
        short Pid { get; set; }
        bool HasPid { get; }
        bool TryEnsureHasPid();
        void EnsureHasPid();

    }
}
