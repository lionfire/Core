#nullable enable
namespace LionFire.Structures.Keys;

public interface IKeyProvider<out TKey> : IKeyProvider<TKey, object>
{
}

public interface IKeyProvider<out TKey, in TValue>
    
{
    TKey GetKey(TValue? value);
}

//public interface IGuaranteedKeyProvider<out TKey> : IGuaranteedKeyProvider<TKey, object>
//{
//}

//public interface IGuaranteedKeyProvider<out TKey, in TValue>
//{
//    TKey GetKey(TValue? value);
//}