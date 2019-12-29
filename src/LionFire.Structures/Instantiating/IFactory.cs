
namespace LionFire.Structures
{
    public interface IFactory<T>
    {
        T Create(params object[] parameters);
    }
}