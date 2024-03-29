#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using LionFire.Vos.Blazor;
using LionFire.Vos;
using LionFire.Referencing;
using LionFire.Threading;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using LionFire.Persistence;
using System.Reflection;
using LionFire.Data.Async.Gets;
using MudBlazor;
using LionFire.Vos.Mounts;
using System.IO;
using System.Diagnostics;

namespace LionFire.Vos.Blazor;

public partial class VosExplorer
{
    public IVob Vob { get; set; }
    public VobMounts? VobMounts { get; set; }

    #region CurrentObject

    [Parameter]
    public object CurrentObject
    {
        get => currentObject;
        set
        {
            if (currentObject == value)
                return;
            if (currentObject != null && currentObject is System.ComponentModel.INotifyPropertyChanged inpcDisabling)
            {
                inpcDisabling.PropertyChanged -= OnCurrentObjectPropertyChanged;
            }

            currentObject = value;
            if (currentObject != null && currentObject is System.ComponentModel.INotifyPropertyChanged inpcEnabling)
            {
                inpcEnabling.PropertyChanged += OnCurrentObjectPropertyChanged;
            }
        }
    }

    private object? currentObject;

    #endregion

    private void OnCurrentObjectPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        InvokeAsync(async () =>
        {
            Logger.LogTrace("OnCurrentObjectPropertyChanged");
            await Refresh();
            StateHasChanged();
        });
    }

    #region Parameters

    [Parameter]
    public string Root { get; set; }

    #region Path

    [Parameter]
    public string Path
    {
        get => path;
        set
        {
            if (value == null)
            {
                value = "";
            }

            if (!value.Contains(":"))
            {
                if (!value.StartsWith("/") && !value.StartsWith("\\"))
                {
                    value = "/" + value;
                }

                //value = $"vos:{value}";
            }

            path = value;
        }
    }

    private string path;

    #endregion

    #endregion

    #region State

    Stack<object> History = new Stack<object>();
    Stack<(object, string)> ForwardHistory = new Stack<(object, string)>();
    Stack<string> BreadCrumbPath = new Stack<string>();
    private IListing<object>[] listings;
    //private List<IReadHandle<object>> readHandles;

    #endregion

    #region Initialization
    protected override async Task OnInitializedAsync()
    {
        var x = ServiceProvider.GetService(typeof(IReferenceProviderService));
        navManager.LocationChanged += OnLocationChanged;
        await UpdateRoot();
        await Refresh();
        await base.OnInitializedAsync();
    }

    #endregion

    #region UI Event handlers

    public void ClickListing(MouseEventArgs e, IListing<object> listing)
    {
        Debug.WriteLine(e);
        GoToPath(listing.Name);
    }
    #endregion

    #region Refresh

    private async Task UpdateRoot()
    {
        //logger.LogInformation("OnInitializedAsync " + uri);
        //System.Diagnostics.Debug.WriteLine("Uri: " + uri);
        //if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("Root", out var param))
        //{
        //    System.Diagnostics.Debug.WriteLine("Root: " + param);
        //    Root = param.First();
        //}
        //else
        //{
        //    System.Diagnostics.Debug.WriteLine("Uri could not find root: " + uri);
        //}
        Root = this.Path;
        //this.Path = Root;
        await Refresh();
        StateHasChanged();
    }

    private async void OnLocationChanged(object sender, LocationChangedEventArgs args)
    {
        Logger.LogInformation("OnLocationChanged " + uri);
        await UpdateRoot();
    }

    public Task GoToPath(string child)
    {
        if (child == "..")
        {
            Path = LionPath.GetParent(Path);
        }
        else
        {
            Path = LionPath.Combine(Path, child);
        }

        this.Vob = rootManager.GetVob(Path);
        this.VobMounts = Vob?.Acquire<VobMounts>();
        this.navManager.NavigateTo("/v/" + Path?.TrimStart('/'));
        return Refresh();
    }

    public async Task RetrieveValue()
    {
        object newValue = null;
        try
        {
            var rh = ("vos:" + Path).ToReference()?.GetReadHandle<object>(); // TODO - get available types
            if (rh != null)
            {
                var result = (await rh.GetIfNeeded().ConfigureAwait(false));
                newValue = result.Value;
                if (result.IsSuccess != true)
                {
                    Logger.Debug($"Failed to retrieve {Path}: {result}");
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Exception when retrieving handle or value");
        }

        CurrentObject = newValue;
    }

    public async Task Refresh()
    {
        var root = rootManager.Get();
        var path = Path;
        if (path != null)
        {
            if (LionUri.TryGetUriScheme(path) == null)
            {
                path = "vos:" + path;
            }
            //if (path.StartsWith("file:"))
            //{
            //    path = "/file";
            //}
        }

#if OLD
//var hList = ((IReference)root[path].Reference).GetListingsHandle();
#else
        var reference = path?.ToReference();
        var hList = reference?.GetListingsHandle();
#endif
        if (hList == null)
        {
            listings = null;
            return;
        }

        var result = await hList.Get();
        listings = (result?.Value.Value?.Where(l=>l.IsDirectory) ?? Array.Empty<IListing<object>>())
            .Concat(result?.Value.Value?.Where(l => !l.IsDirectory) ?? Array.Empty<IListing<object>>())
            .ToArray();
        if (listings.Length == 0)
        {
            // FIXME - why is hList empty?????????
            var list = new List<IListing<object>>();
            var vob = root[reference.Path];
            foreach (var x in vob)
            {
                list.Add(new Listing<object>(x.Name));
            }

            listings = list.ToArray();
        }


        //var newReadHandles = new List<IReadHandle<object>>();
        foreach (var listing in listings)
        {
            if (listing.IsDirectory)
            {
                listing.Type = typeof(LionFire.Vos.Directory);
                continue;
            }
            if (DeserializeAllFilesToDetermineType)
            {
                try
                {
                    var childPath = LionPath.Combine(path, listing.Name);
                    var handle = childPath.ToReference<object>().GetReadHandle<object>();
                    //newReadHandles.Add(handle);
                    listing.Type = (await handle.GetIfNeeded().ConfigureAwait(false))?.GetType();
                }
                catch
                {
                    listing.Type = null;
                }
            }
        }

        //readHandles = newReadHandles;
        await RetrieveValue().ConfigureAwait(false);
    }

    public bool DeserializeAllFilesToDetermineType = false;

    #endregion

    public bool CanUp => Path != "/";

    void Pop()
    {
        ForwardHistory.Push((CurrentObject, BreadCrumbPath.Pop()));
        CurrentObject = History.Pop();
    }

    void Push(object val, string name, bool fromForward = false)
    {
        if (val == null)
            return;
        if (!fromForward && ForwardHistory.Any() && Object.ReferenceEquals(ForwardHistory.Peek().Item1, val))
        {
            Forward();
            return;
        }

        History.Push(CurrentObject);
        BreadCrumbPath.Push(name);
        CurrentObject = val;
        if (!fromForward)
        {
            ForwardHistory.Clear();
        }
    }

    void Back()
    {
        if (History.Count > 0)
        {
            Pop();
        }
    }

    void Forward()
    {
        if (ForwardHistory.Count == 0)
        {
            return;
        }

        var next = ForwardHistory.Pop();
        Push(next.Item1, next.Item2, fromForward: true);
    }

    #region (Private) Utility methods

    private Uri uri => navManager.ToAbsoluteUri(navManager.Uri);
    public string IconClasses(IListing<object> listing)
    {
        if (listing.IsDirectory)
        {
            return "oi oi-folder";
        }

        return "oi oi-chevron-right";
    }
    #endregion

}