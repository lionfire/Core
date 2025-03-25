#if UNUSED
namespace LionFire.UI.Workspaces;

public interface IWorkspaceView : IUIObject
{
    public IWorkspace Workspace { get;  }

    public IUIObject NavPane { get;  }
    public IUIObject ContentPane { get;  }

}
public class WorkspaceView : UIObject, IWorkspaceView
{
    public IWorkspace Workspace { get; set; }

    public IUIObject NavPane { get; set; }
    public IUIObject ContentPane { get; set; }

}
#endif