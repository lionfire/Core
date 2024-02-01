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
    //public abstract class ChartController : DependencyObject
    //{
    //    public Color[] colors = new Color[]
    //    {
    //        Colors.Red,
    //        Colors.Blue,
    //        Colors.Green,
    //        Colors.Orange,
    //        Colors.Purple,
    //        Colors.Black,
    //        Colors.Yellow,
    //        Colors.White,
    //        Colors.Gray,
    //    };

    //    #region ChartHost

    //    public ChartHostPanel ChartHost
    //    {
    //        get { return chartHost; }
    //        set
    //        {
    //            if (chartHost == value) return;
    //            chartHost = value;

    //            var ev = ChartHostChanged;
    //            if (ev != null) ev(this, new EventArgs<ChartHostPanel>(value));
    //        }
    //    } private ChartHostPanel chartHost;

    //    public event EventHandler<EventArgs<ChartHostPanel>> ChartHostChanged;

    //    #endregion
        
    //    public ChartPlotter Chart { get { return ChartHost.Chart; } }

    //    public abstract bool Initialize();
    //    public abstract void Update();





    //    #region CanEnable

    //    public bool CanEnable
    //    {
    //        get { return canEnable; }
    //        set
    //        {
    //            if (canEnable == value) return;
    //            canEnable = value;

    //            var ev = CanEnableChanged;
    //            if (ev != null) ev(this, new EventArgs<bool>(value));
    //        }
    //    } private bool canEnable;

    //    public event EventHandler<EventArgs<bool>> CanEnableChanged;

    //    #endregion



        
    //}

    /// <summary>
    /// Interaction logic for ChartHostPanel.xaml
    /// </summary>
    public partial class LionLogPanel : UserControl, INotifyPropertyChanged
    {
        public LionLogPanel()
        {
            InitializeComponent();

            //this.IsEnabledChanged += new DependencyPropertyChangedEventHandler(ChartHostPanel_IsEnabledChanged);
            //this.DataContextChanged += new DependencyPropertyChangedEventHandler(ChartHostPanel_DataContextChanged);
        }

        //void ChartHostPanel_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    UpdateThreadEnabled();
        //}

        //private bool updateThreadEnabled;

        //Thread updateThread;

        //bool initialized = false;
        //bool initializing = false;

        //private static readonly ILogger l = Log.Get();
		
        //private void UpdateThreadMethod()
        //{
        //    while (updateThreadEnabled)
        //    {
        //        Thread.Sleep(1000);

        //        if (!initialized)
        //        {
        //            if (!initializing)
        //            {
        //                initializing = true;

        //                Dispatcher.Invoke(new Action(() =>
        //                {
                            
        //                    bool initializeResult = controller.Initialize();
        //                    if (initializeResult)
        //                    {
        //                        initialized = true;
        //                    }
        //                }));
        //                initializing = false;

        //            }
        //            else
        //            {
        //                l.Trace("UNTESTED");
        //            }
        //            continue;
        //        }

        //        //Dispatcher.BeginInvoke(new Action(() =>
        //                //{
        //                    controller.Update();
        //                //}));
        //    }
        //}

        //private void UpdateThreadEnabled()
        //{
        //    bool shouldUpdateThreadBeEnabled = this.IsEnabled && this.controller != null && this.controller.CanEnable;

        //    if (shouldUpdateThreadBeEnabled)
        //    {
        //        if (updateThreadEnabled) return;

        //        updateThread = new Thread(UpdateThreadMethod);
        //        updateThread.IsBackground = true;
        //        updateThread.Priority = ThreadPriority.BelowNormal;
        //        updateThread.Start();
        //        updateThreadEnabled = true;

        //    }
        //    else
        //    {
        //        if (!updateThreadEnabled) return;
        //        updateThreadEnabled = false; // Thread should detect this and shut down
        //        updateThread = null;
        //    }
        //}

        //void ChartHostPanel_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    if (DataContext == null) return;

            
        //    //this.controller = (ChartController)DataContext;

        //    //this.controller.ChartHost = this;

        //    //UpdateThreadEnabled();

        //}


        //#region Controller

        //public ChartController Controller
        //{
        //    get { return controller; }
        //    set
        //    {
        //        if (controller == value) return;
        //        if (controller != null)
        //        {
        //            controller.CanEnableChanged -= new EventHandler<EventArgs<bool>>(controller_CanEnableChanged);
        //            controller.ChartHost = null;
        //        }
        //        controller = value;
        //        if (controller != null)
        //        {
        //            controller.CanEnableChanged += new EventHandler<EventArgs<bool>>(controller_CanEnableChanged);
        //            controller.ChartHost = this;
        //        }
        //        UpdateThreadEnabled();

        //        OnPropertyChanged("Controller");
        //    }
        //} private ChartController controller;

        //void controller_CanEnableChanged(object sender, EventArgs<bool> e)
        //{
        //    UpdateThreadEnabled();
        //}

        //#endregion
        
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
