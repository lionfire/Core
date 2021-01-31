#nullable enable
using LionFire.Referencing;
using System.Collections.Concurrent;
using LionFire.Vos;

namespace LionFire.UI.Workspaces
{
    public class VosWorkspaceProvider : IWorkspaceProvider
    {
        public VobReference WorkspaceRoot { get; set; } = "$workspaces";

        private ConcurrentDictionary<string, IWorkspace> workspaces = new ConcurrentDictionary<string, IWorkspace>();
        public IWorkspace Create(string key, IReference? template = null) => workspaces.GetOrAdd(key, k => new Workspace());
        public IWorkspace? Query(string key)
        {
            return workspaces.TryGetValue(key);
        }
    }
}
