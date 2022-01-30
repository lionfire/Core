#nullable enable
using LionFire.ExtensionMethods;
using LionFire.Referencing;
using System.Collections.Concurrent;

namespace LionFire.UI.Workspaces
{
    public class InMemoryWorkspaceProvider : IWorkspaceProvider
    {
        private ConcurrentDictionary<string, IWorkspace> workspaces = new ConcurrentDictionary<string, IWorkspace>();

        public IWorkspace Create(string key, IReference? template = null) => workspaces.GetOrAdd(key, k => new Workspace()); // RENAME to QueryOrCreate?
        public IWorkspace? Query(string key)
        {
             return workspaces.TryGetValue(key);
        }

        //public IWorkspace GetOrCreate(string key) // TODO
        //{

        //}
    }
}
