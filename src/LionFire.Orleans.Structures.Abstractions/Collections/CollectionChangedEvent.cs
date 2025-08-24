using DynamicData;
using LionFire.Data.Collections;

namespace LionFire.Orleans_.Collections;

// REVIEW - Use DynamicData or CySharp ObservableCollection event for this?
[GenerateSerializer]
public class CollectionChangedEvent<TItem> : ChangeSet<TItem>
    where TItem : notnull
{
    //[Id(0)]
    //public ChangeSet<TItem> ChangeSet { get; set; }
    //[Id(0)]
    //public CollectionChangeType Type { get; set; }

    //[Id(1)]
    //public List<TItem> Items { get; set; }
}

