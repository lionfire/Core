using LionFire.DependencyInjection;
using LionFire.Referencing;

namespace LionFire.Persistence.Handles
{
    public interface IReadCollectionHandleProvider
    {
        //RC<T> GetReadCollectionHandle<T>(IReference reference);
    }

    public static class IReadCollectionHandleProviderExtensions
    {
        //public static RC GetReadCollectionHandle(this IReadCollectionHandleProvider handleProvider, IReference reference)
        //    => (RC)handleProvider.GetReadCollectionHandle<object>(reference);

        //public static RC GetReadCollectionHandle(this IReference reference)
        //    => (RC)(DependencyContext.Current.GetService<IReadCollectionHandleProvider>()).GetReadCollectionHandle<object>(reference);
    }

}
