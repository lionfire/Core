using LionFire.Mvvm;
using LionFire.Types;
using ReactiveUI;

namespace LionFire.Orleans_.Mvvm;

public partial class GrainPageVM<TGrain, TGrainVM> : ItemPageVM<TGrain, TGrainVM>
    where TGrain : IGrain
{
    #region Dependencies

    public IClusterClient ClusterClient { get; }

    #endregion

    #region Lifecycle

    public GrainPageVM(ITypeNameMultiRegistry typeNames, IClusterClient clusterClient, IViewModelProvider viewModelProvider) : base(typeNames, viewModelProvider)
    {
        ClusterClient = clusterClient;
        this.WhenAnyValue(t => t.Type, t => t.Id)
            .Subscribe(((Type? type, string? id) x) =>
            {
                if (x.type != null && x.id != null)
                {
                    Model = (TGrain) ClusterClient.GetGrain(x.type, x.id);
                }
            });
    }

    #endregion

    #region Upcast

    public TGrain? Grain => Model;

    #endregion
}
