using LionFire.Mvvm;
using LionFire.Orleans_.Collections;
using LionFire.Structures;
using Orleans;

namespace LionFire.Orleans_.Mvvm;

public class GrainListItemVM<TViewModel, TItem> : ViewModel<GrainListItem<TItem>>
    where TItem : IGrain
    where TViewModel : IReadWrapper<TItem>
{
    //public override GrainListItem<TItem> Model { get; set; }
}
