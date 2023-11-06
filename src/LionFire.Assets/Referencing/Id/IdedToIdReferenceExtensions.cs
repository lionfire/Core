using LionFire.Data.Id;
using LionFire.Dependencies;
using System;


namespace LionFire.Data.Id
{
    public static class IdedToIdReferenceExtensions
    {
        public static IdReference<TValue> ToIdReference<TValue>(this IIdentified<string> ided)
        {
            if (ided == null) return null;
            return new IdReference<TValue>(ided.Id ?? throw new ArgumentNullException(nameof(ided.Id)));
        }

        public static IdReference<TValue> ToIdReference<TValue>(this object obj)
        {
            var adapter = DependencyContext.Current.GetService<IdAdapter>();
            if (adapter == null) return null;
            var result = adapter.TryGetId(obj);
            if (!result.Item1) return null;

            return new IdReference<TValue>(result.Item2);
        }
    }
}
