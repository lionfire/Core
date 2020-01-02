namespace LionFire.Vos
{
    public static class VosReferenceExtensions
    {
        public static VosReference ToVosReference(this string str) => new VosReference(str);
        public static VosReference ToVosReference(this string[] str) => new VosReference(str);
        //    public static Vob GetVob(this VosReference vosReference)
        //    {
        //        if (vosReference == null) throw new ArgumentNullException("vosReference");
        //        return Vos.Default[vosReference.Path];
        //    }
    }
}
