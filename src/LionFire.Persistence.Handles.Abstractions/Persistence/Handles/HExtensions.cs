namespace LionFire.Persistence
{
    public static class HExtensions
    {
        public static IReadHandleBase<TR> R<TR,TH>(this IReadWriteHandleBase<TH> h) => (IReadHandleBase<TR>)h;
    }

}
