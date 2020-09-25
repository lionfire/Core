#if WPF
using System;
using System.Windows;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using LionFire.UI;
using LionFire.UI.Wpf;
using LionFire.Shell.Wpf;
using LionFire.Threading;
using LionFire.Dependencies;

namespace LionFire.Shell
{
    public interface IWpfShell // : IHostedService, IKeyboardShell
    {
        Application Application { get; }
    }

    public class WpfShell : WpfShellBase<WpfShellPresenter>, IWpfShell
    {
        #region (Static) Instance

        public static WpfShell Instance => DependencyContext.Default.GetService<WpfShell>();

        #endregion

        public WpfDispatcherAdapter WpfDispatcherAdapter { get; set; }
        public override IDispatcher Dispatcher => WpfDispatcherAdapter;

        public Application Application { get; }

        public WpfShell(IServiceProvider serviceProvider, IHostApplicationLifetime hostApplicationLifetime, Application application, IOptionsMonitor<StartupInterfaceOptions> interfaceOptionsMonitor, IViewLocator viewLocator, WpfShellPresenter shellPresenter, IOptionsMonitor<ShellOptions> shellOptionsMonitor)
            : base(serviceProvider, hostApplicationLifetime, interfaceOptionsMonitor, viewLocator, shellPresenter, shellOptionsMonitor)
        {
            #region Derived

            Application = application;

            WpfDispatcherAdapter = new WpfDispatcherAdapter(Application);

            #endregion
        }

        protected override void OnConstructed()
        {
            System.Windows.Media.Animation.Timeline.DesiredFrameRateProperty.OverrideMetadata( // MOVE?
                typeof(System.Windows.Media.Animation.Timeline),
                new FrameworkPropertyMetadata { DefaultValue = 120 }
                );

            // Doesn't look like this can be here for all derived classes, unless they inherit from this??
            Application.Resources.Source = new Uri("pack://application:,,,/LionFire.UI.Wpf.Controls;component/Resources/default-lfa.xaml");
            //Application.Startup += Application_Startup;

            //InstanceStackRegistrar.Instance.Register<LionFire.Alerting.IAlerter>(new WpfAlerter()); // Fallback
            //InstanceStackRegistrar.Instance.Register<LionFire.Alerting.IAlerter>(this);

            //Alerter.Alert("TEST!!!!", title: "Title1kljlkj", detail: "my detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabc", ex: new ArgumentNullException("arg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lk"));
        }

        protected override void OnClosed()
        {
            Application?.Shutdown(); // Superfluous to StopApplication?
            base.OnClosed();
        }

        #region Derived

        public Window MainWindow => ShellPresenter?.MainPresenter?.CurrentWindow as Window;

        #endregion
    }
}
#endif