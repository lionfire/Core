﻿using LionFire.Ontology;
using LionFire.Serialization;
using System;
using System.IO;
//using static LionFire.Persistence.Filesystemlike.VirtualFilesystemPersisterBase<TReference, TPersistenceOptions>;

namespace LionFire.Persistence.Persisters;

public class PersisterBase<TOptions> : IHas<TOptions>
    //where TOptions : PersistenceOptions
{
    #region PersistenceOptions

    public TOptions PersistenceOptions
    {
        get => persistenceOptions;
        set
        {
            if (persistenceOptions != null) throw new AlreadySetException();
            persistenceOptions = value;
        }
    }
    private TOptions persistenceOptions;
    TOptions IHas<TOptions>.Object => persistenceOptions;

    #endregion
    
    protected PersisterEvents PersisterEvents { get; }

    public PersisterBase(PersisterEvents? persisterEvents = null)
    {
        PersisterEvents = persisterEvents;
    }


    public virtual bool AllowAutoRetryForThisException(Exception e)
    { 
        if(e is IOException ioe)
        {
            if(ioe.Message.EndsWith("being used by another process.")) { return true; }
        }

        // ENH - better approach for this -- maybe register exception types
        return !(
            e is SerializationException
            || e is FileNotFoundException
            || e is UnauthorizedAccessException
            );
    }

    protected virtual void OnDeserialized(object obj) => (obj as INotifyDeserialized)?.OnDeserialized();
}
