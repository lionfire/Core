namespace LionFire.Vos.Mounts
{
    public class VobMountOptions
    {
        public bool AllowMultiMounts { get; set; }

        /// <summary>
        /// False if mounts are sealed
        /// </summary>
        public bool AllowMounts { get; set; }

        public bool IsFrozen { get; set; } // TODO Implement
    }
}
