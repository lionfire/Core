namespace LionFire.Threading
{
    public interface IDispatcherProvider
    {
        IDispatcher DispatcherFor(object obj);

    }
}
