using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Events;

public interface ICancelable // RENAME ICancellable
{
    /// <summary>
    /// Request that the operation be canceled, optionally providing a reason for the cancelation.
    /// </summary>
    /// <param name="reason">A string or object containing info about why the cancelation is requested.</param>
    void Cancel(object reason = null);
}

public interface IAsyncCancellable
{
    Task Cancel(CancellationToken cancellationToken = default);
}
