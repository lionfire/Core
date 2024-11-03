using DynamicData;
using LionFire.DynamicData_.Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Orleans_.Serialization.Surrogates;


//[GenerateSerializer]
//public struct LionFireException_Surrogate
//{
//    [Id(0)]
//    public string Message { get; set; }

//}

//[RegisterConverter]
//public sealed class LionFireException_SurrogateConverter : IConverter<LionFireException, LionFireException_Surrogate>
//{
//    public LionFireException ConvertFromSurrogate(in ChangeSet_Surrogate<TItem, TKey> surrogate) => new(surrogate.Data);

//    public ChangeSet_Surrogate<TItem, TKey> ConvertToSurrogate(in ChangeSet<TItem, TKey> value) => new()
//    {
//        Data = value.ToArray(),
//    };
//}

