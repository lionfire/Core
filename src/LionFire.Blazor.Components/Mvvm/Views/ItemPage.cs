﻿using Microsoft.AspNetCore.Components;
using LionFire.Mvvm;
using ReactiveUI.Blazor;
using ReactiveUI;
using System.ComponentModel;
using System.Threading.Tasks;

namespace LionFire.Blazor.Components;

public class ItemPage<TViewModel> : ReactiveInjectableComponentBase<TViewModel>
    where TViewModel : class, IItemPageVM, INotifyPropertyChanged
{
    [Parameter]
    public string? Id { get; set; }

    [Parameter]
    public string? TypeName { get; set; }


    //public ItemPage()
    //{
    //    this.WhenAnyValue(t => t.Id).BindTo(this, t => t.ViewModel.Id);
    //    this.WhenAnyValue(t => t.TypeName).BindTo(this, t => t.ViewModel.TypeName);
    //}

    protected override Task OnParametersSetAsync()
    {
        ViewModel.Id = Id;
        ViewModel.TypeName = TypeName;

        return base.OnParametersSetAsync();
    }
}
