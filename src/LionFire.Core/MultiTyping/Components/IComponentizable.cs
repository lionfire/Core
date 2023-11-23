namespace LionFire.Structures;

public interface IComponentizable : IComponentized
{
    T GetOrAddComponent<T>() where T : class;
}
