#nullable enable
using LionFire.Referencing;

namespace LionFire.UI.Workspaces
{
    public interface IWorkspaceViewProvider
    {
        IWorkspaceView? Query(string key);
        IWorkspaceView Create(string key, IReference? template = null);

    }
}
