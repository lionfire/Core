using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LionFire.Data.Async.Gets;
using Newtonsoft.Json.Linq;

namespace LionFire.Orleans_.Serialization.Surrogates;

[GenerateSerializer]
public struct ResolveResultSuccess_Surrogate<T>
{
    [Id(0)]
    public T Value { get; set; }
}


[RegisterConverter]
public sealed class ResolveResultSuccess_SurrogateConverter<T> : IConverter<SuccessGetResult<T>, ResolveResultSuccess_Surrogate<T>>
{
    public SuccessGetResult<T> ConvertFromSurrogate(in ResolveResultSuccess_Surrogate<T> surrogate) => new(surrogate.Value);

    public ResolveResultSuccess_Surrogate<T> ConvertToSurrogate(in SuccessGetResult<T> value) => new()
    {
        Value = value.Value
    };
}
