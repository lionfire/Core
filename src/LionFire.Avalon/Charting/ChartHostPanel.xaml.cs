using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using Microsoft.Research.DynamicDataDisplay;
using System.ComponentModel;
using Microsoft.Extensions.Logging;

namespace LionFire.Avalon
{
    /// <summary>
    /// Interaction logic for ChartHostPanel.xaml
    /// </summary>
    public partial class ChartHostPanel : UserControl, INotifyPropertyChanged
    {
        public ChartHostPanel()
        {
            InitializeComponent();

            this.IsEnabledChanged += new DependencyPropertyChangedEventHandler(ChartHostPanel_IsEnabledChanged);
            this.DataContextChanged += new DependencyPropertyChangedEventHandler(ChartHostPanel_DataContextChanged);
        }

        void ChartHostPanel_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateThreadEnabled();
        }

        private bool updateThreadEnabled;

        Thread updateThread;

        bool initialized = false;
        bool initializing = false;

        private static readonly ILogger l = Log.Get();
		
        private void UpdateThreadMethod()
        {
            while (updateThreadEnabled)
            {
                Thread.Sleep(1000);

                if (!initialized)
                {
                    if (!initializing)
                    {
                        initializing = true;

                        Dispatcher.Invoke(new Action(() =>
                        {
                            
                            bool initializeResult = controller.Initialize();
                            if (initializeResult)
                            {
                                initialized = true;
                            }
                        }));
                        initializing = false;

                    }
                    else
                    {
                        l.Trace("UNTESTED");
                    }
                    continue;
                }

                //Dispatcher.BeginInvoke(new Action(() =>
                        //{
                            controller.Update();
                        //}));
            }
        }

        private void UpdateThreadEnabled()
        {
            bool shouldUpdateThreadBeEnabled = this.IsEnabled && this.controller != null && this.controller.CanEnable;

            if (shouldUpdateThreadBeEnabled)
            {
                if (updateThreadEnabled) return;

                updateThread = new Thread(UpdateThreadMethod);
                updateThread.IsBackground = true;
                updateThread.Priority = ThreadPriority.BelowNormal;
                updateThread.Start();
                updateThreadEnabled = true;

            }
            else
            {
                if (!updateThreadEnabled) return;
                updateThreadEnabled = false; // Thread should detect this and shut down
                updateThread = null;
            }
        }

        void ChartHostPanel_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext == null) return;

            
            //this.controller = (ChartController)DataContext;

            //this.controller.ChartHost = this;

            //UpdateThreadEnabled();

        }


        #region Controller

        public ChartController Controller
        {
            get { return controller; }
            set
            {
                if (controller == value) return;
                if (controller != null)
                {
                    controller.CanEnableChanged -= new EventHandler<EventArgs<bool>>(controller_CanEnableChanged);
                    controller.ChartHost = null;
                }
                controller = value;
                if (controller != null)
                {
                    controller.CanEnableChanged += new EventHandler<EventArgs<bool>>(controller_CanEnableChanged);
                    controller.ChartHost = this;
                }
                UpdateThreadEnabled();

                OnPropertyChanged("Controller");
            }
        } private ChartController controller;

        void controller_CanEnableChanged(object sender, EventArgs<bool> e)
        {
            UpdateThreadEnabled();
        }

        #endregion
        
        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            var ev = PropertyChanged;
            if (ev != null) ev(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
        
    }
}


#if COPY // REviEW And discard
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using Microsoft.Research.DynamicDataDisplay;
using System.ComponentModel;

namespace LionFire.Avalon
{
    public abstract class ChartController : DependencyObject
    {
        public Color[] colors = new Color[]
        {
            Colors.Red,
            Colors.Blue,
            Colors.Green,
            Colors.Orange,
            Colors.Purple,
            Colors.Black,
            Colors.Yellow,
            Colors.White,
            Colors.Gray,
        };

        #region ChartHost

        public ChartHostPanel ChartHost
        {
            get { return chartHost; }
            set
            {
                if (chartHost == value) return;
                chartHost = value;

                var ev = ChartHostChanged;
                if (ev != null) ev(this, new EventArgs<ChartHostPanel>(value));
            }
        } private ChartHostPanel chartHost;

        public event EventHandler<EventArgs<ChartHostPanel>> ChartHostChanged;

        #endregion
        
        public ChartPlotter Chart { get { return ChartHost.Chart; } }

        public abstract bool Initialize();
        public abstract void Update();

        #region CanEnable

