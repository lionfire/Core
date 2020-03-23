namespace LionFire.Instantiating
{
    public static class IPidRootExtensions
    {
        
        public static void EnsureHasPid(this IPidRoot pidRoot, IHasPid obj)
        {
            if (obj == null) return;

            if (obj.Pid != 0) return;

            obj.Pid = pidRoot.KeyKeeper.GetNextKey();
            return;
        }
    }
}
