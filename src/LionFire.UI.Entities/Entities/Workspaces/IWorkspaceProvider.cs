#nullable enable
using LionFire.Referencing;

namespace LionFire.UI.Workspaces
{
    public interface IWorkspaceProvider
    {
        IWorkspace? Query(string key);
        IWorkspace Create(string key, IReference? template = null);
    }

    public static class WorkspaceConstants
    {
        public static string DefaultWorkspaceKey = "Default";
    }
    public static class IWorkspaceProviderExtensions
    {
        public static IWorkspace Get(this IWorkspaceProvider workspaceProvider, string? key = null, IReference? template = null) 
            => workspaceProvider.Query(key ?? WorkspaceConstants.DefaultWorkspaceKey) ?? workspaceProvider.Create(key ?? WorkspaceConstants.DefaultWorkspaceKey, template);
    }
}
