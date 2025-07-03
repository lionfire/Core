namespace LionFire.Workspaces.Services;

public interface IWorkspaceDocumentRunner<TKey, TValue>
{

    Type RunnerType { get; }
}
