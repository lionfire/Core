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

namespace LionFire.Blazor.Components;

public partial class WorkspaceLayoutVM : UserLayoutVM
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
        this.WhenAnyValue(x => x.UserId)
         .Subscribe(async _ => await OnWorkspaceChanged());

        PostConstructor();
    }

    protected override bool DeferPostConstructor => true;

    #endregion

    #region User Services

    public bool WorkspacesAvailable => WorkspacesDir != null;
    public string? WorkspacesDir { get; set; }

    protected override async ValueTask ConfigureUserServices(IServiceCollection services)
    {
        string commonAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        var orgDataDir = AppInfo.OrgDir == null ? null : Path.Combine(commonAppDataPath, AppInfo.OrgDir);

        if (WorkspacesDir == null)
        {
            string? appDataDir;
            if (orgDataDir == null) { appDataDir = null; }
            else { appDataDir = Path.Combine(orgDataDir, AppInfo.EffectiveDataDirName!); }

            string? usersDataDir;
            if (appDataDir == null) usersDataDir = null;
            else usersDataDir = Path.Combine(appDataDir, "Users");

            string? userDir;
            if (usersDataDir == null) userDir = null;
            else userDir = Path.Combine(usersDataDir, EffectiveUserName);

            if (userDir != null) { WorkspacesDir = Path.Combine(userDir, "Workspaces"); }

        }

        if (WorkspacesDir != null)
        {
            await WorkspaceSchemas.InitFilesystemSchemas(WorkspacesDir);

            var dir = new LionFire.IO.Reactive.DirectorySelector(WorkspacesDir) { Recursive = true };
            var r = ActivatorUtilities.CreateInstance<HjsonFsDirectoryReaderRx<string, Workspace>>(ServiceProvider, dir);
            var w = ActivatorUtilities.CreateInstance<HjsonFsDirectoryWriterRx<string, Workspace>>(ServiceProvider, dir);
            services.AddSingleton<IObservableReaderWriter<string, Workspace>>(sp => new ObservableReaderWriterFromComponents<string, Workspace>(r, w));
        }
    }

    #endregion

    #region Workspace

    [Reactive]
    private string? _workspaceId;

    public string EffectiveWorkspaceName => WorkspaceId ?? "Anonymous";

    private async ValueTask OnWorkspaceChanged()
    {
        await DoConfigureWorkspaceServices();
    }

    #region Workspace Services

    public IEnumerable<IWorkspaceServiceConfigurator> WorkspaceServiceConfigurators { get; }

    public IServiceProvider? WorkspaceServices { get; set; }

    private async ValueTask DoConfigureWorkspaceServices()
    {
        var services = new ServiceCollection();

        await ConfigureWorkspaceServices(services);

        WorkspaceServices = services.BuildServiceProvider();
    }

    protected virtual ValueTask ConfigureWorkspaceServices(IServiceCollection services)
    {
        if (WorkspacesDir != null)
        {
            var r = new FileReference(WorkspacesDir);
            foreach (var s in WorkspaceServiceConfigurators)
            {
                s.ConfigureWorkspaceServices(UserServices, r, services);
            }
        }
        return ValueTask.CompletedTask;
    }

    #endregion

    #endregion

}

public interface IWorkspaceServiceConfigurator
{
    ValueTask ConfigureWorkspaceServices(IServiceProvider? userServices, IReference workspaceReference, IServiceCollection services);
}

public class WorkspaceConfiguration
{
    public List<Type> MemberTypes { get; set; } = new();
}
