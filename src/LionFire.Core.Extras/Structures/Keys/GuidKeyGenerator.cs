using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Nito.ConnectedProperties;

namespace LionFire.Structures.Keys;

public class GuidKeyGenerator<TKey> : IKeyGenerator<TKey>
    where TKey : class
{
    ConditionalWeakTable<object, TKey> cwt = new();

    public (bool success, TKey? key) TryGetKey(object obj)
    {
        return (true, cwt.GetValue(obj, _ =>
        {
            var guid = Guid.NewGuid();

            if (typeof(TKey) == typeof(Guid)) { return (TKey)(object)guid; }
            if (typeof(TKey) == typeof(string)) { return (TKey)(object)guid.ToString(); }
            if (typeof(TKey) == typeof(byte[])) { return (TKey)(object)guid.ToByteArray(); }
            throw new NotSupportedException($"convert {nameof(Guid)} to {nameof(TKey)}");
        }));
    }
}

