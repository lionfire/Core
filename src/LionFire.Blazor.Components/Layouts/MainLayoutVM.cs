using Microsoft.AspNetCore.Components.Authorization;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using LionFire.Mvvm;
using System.Reactive.Subjects;
using System.Reactive;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Blazor.Components;

public partial class UserLayoutVM : ReactiveObject
{
    #region Dependencies

    public AuthenticationStateProvider AuthStateProvider { get; }

    #endregion

    public UserLayoutVM(AuthenticationStateProvider AuthStateProvider)
    {
        this.AuthStateProvider = AuthStateProvider;

        if (!DeferPostConstructor) PostConstructor();
    }
    virtual protected void PostConstructor()
    {
        this.WhenAnyValue(x => x.UserId)
            .Subscribe(async _ => await OnUserChanged());
    }
    virtual protected bool DeferPostConstructor => false;


    public async ValueTask InitializeAsync()
    {
        await UpdateAuthenticationState();
    }


    public string EffectiveUserName => UserId ?? "Anonymous";

    [Reactive]
    private string? _userId;

    public async ValueTask UpdateAuthenticationState()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        UserId = user?.Identity?.Name;
    }

    public IObservable<Unit> AuthChanges => authChanges;
    private Subject<Unit> authChanges = new();

    public IServiceProvider? UserServices { get; set; }

    private async ValueTask OnUserChanged()
    {
        await DoConfigureServices();
    }
    private async ValueTask DoConfigureServices()
    {
        var services = new ServiceCollection();

        await ConfigureServices(services);

        UserServices = services.BuildServiceProvider();
    }

    protected virtual ValueTask ConfigureServices(IServiceCollection services)
    {
        return ValueTask.CompletedTask;
    }
}
