using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LionFire.Mvvm;

//public interface IViewModel { object? Model { get; set; } }

public interface IViewModel<out TModel> : IReadWrapper<TModel> //: IViewModel
{
    //TModel IReadWrapper.Value => Model;

    //TModel? Value { get; set; }
}

public static class IViewModelExtensions
{
    public static TViewModel? GetViewModelOfModel<TViewModel, TModel>(IEnumerable<object> objects, TModel source)
        where TViewModel : class
    {
        //return objects.OfType<IViewModel<TSource>>().FirstOrDefault(v => v.IsViewModelOf(source ?? throw new ArgumentNullException(nameof(source)))) as TTarget;
        return objects.OfType<IViewModel<TModel>>().FirstOrDefault(v => ReferenceEquals(v.Value,source)) as TViewModel;
    }
    public static bool IsViewModelOf<TViewModel, TModel>(this IViewModel<TModel> vm, TModel obj)
        where TViewModel : IViewModel<TModel>
        where TModel : class
        => vm?.Value != default && object.ReferenceEquals(obj, vm.Value);

}

