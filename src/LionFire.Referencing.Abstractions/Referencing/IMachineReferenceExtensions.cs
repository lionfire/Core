namespace LionFire.Referencing
{
    public static class IMachineReferenceExtensions
    {
        public static bool IsLocalhost(IMachineReference reference)
        {
            return ReferenceConstants.IsLocalhost(reference.Host);
        }
    }
}
