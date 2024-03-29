﻿using LionFire.Applications;
using LionFire.Persistence;
using LionFire.Data.Async.Gets;
using LionFire.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MorseCode.ITask;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace LionFire.Settings;

// TODO: Move to separate DLL?  LionFire.Preferences, or LionFire.Applications?

public class SettingsManager : IHostedService
{
    #region Dependencies

    public IOptionsMonitor<SettingsOptions> SettingsOptionsMonitor { get; }
    //public AppInfo AppInfo { get; }

    public SettingsOptions Options => SettingsOptionsMonitor.CurrentValue;

    #endregion

    #region Construction

    public SettingsManager(IOptionsMonitor<SettingsOptions> settingsOptionsMonitor, IServiceProvider serviceProvider)
    {
        SettingsOptionsMonitor = settingsOptionsMonitor;
        //AppInfo = serviceProvider.GetService<AppInfo>();
    }

    #endregion

    #region Methods

    #region IsAutoSaveEnabled

    public bool IsAutoSaveEnabled
    {
        get => isAutoSaveEnabled;
        set
        {
            isAutoSaveEnabled = value;
        }
    }
    private bool isAutoSaveEnabled;

    #endregion


    public Task Save()
    {
        return Task.WhenAll(Options.Handles.Select(h => h.Set()));
    }

    public Task Load()
    {
        return Task.WhenAll(Options.Handles.Select(h => h.GetValue().AsTask()));
    }

    #endregion

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await Load().ConfigureAwait(false);

        if (Options.AutoSave)
        {
            foreach (var handle in Options.Handles)
            {
                handle.EnableAutoSave();
            }
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (Options.AutoSave)
        {
            foreach (var handle in Options.Handles)
            {
                handle.EnableAutoSave(enable: false);
            }
        }

        if (Options.SaveOnExit)
        {
            await Save().ConfigureAwait(false);
        }
    }
}
