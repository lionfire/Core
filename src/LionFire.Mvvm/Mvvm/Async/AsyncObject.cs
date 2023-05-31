using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Mvvm.Async;

public class ObjectOptions
{
    public AsyncPropertyOptions PropertyOptions { get; } = new();
}

public class AsyncObject<TObject> : ReactiveObject
{
    #region Relationships

    [Reactive]
    public TObject Target { get; set; }

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
