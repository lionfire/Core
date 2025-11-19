using LionFire.Mvvm;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LionFire.Blazor.Components;

public partial class UserLayoutVM : ReactiveObject, IDisposable
{
    #region Dependencies

    public AuthenticationStateProvider AuthStateProvider { get; }

    #endregion

    #region Lifecycle

    public UserLayoutVM(AuthenticationStateProvider AuthStateProvider)
    {
        this.AuthStateProvider = AuthStateProvider;

        if (!DeferPostConstructor) PostConstructor();
    }
    virtual protected void PostConstructor()
    {
        this.WhenAnyValue(x => x.UserId).Subscribe(async _ => await OnUserChanged()).DisposeWith(disposables);
    }
    virtual protected bool DeferPostConstructor => false;

    public bool IsDisposed => disposables.IsDisposed;
    protected CompositeDisposable disposables = new();

    public void Dispose()
    {
        disposables.Dispose();
    }

    public async ValueTask InitializeAsync()
    {
        await UpdateAuthenticationState();
    }

    #endregion

    #region Auth

    public async ValueTask UpdateAuthenticationState()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        UserId = user?.Identity?.Name;
    }

    //public IObservable<Unit> AuthChanges => authChanges;
    //private Subject<Unit> authChanges = new();

    #endregion


    #region User

    public string EffectiveUserName => UserId ?? "Anonymous";
    [Reactive]
    private string? _userId;

    protected virtual async ValueTask OnUserChanged()
    {
        await DoConfigureUserServices();
    }

    #region User Services

    [ReactiveUI.SourceGenerators.Reactive]
    private IServiceProvider? _userServices;

    private async ValueTask DoConfigureUserServices()
    {
        var services = new ServiceCollection();

        await ConfigureUserServices(services);

        UserServices = services.BuildServiceProvider();
    }

    protected virtual ValueTask ConfigureUserServices(IServiceCollection services)
    {
        return ValueTask.CompletedTask;
    }

    #endregion

    #endregion
}
