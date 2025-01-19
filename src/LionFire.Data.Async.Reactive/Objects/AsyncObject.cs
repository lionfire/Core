using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Data.Async;

/// <summary>
/// Represents an Async Object that has its members accessed in an async manner.
/// The intended purpose is to act as a write-through cache or proxy for a remote object.
/// </summary>
/// <typeparam name="TObject"></typeparam>
public partial class AsyncObject<TObject> : ReactiveObject
{
    #region Relationships

    [ReactiveUI.SourceGenerators.Reactive]
    private TObject _target;

    #endregion

    #region Parameters  

    public ObjectOptions Options { get; } 
    public static ObjectOptions DefaultOptions = new();

    #endregion

    #region Lifecycle

    public AsyncObject(TObject target, ObjectOptions? options = null)
    {
        Target = target;
        Options = options ?? DefaultOptions;
    }

    #endregion

}
