#if TODO
#nullable enable
using System;
using System.Threading.Tasks;

namespace LionFire.Shell
{
    /// <summary>
    /// Hosts ASP.NET Blazor server and navigation controls the current route.
    /// </summary>
    /// <remarks>
    /// TODO: 
    ///  - link tabName up with tab somehow
    /// </remarks>
    public class BlazorServerNavigator : INavigator
    {
        public bool Close(string? viewName = null) => throw new NotImplementedException();
        public Task Show(ShellViewReference context, string? viewName = null, ViewParameters? options = null) => throw new NotImplementedException();
    }
}
#endif