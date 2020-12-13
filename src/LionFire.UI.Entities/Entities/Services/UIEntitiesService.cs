using LionFire.UI.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.UI
{
    public class UIEntitiesService : IUIEntitiesService
    {

        public UIStarter Starter { get; protected set; }

        public UIEntitiesService(UIStarter starter)
        {
            Starter = starter;
        }

        #region State

        public IUIRoot UIRoot { get; protected set; }

        #endregion

        #region ILifetime

        public CancellationToken Started => throw new System.NotImplementedException();

        public CancellationToken Stopping => throw new System.NotImplementedException();

        public CancellationToken Stopped => throw new System.NotImplementedException();


        public void StopUI()
        {
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Starter.StartAsync(cancellationToken).ConfigureAwait(false);
            UIRoot = Starter.UIRoot;
            Starter = null;
        }

        public Task StopAsync(CancellationToken cancellationToken) => throw new System.NotImplementedException();

        #endregion
    }

    //public class UIAccessor
    //{
    //    public IUIRoot Root { get; }
    //    public IOptionsMonitor<UIConventions> MainUIOptionsMonitor { get; }
    //    public UIConventions UIConventions { get; }

    //    public UIAccessor(IUIRoot root, IOptionsMonitor<UIConventions> conventionsOptionsMonitor)
    //    {
    //        Root = root;
    //        MainUIOptionsMonitor = conventionsOptionsMonitor;
    //    }

    //    public IUICollection MainPresenter
    //    {
    //        get
    //        {
    //            return Root.GetOrCreate<IUICollection>(UIConventions.MainWindow).GetOrCreate(UIConventions.MainPresenter);
    //        }
    //    }

    //}
}
