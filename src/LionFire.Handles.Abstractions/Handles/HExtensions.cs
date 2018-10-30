namespace LionFire.Referencing
{
    public static class HExtensions
    {
        public static R<TR> R<TR,TH>(this H<TH> h) => (R<TR>)h;
    }

}
