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
        // TODO THREADSAFETY: C# 8 Default interface implementation, then implement via concurrentdictionary in WorkspaceProvider
        public static IWorkspace Get(this IWorkspaceProvider workspaceProvider, string? key = null, IReference? template = null) 
            => workspaceProvider.Query(key ?? WorkspaceConstants.DefaultWorkspaceKey) ?? workspaceProvider.Create(key ?? WorkspaceConstants.DefaultWorkspaceKey, template);
    }
}
