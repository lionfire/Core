using LionFire.Mvvm;
using LionFire.UI.Metadata;
using ReactiveUI;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using LionFire.UI.Components.PropertyGrid;

namespace LionFire.UI.Components;

public class PropertyGridVM : ReactiveObject
{
    [Reactive]
    public object Object { get; set; }

    #region Derived

    public TypeInteractionModel? TypeModel { get; set; }

    #endregion

    public string Title { get; set; } = "Properties";
    public bool ShowTitle { get; set; } = true;

    #region Lifecycle

    public PropertyGridVM()
    {
        this.WhenAnyValue(t => t.Object)
            .Subscribe(o => Init(o));
    }

    public void Init(object o)
    {
        TypeModel = TypeInteractionModels.Get(o?.GetType());
        Title = o?.GetType().Name.ToDisplayString();
    }

    #endregion
}
