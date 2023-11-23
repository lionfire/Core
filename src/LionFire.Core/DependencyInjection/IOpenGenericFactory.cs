
namespace LionFire.DependencyInjection;

//public class OpenGenericSingletons
//{

//    private ConcurrentDictionary<TOpenGeneric, object> Singletons => singletons;
//    private ConcurrentDictionary<TOpenGeneric, object> singletons = new();

//    public IServiceProvider ServiceProvider { get; init; }

//    public SingletonGenericFactory(IServiceProvider serviceProvider)
//    {
//        ServiceProvider = serviceProvider;
//    }

//    public T GetSingleton<T>()
//    {
//        throw new NotImplementedException();
//    }
//}

public interface IOpenGenericFactory
{
    TResult Create<TResult>(object[] parameters);
}
