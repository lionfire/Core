﻿using LionFire.Dependencies;
using LionFire.Ontology;
using LionFire.Persistence.Filesystemlike;
using LionFire.Serialization;
using LionFire.Structures;
using System.Collections.Generic;

namespace LionFire.Persistence.Filesystem;

public class FilesystemPersisterOptions : FilesystemlikePersisterOptionsBase, IHas<PersisterRetryOptions>
{

    public FilesystemPersisterOptions()
    {
        RetryOptions.MaxGetRetries = 10;
    }

    public static readonly bool AutoDeleteEmptyFiles = true;

    public PersisterRetryOptions RetryOptions { get; set; } = new PersisterRetryOptions();
    PersisterRetryOptions IHas<PersisterRetryOptions>.Object => RetryOptions;

    // FUTURE? Delete if file is all null (saw this on my SSDs after a machine crash) - TODO: some sort of null detection feature, and not FS-specific
    //public static readonly bool AutoDeleteNullFiles = true;


#if UNUSED
    public List<IFilesystemPersistenceInterceptor> Interceptors => interceptors;

    private readonly List<IFilesystemPersistenceInterceptor> interceptors = new List<IFilesystemPersistenceInterceptor>();
#endif
    
}

// OLD. Replaced with FilesystemPersisterOptions
//    public class FilesystemConnectionConfig : NamedHandleProviderConfig
//    {

//        public string RootDirectory { get; set; }

//        //public FilesystemConnectionConfig(string name, string rootDirectory) : base(name)
//        //{
//        //    this.rootDirectory = rootDirectory;
//        //}

//        public override string ConnectionString { get => RootDirectory; set => RootDirectory = value; }

//    }
