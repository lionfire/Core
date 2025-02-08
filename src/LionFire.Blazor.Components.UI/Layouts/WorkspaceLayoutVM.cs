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
using System.Diagnostics;

namespace LionFire.Blazor.Components;

public partial class WorkspaceLayoutVM : UserLayoutVM
{
    #region Dependencies

    public AppInfo AppInfo { get; }

    #endregion

    [Reactive]
    private string? _workspaceId;

    #region Lifecycle

    public WorkspaceLayoutVM(AuthenticationStateProvider AuthStateProvider, AppInfo appInfo) : base(AuthStateProvider)
    {
        AppInfo = appInfo;

        PostConstructor();

        this.PropertyChanged += WorkspaceLayoutVM_PropertyChanged;
    }

    private void WorkspaceLayoutVM_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "WorkspaceId")
        {
            Debug.WriteLine("Workspace: " + WorkspaceId);
        }
    }

    protected override bool DeferPostConstructor => true;

    #endregion

    #region Event handling

    protected override async ValueTask ConfigureServices(IServiceCollection services)
    {
        string commonAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        var orgDataDir = Path.Combine(commonAppDataPath, AppInfo.OrgDir);

        var appDataDir = Path.Combine(orgDataDir, AppInfo.EffectiveDataDirName!);
        var usersDataDir = Path.Combine(appDataDir, "Users");
        var userDir = Path.Combine(usersDataDir, EffectiveUserName);
        var workspacesDir = Path.Combine(userDir, "Workspaces");

        await WorkspaceSchemas.InitFilesystemSchemas(workspacesDir);


        var dir = new LionFire.IO.Reactive.DirectorySelector(workspacesDir) { Recursive = true };
        var r = new HjsonFsDirectoryReaderRx<string, Workspace>(dir);
        var w = new HjsonFsDirectoryWriterRx<string, Workspace>(dir);
        services.AddSingleton<IObservableReaderWriter<string, Workspace>>(sp => new ObservableReaderWriterFromComponents<string, Workspace>(r, w));
    }

    #endregion

}
