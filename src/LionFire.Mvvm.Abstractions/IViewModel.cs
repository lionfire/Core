using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LionFire.Mvvm;

public interface IViewModel { object? Model { get; set; } }

public interface IViewModel<T> : IViewModel
{
    new T? Model { get; set; }
}

public static class IViewModelExtensions
{
    public static TTarget? GetViewModelOfModel<TTarget, TSource>(IEnumerable<object> objects, TSource source)
        where TTarget : class
    {
        return objects.OfType<IViewModel<TSource>>().FirstOrDefault(v => v.IsViewModelOf(source ?? throw new ArgumentNullException(nameof(source)))) as TTarget;
    }
    public static bool IsViewModelOf(this IViewModel vm, object obj) => vm?.Model != null && object.ReferenceEquals(obj, vm.Model);

}
