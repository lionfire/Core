using System.Windows.Controls;
using LionFire.Collections;
using LionFire.Processes;
using LionFire.Services;

namespace LionFire.Avalon
{
    /// <summary>
    /// Interaction logic for LocalProcessMonitorPanel.xaml
    /// </summary>
    public partial class LocalProcessMonitorPanel : UserControl
    {
        public LocalProcessMonitor Monitor;
        public LocalProcessMonitorPanel()
        {
            InitializeComponent();
            Monitor = new LocalProcessMonitor();
            Monitor.Settings = new LocalProcessMonitorSettings
            {
                PollTimer = 2000,
                UsePolling = true,
                UseWatcher = true,
            };
            processesDataGrid.ItemsSource = new MultiBindableCollection<RunFile>(Monitor.Items);
            Monitor.Start();

        }
    }
}
