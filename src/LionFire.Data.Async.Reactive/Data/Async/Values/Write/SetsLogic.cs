using DynamicData;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Data.Async.Sets;

internal static class SetsLogic<TValue>
{
    internal static BehaviorSubject<ISetOperation<TValue>> InitSetOperations => new(NoopSetOperation<TValue>.Instantiated); //  new (new SetOperation<TValue>(default, Task.FromResult<ITransferResult>(TransferResult.Initialized).AsITask()));

    internal static bool IsSetStateSetting(ISetOperation<TValue> setState) => setState.Task != null && setState.Task.AsTask().IsCompleted == false;

    internal static async Task<ISetResult<TValue>> Set(ISetsInternal<TValue> @this, TValue? value, CancellationToken cancellationToken = default) 
    {
        ISetOperation<TValue> currentSetState;
    start:
        do
        {
            currentSetState = @this.SetState;
            if (!currentSetState.Task.AsTask().IsCompleted)
            {
                if (@this.EqualityComparer.Equals(currentSetState.DesiredValue, value))
                {
                    // Something already set to the same value, so return that task.
                    return await currentSetState.Task;
                }
                else
                {
                    // Wait for the current set to complete, because we have to do another one.
                    await currentSetState.Task.ConfigureAwait(false);
                }
            }

            // ENH: Based on option: Also wait for existing get/set to complete to avoid setting to a value that will be overwritten, or to avoid setting to a value that is the same as the gotten value
        } while (currentSetState.Task != null);

        lock (@this.setLock)
        {
            if (SetsLogic<TValue>.IsSetStateSetting(@this.SetState)) goto start;
            @this.sets.OnNext(new SetOperation<TValue>(value, @this.SetImpl(value, cancellationToken).AsITask()));
        }
        await @this.SetState.Task.ConfigureAwait(false);

        return await currentSetState.Task!.ConfigureAwait(false);
    }

    internal static async Task<ISetResult<TValue>> Set(ISetsInternal<TValue> @this, CancellationToken cancellationToken = default)
    {
        // ENH idea: 
        //Options.Set.DoSet(() => () =>
        //{
        //    return Set(StagedValue, cancellationToken).ConfigureAwait(false);
        //});

        try
        {
            return await @this.SetImpl(@this.StagedValue, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            if (@this.Options.Set.ThrowOnException) throw;
            else return SetResult<TValue>.FromException(ex);
        }
    }
}
