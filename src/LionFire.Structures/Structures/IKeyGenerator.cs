namespace LionFire.Structures;

public interface IKeyGenerator<TKey, TObject>
{
    TKey GetKey(TObject obj);
}
