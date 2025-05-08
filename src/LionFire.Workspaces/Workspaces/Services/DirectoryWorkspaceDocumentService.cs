using LionFire.Execution;
using LionFire.IO.Reactive.Filesystem;
using LionFire.Persistence.Filesystem;

namespace LionFire.Workspaces.Services;

public class DirectoryWorkspaceDocumentService<TValue /*TValueVM,*/ /*TRunner*/> : WorkspaceDocumentService<string, TValue /*TValueVM,*/ /*TRunner*/>
    where TValue : notnull
    //where TValueVM : IEnablable
    //where TRunner : IRunner<TValue>
{
    static string dir = "C:\\ProgramData\\LionFire\\Trading\\Users"; // TEMP HARDCODE
    public DirectoryWorkspaceDocumentService(IServiceProvider serviceProvider, ILogger<DirectoryWorkspaceDocumentService<TValue /*TValueVM,*/ /*TRunner*/>> logger) : base(serviceProvider, logger,
        serviceProvider
            .RegisterObservablesInDir<TValue>(new IO.Reactive.DirectoryReferenceSelector(new FileReference(dir)) { Recursive = true })(serviceProvider)
        )
    {
    }
}
