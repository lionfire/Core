using Microsoft.AspNetCore.Components.Authorization;
using LionFire.Applications;
using LionFire.Reactive.Persistence;
using LionFire.UI.Workspaces;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using LionFire.IO.Reactive.Hjson;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System.Diagnostics;
using LionFire.Referencing;
using LionFire.Persistence.Filesystem;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using LionFire.IO.Reactive.Filesystem;
using LionFire.IO.Reactive;
using LionFire.Blazor.Components;
using LionFire.FlexObjects;

namespace LionFire.Workspaces.UI;

public partial class WorkspaceLayoutVM : UserLayoutVM
    //, IFlex
{
    #region Dependencies (some of them)

    public IServiceProvider ServiceProvider { get; }
    public AppInfo AppInfo { get; }

    #endregion

    #region Lifecycle

    public WorkspaceLayoutVM(IServiceProvider serviceProvider, AuthenticationStateProvider AuthStateProvider, AppInfo appInfo, IEnumerable<IWorkspaceServiceConfigurator> workspaceServiceConfigurators) : base(AuthStateProvider)
    {
        ServiceProvider = serviceProvider;
        AppInfo = appInfo;
        WorkspaceServiceConfigurators = workspaceServiceConfigurators;

        userWorkspacesService = new(serviceProvider);

        //this.WhenAnyValue(x => x.UserId).Subscribe(async _ => await OnWorkspaceChanged()).DisposeWith(disposables);
        this.WhenAnyValue(x => x.WorkspaceId)
            .DistinctUntilChanged()
            .Subscribe(async workspaceId => await OnWorkspaceChanged())
            .DisposeWith(disposables);

        this.WhenAnyValue(x => x.UserServices)
            .DistinctUntilChanged()
            .Subscribe(userServices => userWorkspacesService.UserServices = userServices)
            .DisposeWith(disposables);

        PostConstructor();
    }
    protected override async ValueTask OnUserChanged()
    {
        await base.OnUserChanged();
        WorkspaceId = null;
    }

    protected override bool DeferPostConstructor => true;

    #endregion

    #region State

    //object? IFlex.FlexData { get; set; }

    #endregion

    #region User Services

    public bool WorkspacesAvailable => WorkspacesDir != null;
    public string? WorkspacesDir { get => userWorkspacesService.UserWorkspacesDir; }
    UserWorkspacesService userWorkspacesService { get; }

    protected override async ValueTask ConfigureUserServices(IServiceCollection services)
    {
        await base.ConfigureUserServices(services);

        services.AddSingleton(userWorkspacesService);

        string commonAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        var orgDataDir = AppInfo.OrgDir == null ? null : Path.Combine(commonAppDataPath, AppInfo.OrgDir);

        if (userWorkspacesService.UserWorkspaces == null)
        {
            // EXTENSIBILITY: Consider allowing configurability of default location through another IReference type (e.g. Vos) instead of direct filesystem access, though maybe this is what we want in this case:

            Debug.WriteLine($"[WorkspaceLayoutVM] orgDataDir: {orgDataDir}");
            Debug.WriteLine($"[WorkspaceLayoutVM] AppInfo.OrgDir: {AppInfo.OrgDir}");
            Debug.WriteLine($"[WorkspaceLayoutVM] AppInfo.EffectiveDataDirName: {AppInfo.EffectiveDataDirName}");
            Debug.WriteLine($"[WorkspaceLayoutVM] EffectiveUserName: {EffectiveUserName}");

            string? appDataDir;
            if (orgDataDir == null) { appDataDir = null; }
            else { appDataDir = Path.Combine(orgDataDir, AppInfo.EffectiveDataDirName!); }

            Debug.WriteLine($"[WorkspaceLayoutVM] appDataDir: {appDataDir}");

            string? usersDataDir;
            if (appDataDir == null) usersDataDir = null;
            else usersDataDir = Path.Combine(appDataDir, "Users");

            Debug.WriteLine($"[WorkspaceLayoutVM] usersDataDir: {usersDataDir}");

            string? userDir;
            if (usersDataDir == null) userDir = null;
            else userDir = Path.Combine(usersDataDir, EffectiveUserName);

            Debug.WriteLine($"[WorkspaceLayoutVM] userDir: {userDir}");

            if (userDir != null) {
                userWorkspacesService.UserWorkspacesDir = Path.Combine(userDir, "Workspaces");
                Debug.WriteLine($"[WorkspaceLayoutVM] Set UserWorkspacesDir to: {userWorkspacesService.UserWorkspacesDir}");
            }
            else
            {
                Debug.WriteLine($"[WorkspaceLayoutVM] WARNING: userDir is null, cannot set UserWorkspacesDir");
            }
        }
        else
        {
            Debug.WriteLine($"[WorkspaceLayoutVM] Skipping workspace dir setup because UserWorkspaces is already set: {userWorkspacesService.UserWorkspaces}");
        }

        if (userWorkspacesService.UserWorkspaces != null)
        {
            await WorkspaceSchemas.InitFilesystemSchemas(userWorkspacesService.UserWorkspaces);

            try
            {
                services.RegisterObservablesInDir<Workspace>(ServiceProvider, new DirectoryReferenceSelector(userWorkspacesService.UserWorkspaces) { Recursive = true });
            }
            catch (ObjectDisposedException)
            {

            }

            //services.RegisterObservablesInSubDirForType<Workspace>(ServiceProvider, userWorkspacesService.UserWorkspaces);

            //var dirSelector = new LionFire.IO.Reactive.DirectoryReferenceSelector(userWorkspacesService.UserWorkspaces) { Recursive = true };
            //var r = ActivatorUtilities.CreateInstance<HjsonFsDirectoryReaderRx<string, Workspace>>(ServiceProvider, dirSelector);
            //var w = ActivatorUtilities.CreateInstance<HjsonFsDirectoryWriterRx<string, Workspace>>(ServiceProvider, dirSelector);
            //services.AddSingleton<IObservableReaderWriter<string, Workspace>>(sp => new ObservableReaderWriterFromComponents<string, Workspace>(r, w));

            // REVIEW - This isn't very ergonomic, so make this a property on UserWorkspaceService, and make that have a base class for containing entities (Workspaces)
        }
    }

    #endregion

    #region Workspace

    [Reactive]
    private string? _workspaceId;


    public string EffectiveWorkspaceName => WorkspaceId ?? "Anonymous";

    protected virtual async ValueTask OnWorkspaceChanged()
    {
        await DoConfigureWorkspaceServices();
    }

    #region Workspace Services

    public IEnumerable<IWorkspaceServiceConfigurator> WorkspaceServiceConfigurators { get; }

    public IServiceProvider? WorkspaceServices { get; set; }

    private async ValueTask DoConfigureWorkspaceServices()
    {
        var services = new ServiceCollection();

        await ConfigureWorkspaceServices(services, WorkspaceId);

        WorkspaceServices = services.BuildServiceProvider();
    }

    protected virtual async ValueTask ConfigureWorkspaceServices(IServiceCollection services, string? workspaceId)
    {
        Debug.WriteLine($"[WorkspaceLayoutVM.ConfigureWorkspaceServices] WorkspacesDir: {WorkspacesDir}, workspaceId: {workspaceId}");
        Debug.WriteLine($"[WorkspaceLayoutVM.ConfigureWorkspaceServices] WorkspaceServiceConfigurators count: {WorkspaceServiceConfigurators.Count()}");

        if (WorkspacesDir != null)
        {
            var r = new FileReference(WorkspacesDir);
            foreach (var s in WorkspaceServiceConfigurators)
            {
                Debug.WriteLine($"[WorkspaceLayoutVM.ConfigureWorkspaceServices] Calling configurator: {s.GetType().Name}");
                await s.ConfigureWorkspaceServices(services, userWorkspacesService, workspaceId);
            }
        }
        else
        {
            Debug.WriteLine($"[WorkspaceLayoutVM.ConfigureWorkspaceServices] WARNING: WorkspacesDir is null, skipping workspace service configuration");
        }
    }

    #endregion

    #endregion


    
}
