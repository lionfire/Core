namespace LionFire.Copying
{
    public static class CopyFlagsSettings
    {
        //public const CopyFlags Default = CopyFlags.ICloneable | CopyFlags.PublicProperties;

        public const CopyFlags Default =
            CopyFlags.ICloneable
            | CopyFlags.ICloneProvider
            | CopyFlags.PublicProperties | CopyFlags.PublicFields
            //| CopyFlags.NonPublicProperties | CopyFlags.NonPublicFields  // RECENTCHANGE
            ;
    }
}
