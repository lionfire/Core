namespace LionFire.Persistence
{
    public static class HExtensions
    {
        public static RH<TR> R<TR,TH>(this W<TH> h) => (RH<TR>)h;
    }

}
