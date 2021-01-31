#nullable enable
using LionFire.Referencing;
using System.Collections.Concurrent;

namespace LionFire.UI.Workspaces
{
    public class WorkspaceViewProvider : IWorkspaceViewProvider
    {
        private ConcurrentDictionary<string, IWorkspaceView> views = new ConcurrentDictionary<string, IWorkspaceView>();


        public IWorkspaceView Create(string key, IReference? template = null)
        {
            return views.GetOrAdd(key, k => new WorkspaceView());
        }

        public IWorkspaceView? Query(string key) =>  views.TryGetValue(key);
    }
}
