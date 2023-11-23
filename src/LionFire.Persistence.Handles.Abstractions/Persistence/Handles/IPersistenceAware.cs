namespace LionFire.Persistence;

public interface IPersistenceAware<in T>
{
    void OnPreresolved(T? preresolvedValue);
}
