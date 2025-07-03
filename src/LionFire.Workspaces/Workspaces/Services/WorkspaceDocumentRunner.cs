namespace LionFire.Workspaces.Services;

public class WorkspaceDocumentRunner<TKey, TValue, TRunner> : IWorkspaceDocumentRunner<TKey, TValue>
{
    public Type RunnerType => typeof(TRunner);
}
