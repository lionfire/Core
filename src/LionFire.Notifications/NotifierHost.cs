using System;
using System.Collections.Generic;
using System.Text;
using LionFire.Execution.Executables;
using LionFire.Instantiating;
using System.Collections.ObjectModel;

namespace LionFire.Notifications
{
    public class TNotifierHost : ITemplate
    {
    }

    public interface INotifierHost
    {
        //ReadOnlyObservableCollection<Notifier> Notifiers { get; }
    }

    public class NotifierHost : ExecutableBase, INotifierHost
    {

        public NotifierHost()
        {
            Notifiers = new ReadOnlyObservableCollection<Notifier>(notifiers);
        }

        public ReadOnlyObservableCollection<Notifier> Notifiers { get; private set; }


        private ObservableCollection<Notifier> notifiers = new ObservableCollection<Notifier>();
    }
}
