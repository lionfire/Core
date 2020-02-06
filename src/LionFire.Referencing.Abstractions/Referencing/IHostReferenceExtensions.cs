namespace LionFire.Referencing
{
    public static class IHostReferenceExtensions
    {
        public static bool IsLocalhost(IHostReference reference)
        {
            return ReferenceConstants.IsLocalhost(reference.Host);
        }
    }
}
