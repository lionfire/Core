using LionFire.Types;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Mvvm;

public class ItemPageVM<TItem, TItemVM> : ReactiveObject, IItemPageVM
//where TItemVM : IViewModel<TItem>
{
    #region Static

    static ItemPageVM()
    {
        DefaultTypeNameRegistryName = typeof(TItem).GetCustomAttributes<TypeNameRegistryAttribute>().FirstOrDefault()?.Name;
    }

    public static string? DefaultTypeNameRegistryName { get; set; }

    #endregion

    #region Dependencies

    public ITypeNameMultiRegistry TypeNames { get; set; }

    #endregion

    #region Parameters

    [Reactive]
    public string? TypeName { get; set; }
    
    public string? TypeNameRegistryName => DefaultTypeNameRegistryName;

    /// <summary>
    /// From URL
    /// </summary>
    [Reactive]
    public string? Id { get; set; }

    #endregion

    #region Lifecycle

    public ItemPageVM(ITypeNameMultiRegistry typeNames, IViewModelProvider viewModelProvider)
    {
        TypeNames = typeNames;
        this.WhenAnyValue(t => t.TypeName)
            .Subscribe((Action<string?>)(typeName =>
            {
                Type = typeName == null ? null : TypeNames.GetTypeFromPreferredRegistry(typeName, TypeNameRegistryName);
            }));


        this.WhenAnyValue(t => t.Model)
            .Subscribe(model => ViewModel = model == null ? default(TItemVM) : viewModelProvider.TryActivate<TItemVM, TItem>(model));
    }

    #endregion

    #region Outputs

    [Reactive]
    public Type? Type { get; set; }

    [Reactive]
    public TItem? Model { get; set; }

    [Reactive]
    public TItemVM? ViewModel { get; set; }

    #endregion
}
