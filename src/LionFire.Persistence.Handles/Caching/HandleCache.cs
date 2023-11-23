// OLD, never really used
//using LionFire.Collections;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace LionFire.Persistence.Handles;

//// ENH idea: options for strong vs weak cache per type of TValue

//public class HandleCache
//{

//    public ConcurrentWeakDictionaryCache<string, object> Dict { get; } = new();

//    public TValue GetOrAdd<TValue>(string key, Func<TValue> factory)
//        => (TValue)Dict.GetOrAdd(key, () => factory());
//}
