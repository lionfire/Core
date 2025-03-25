using LionFire.Data.Async.Sets;
using LionFire.Reflection;

namespace LionFire.Data.Mvvm;

public abstract partial class AsyncObservableXCacheVMBase<TKey, TValue, TValueVM>
    : LazilyGetsKeyedCollectionVM<TKey, TValue, TValueVM>
    , ICreatesAsyncVM<TValue>
    //, IActivatableViewModel
    where TKey : notnull
    where TValue : notnull
    where TValueVM : notnull//, IKeyed<TKey>
    //, IViewModel<TValue> // suggested only
{
    #region Lifecycle

    public AsyncObservableXCacheVMBase(IViewModelProvider viewModelProvider) : base(viewModelProvider)
    {
        // TODO: map CanCreate, ReadOnly into this?
        Create = ReactiveCommand.Create<ActivationParameters, Task<TValue>>(
            execute: async p =>
            {
                var newValue = await (EffectiveCreator
                   ?? throw new ArgumentException($"{nameof(Source)} must be ICreatesAsync<TValue>"))
                    .Create(p.Type, p.Parameters);
                OnCreated(newValue);
                return newValue;
            },
            canExecute: Observable.Create<bool>(o =>
            {
                o.OnNext(EffectiveCreator is ICreatesAsync<TValue>);
                return Disposable.Empty;
            })
        );

        Create.CanExecute.ToProperty(this, nameof(CanCreate));
    }

    #endregion

    public virtual ICreatesAsync<TValue>? EffectiveCreator => Creator;
    public ICreatesAsync<TValue>? Creator { get; set; }

    #region ReadOnly

    public bool ReadOnly { get; set; }

    void ValidateCanModify()
    {
        if (ReadOnly) { throw new InvalidOperationException($"ReadOnly is true"); }
        //if (ObservableCache.IsReadOnly) { throw new InvalidOperationException($"ObservableCache.ReadOnly is true"); }
    }

    #endregion

    #region User Input

    #region Create

    public IObservable<(string key, Type type, object[] parameters, Task result)> CreatesForKey => throw new NotImplementedException();
    public ReactiveCommand<ActivationParameters, Task<TValue>> Create { get; }

    [ReactiveUI.SourceGenerators.Reactive]
    private bool _canCreate;

    public IEnumerable<Type> CreatableTypes { get; set; }

    protected virtual void OnCreated(TValue value) { }

    // TODO Triage
    //public async void OnCreate(Type type)
    //{
    //    ValidateCanModify();

    //    try
    //    {
    //        if (Create != null)
    //        {
    //            await Create(type, null);
    //            return;
    //        }
    //        else if (ObservableCache is IObservableCreatesAsync<TValue> cc)
    //        {
    //            await cc.Create(type);
    //            return;
    //        }
    //        //else if (AddNew != null)
    //        //{
    //        //    await AddNew(type);
    //        //    return;
    //        //}
    //        //else if (ObservableCache is IAddsAsync addsNew)
    //        //{
    //        //    await addsNew.AddNew(type);
    //        //    return;
    //        //}
    //        else if (ObservableCache is IAddsAsync<TValue> adds)
    //        {
    //            await adds.Add(await instantiate(type));
    //            return;
    //        }
    //        else
    //        {
    //            throw new NotSupportedException();
    //        }
    //    }
    //    finally
    //    {
    //        //await (ResolvesVM?.Get() ?? Task.CompletedTask);
    //        //StateHasChanged();
    //    }

    //    async Task<TValue> instantiate(Type type)
    //    {
    //        object[] args = Array.Empty<object>();
    //        if (ItemConstructorParameters != null) { args = ItemConstructorParameters.Invoke(type); }

    //        if (Create != null) { return await Create(type, args); }
    //        else if (ObservableCache is IObservableCreatesAsync<TValue> cc && cc.CanCreate)
    //        {
    //            return await cc.Create(type, args);
    //        }

    //        return (TValue)ActivatorUtilities.CreateInstance(ServiceProvider, type, args)
    //            ?? (TValue?)Activator.CreateInstance(type, args)
    //            ?? throw new Exception($"Failed to create item of type {type.FullName}");
    //    }
    //}

    #endregion

    #endregion
}

