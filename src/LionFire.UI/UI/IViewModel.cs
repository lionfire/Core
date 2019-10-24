
using System.Collections.Generic;
using System.Linq;

namespace LionFire.UI
{
    public interface IViewModel { object Model { get; set; } }

    public interface IViewModel<T> : IViewModel
    {
        new T Model { get; set; }
        bool IsViewModelOf(object obj);
    }

    public static class IViewModelExtensions
    {
        public static TTarget GetViewModelOfModel<TTarget, TSource>(IEnumerable<object> objects, TSource source)
            where TTarget : class
        {
            return objects.OfType<IViewModel<TSource>>().FirstOrDefault(v => v.IsViewModelOf(source)) as TTarget;
        }
    }

    // Default implementation of a IViewModel -- useless? Or good base class?
    //public class ViewModel<TValue> : IViewModel<TValue>
    //{
    //    object IViewModel.Model { get { return Model; } set { Model = (TValue)value; } }
    //    public TValue Model { get; set; }
    //    bool IsViewModelOf(object obj);
    //}
}
