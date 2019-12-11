using LionFire.Persistence.Persisters;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Persistence.Filesystem
{
    public class FilesystemPersisterProvider : OptionallyNamedPersisterProvider<FileReference, FilesystemPersister, FilesystemPersisterOptions>, IPersisterProvider<ProviderFileReference>
    {
        public FilesystemPersisterProvider(IServiceProvider serviceProvider, IOptionsMonitor<FilesystemPersisterOptions> options) : base(serviceProvider, options)
        { }

        IPersister<ProviderFileReference> IPersisterProvider<ProviderFileReference>.GetPersister(string name) => GetPersister(name);
    }
}
