namespace LionFire.Structures;

public interface IComponentized
{
    T TryGetComponent<T>() where T : class;
}
