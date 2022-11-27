using LionFire.Mvvm;
using LionFire.Orleans_.Collections.PolymorphicGrainListGrain;
using LionFire.Structures;
using Orleans;

namespace LionFire.Orleans_.Mvvm;

public class GrainListItemVM<TViewModel, TItem> : ViewModel<IPolymorphicGrainListGrainItem<TItem>>
    where TItem : IGrain
    where TViewModel : IReadWrapper<TItem>
{
    //public override PolymorphicGrainListItem<TItem> Model { get; set; }
}
