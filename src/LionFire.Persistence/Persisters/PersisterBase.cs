#nullable enable

using LionFire.Ontology;
using LionFire.Serialization;
using LionFire.Structures;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
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

    public PersisterBase(IServiceProvider serviceProvider, PersisterEvents? persisterEvents = null)
    {
        DeserializedPipeline = serviceProvider.GetService<DeserializedPipeline>();
        PersisterEvents = persisterEvents;
    }


    public virtual bool AllowAutoRetryForThisException(Exception e)
    {
        if (e is IOException ioe)
        {
            if (ioe.Message.EndsWith("being used by another process.")) { return true; }
        }

        // ENH - better approach for this -- maybe register exception types
        return !(
            e is SerializationException
            || e is FileNotFoundException
            || e is UnauthorizedAccessException
            );
    }

    protected virtual void OnDeserialized<T>(PersistenceOperation operation, DeserializationResult<T> result)
    {
        DeserializedPipeline?.Execute((operation,result));

        if (result.Object is IIdentifiable<string> i && i.Id is null && operation.Id is not null)
        {
            i.Id = operation.Id;
        }

        (result.Object as INotifyDeserialized)?.OnDeserialized();
    }

    // ENH: Named pipelines using .NET 8's IKeyedServiceProvider?
    public DeserializedPipeline? DeserializedPipeline { get; set; }

}

//public class PipelineBuilder<T>
//{
//    private List<Action<T>> list = new();
//    public PipelineBuilder<T> Add(Action<T> action) => list.Add(action);

//    public Pipeline<T> Build() => new Pipeline<T>(list);
//}

public class DeserializedPipeline : Pipeline<(PersistenceOperation, IDeserializationResult)> { }

public class Pipeline<T> : List<Action<T>>
{
    public void Execute(T context)
    {
        foreach (var action in this)
        {
            action(context);
        }
    }

}