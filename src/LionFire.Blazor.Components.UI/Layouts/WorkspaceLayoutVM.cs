using Microsoft.AspNetCore.Components.Authorization;
using LionFire.Applications;
using LionFire.Reactive.Persistence;
using LionFire.UI.Workspaces;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using LionFire.IO.Reactive.Hjson;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Microsoft.Extensions.Options;

namespace LionFire.Blazor.Components;

public partial class WorkspaceLayoutVM : UserLayoutVM
{
    #region Dependencies

    public AppInfo AppInfo { get; }
    public IOptionsSnapshot<AppInfo> AppInfo2 { get; }

    #endregion

    [Reactive]
    private string? _workspaceId;

    #region Lifecycle

    public WorkspaceLayoutVM(AuthenticationStateProvider AuthStateProvider, AppInfo appInfo, IOptionsSnapshot<AppInfo> appInfo2) : base(AuthStateProvider)
    {
        AppInfo = appInfo;
        AppInfo2 = appInfo2;
    }

    #endregion

    #region Event handling

    protected override async ValueTask ConfigureServices(IServiceCollection services)
    {
        string commonAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        var orgDataDir = Path.Combine(commonAppDataPath, AppInfo.OrgDir);

        var EcosystemDataDir = AppInfo.EffectiveDataDirName;

        var appDataDir = Path.Combine(orgDataDir, EcosystemDataDir);
        var usersDataDir = Path.Combine(appDataDir, "Users");
        var userDir = Path.Combine(usersDataDir, EffectiveUserName);
        var workspacesDir = Path.Combine(userDir, "Workspaces");

        await WorkspaceSchemas.InitFilesystemSchemas(workspacesDir);


        var dir = new LionFire.IO.Reactive.DirectorySelector(workspacesDir) { Recursive = true };
        var r = new HjsonFsDirectoryReaderRx<string, IWorkspace>(dir);
        var w = new HjsonFsDirectoryWriterRx<string, IWorkspace>(dir);
        services.AddSingleton<IObservableReaderWriter<string, IWorkspace>>(sp => new ObservableReaderWriterFromComponents<string, IWorkspace>(r, w));
    }

    #endregion

}
