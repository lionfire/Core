using Microsoft.Research.DynamicDataDisplay;
using System;
using System.Windows;
using System.Windows.Media;

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


}
