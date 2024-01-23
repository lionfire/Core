namespace LionFire.Stride_.Runtime;

public interface ITypedServiceProvider
{
    T? GetService<T>() where T : class;
}

