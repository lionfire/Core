namespace LionFire.Vos
{
    public class VobNodeOptions
    {
        public bool AllowMultiMounts { get; set; }

        /// <summary>
        /// False if mounts are sealed
        /// </summary>
        public bool AllowMounts { get; set; }

        public bool IsFrozen { get; set; } // TODO Implement
    }
}
