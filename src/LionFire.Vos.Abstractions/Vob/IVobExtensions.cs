namespace LionFire.Vos
{
    public static class IVobExtensions
    {
        public static T GetOwnRequired<T>(this IVob vob) where T : class => vob.GetOwn<T>() ?? throw new NotFoundException($"{typeof(T).FullName} not found on Vob: {vob}");
        public static T GetNextRequired<T>(this IVob vob, bool skipOwn = false) where T : class => vob.GetNext<T>() ?? throw new NotFoundException($"{typeof(T).FullName} not found on Vob {vob} {(skipOwn ? "":"or its")} ancestors");
    }
}
