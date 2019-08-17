namespace LionFire.Persistence
{
    public static class HExtensions
    {
        public static RH<TR> R<TR,TH>(this H<TH> h) => (RH<TR>)h;
    }

}
