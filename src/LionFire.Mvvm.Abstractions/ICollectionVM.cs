using ObservableCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Mvvm;

public interface ICollectionVM<TItem> : IReadOnlyCollection<TItem>, INotifyPropertyChanged
{
    IObservableCollection<TItem>? Collection { get; }

    IEnumerable<Type>? CreateableTypes { get; }

    public bool HasRetrieved { get; }
    public bool IsRetrieving { get; }
    public Task<IEnumerable<TItem>> Retrieve(CancellationToken cancellationToken = default);
    public bool CanRetrieve { get; }

    IEnumerable<TItem> Adding { get; }
    bool IsReadOnly { get; }
    Task Add(TItem item);
    Task AddRange(IEnumerable<TItem> item);

    IEnumerable<TItem> Removing { get; }
    Task<bool> Remove(TItem item);

    // Collection Changed event
    // Item Changed event
}
