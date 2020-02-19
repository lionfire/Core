namespace LionFire.Vos.Mounts
{
    public static class VosMountsExtensions
    {
        public static VobMounts Mounts(this IVob vob) => vob.AcquireNext<VobMounts>();

    }
}