        public bool CanEnable
        {
            get { return canEnable; }
            set
            {
                if (canEnable == value) return;
                canEnable = value;

                var ev = CanEnableChanged;
                if (ev != null) ev(this, new EventArgs<bool>(value));
            }
        } private bool canEnable;

        public event EventHandler<EventArgs<bool>> CanEnableChanged;

        #endregion

    }

    /// <summary>
    /// Interaction logic for ChartHostPanel.xaml
    /// </summary>
    public partial class ChartHostPanel : UserControl, INotifyPropertyChanged
    {
        public ChartHostPanel()
        {
            InitializeComponent();

            this.IsEnabledChanged += new DependencyPropertyChangedEventHandler(ChartHostPanel_IsEnabledChanged);
            this.DataContextChanged += new DependencyPropertyChangedEventHandler(ChartHostPanel_DataContextChanged);
        }

        void ChartHostPanel_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateThreadEnabled();
        }

        private bool updateThreadEnabled;

        Thread updateThread;

        bool initialized = false;
        bool initializing = false;

        private static readonly ILogger l = Log.Get();
		
        private void UpdateThreadMethod()
        {
            while (updateThreadEnabled)
            {
                Thread.Sleep(1000);

                if (!initialized)
                {
                    if (!initializing)
                    {
                        initializing = true;

                        Dispatcher.Invoke(new Action(() =>
                        {
                            
                            bool initializeResult = controller.Initialize();
                            if (initializeResult)
                            {
                                initialized = true;
                            }
                        }));
                        initializing = false;

                    }
                    else
                    {
                        l.Trace("UNTESTED");
                    }
                    continue;
                }

                //Dispatcher.BeginInvoke(new Action(() =>
                        //{
                            controller.Update();
                        //}));
            }
        }

        private void UpdateThreadEnabled()
        {
            bool shouldUpdateThreadBeEnabled = this.IsEnabled && this.controller != null && this.controller.CanEnable;

            if (shouldUpdateThreadBeEnabled)
            {
                if (updateThreadEnabled) return;

                updateThread = new Thread(UpdateThreadMethod);
                updateThread.IsBackground = true;
                updateThread.Priority = ThreadPriority.BelowNormal;
                updateThread.Start();
                updateThreadEnabled = true;

            }
            else
            {
                if (!updateThreadEnabled) return;
                updateThreadEnabled = false; // Thread should detect this and shut down
                updateThread = null;
            }
        }

        void ChartHostPanel_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext == null) return;

            
            //this.controller = (ChartController)DataContext;

            //this.controller.ChartHost = this;

            //UpdateThreadEnabled();

        }


        #region Controller

        public ChartController Controller
        {
            get { return controller; }
            set
            {
                if (controller == value) return;
                if (controller != null)
                {
                    controller.CanEnableChanged -= new EventHandler<EventArgs<bool>>(controller_CanEnableChanged);
                    controller.ChartHost = null;
                }
                controller = value;
                if (controller != null)
                {
                    controller.CanEnableChanged += new EventHandler<EventArgs<bool>>(controller_CanEnableChanged);
                    controller.ChartHost = this;
                }
                UpdateThreadEnabled();

                OnPropertyChanged("Controller");
            }
        } private ChartController controller;

        void controller_CanEnableChanged(object sender, EventArgs<bool> e)
        {
            UpdateThreadEnabled();
        }

        #endregion
        
        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            var ev = PropertyChanged;
            if (ev != null) ev(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
        
    }
}

#endif

#if COPY // REVIEW and discard
<UserControl x:Class="LionFire.Avalon.ChartHostPanel"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006" 
             xmlns:d="http://schemas.microsoft.com/expression/blend/2008" 
             mc:Ignorable="d" 
             xmlns:d3="http://research.microsoft.com/DynamicDataDisplay/1.0"
             Background="Transparent"             
             
             d:DesignHeight="300" d:DesignWidth="300">
    <Grid>
        <Grid.Resources>
            <Style TargetType="Button">
                <Setter Property="Background" Value="Transparent"/>
                <Setter Property="Margin" Value="2"/>
                <Setter Property="Padding" Value="3 0"/>
            </Style>
        </Grid.Resources>
        <DockPanel>
            
            <DockPanel DockPanel.Dock="Bottom" LastChildFill="False">
                <Button>10s</Button>
                <Button>1m</Button>
                <Button>10m</Button>
                <Button>Max</Button>
            </DockPanel>

            <d3:ChartPlotter x:Name="Chart" 
                                 Background="#20FFFFFF" Foreground="#77888888"
                                 DockPanel.Dock="Top"/>

        </DockPanel>
    </Grid>
</UserControl>

#endif
