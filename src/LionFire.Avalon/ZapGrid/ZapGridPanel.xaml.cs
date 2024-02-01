using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LionFire.Avalon
{
    /// <summary>
    /// Interaction logic for ZapGridPanel.xaml
    /// </summary>
    public partial class ZapGridPanel : UserControl
    {


        #region FUTURE

        // If true, zap scroll through all intermediate items.  If false, scroll immediately to next grid cell as though it was next
        //bool ScrollByIntermediaries;

        //bool RemoveUnusedFromVisualTree;

        //Thickness CellViewMargin; // shrinks view so that neighbouring cells can be seen
        //Thickness CellPaddings; // space between cells

        #endregion


        #region Members

        List<object> items = new List<object>();

        #endregion

        #region Size

        #region Size

        public int CellCount
        {
            get { return gridWidth * gridHeight; }
            //set { size = value; }
        } 

        #endregion

        #region Width

        public int GridWidth
        {
            get { return gridWidth; }
            set { gridWidth = value;
            OnCellCountChanged();
            }
        } private int gridWidth = 1;

        #endregion

        #region Height

        public int GridHeight
        {
            get { return gridHeight; }
            set { gridHeight = value; OnCellCountChanged(); }
        } private int gridHeight = 1;

        #endregion

        private void OnCellCountChanged()
        {
            var c = CellCount;
            while (items.Count < c)
            {
                items.Add(null);
            }
            while (items.Count > c)
            {
                items.RemoveAt(items.Count - 1);
            }
        }

        #endregion

        #region Construction

        public ZapGridPanel()
        {
            InitializeComponent();
            OnCellCountChanged();
            OnCurrentPositionChanged();
        }

        #endregion

        #region Get / Set
        
        public int IndexFor(int x, int y)
        {
            return y * GridWidth + x;
        }
        public object Get(int x, int y)
        {
            var i = IndexFor(x, y);
            if (i > CellCount) return null;
            return items[i];
        }

        public void Set(int x, int y, object obj, bool allowResize = true)
        {
            if (allowResize)
            {
                if (x >= GridWidth) GridWidth = x + 1;
                if (y >= GridHeight) GridHeight = y + 1;
            }
            var i = IndexFor(x, y);
            items[i] = obj;
            OnItemChanged(x, y);
        }

        #endregion
        
        #region CurrentX

        /// <summary>
        /// CurrentX Dependency Property
        /// </summary>
        public static readonly DependencyProperty CurrentXProperty =
            DependencyProperty.Register("CurrentX", typeof(int), typeof(ZapGridPanel),
                new FrameworkPropertyMetadata((int)0,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback(OnCurrentXChanged),
                    new CoerceValueCallback(CoerceCurrentX)));

        /// <summary>
        /// Gets or sets the CurrentX property. This dependency property 
        /// indicates ....
        /// </summary>
        public int CurrentX
        {
            get { return (int)GetValue(CurrentXProperty); }
            set { SetValue(CurrentXProperty, value); }
        }

        /// <summary>
        /// Handles changes to the CurrentX property.
        /// </summary>
        private static void OnCurrentXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ZapGridPanel target = (ZapGridPanel)d;
            int oldCurrentX = (int)e.OldValue;
            int newCurrentX = target.CurrentX;
            target.OnCurrentXChanged(oldCurrentX, newCurrentX);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the CurrentX property.
        /// </summary>
        protected virtual void OnCurrentXChanged(int oldCurrentX, int newCurrentX)
        {
            OnCurrentPositionChanged();
        }

        /// <summary>
        /// Coerces the CurrentX value.
        /// </summary>
        private static object CoerceCurrentX(DependencyObject d, object value)
        {
            ZapGridPanel target = (ZapGridPanel)d;
            int desiredCurrentX = (int)value;
            
            if (desiredCurrentX < 0) desiredCurrentX = 0;
            if (desiredCurrentX >= target.GridWidth) desiredCurrentX = target.GridWidth;

            return desiredCurrentX;
        }

        #endregion

        #region CurrentY

        /// <summary>
        /// CurrentY Dependency Property
        /// </summary>
        public static readonly DependencyProperty CurrentYProperty =
            DependencyProperty.Register("CurrentY", typeof(int), typeof(ZapGridPanel),
                new FrameworkPropertyMetadata((int)0,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback(OnCurrentYChanged),
                    new CoerceValueCallback(CoerceCurrentY)));

        /// <summary>
        /// Gets or sets the CurrentY property. This dependency property 
        /// indicates ....
        /// </summary>
        public int CurrentY
        {
            get { return (int)GetValue(CurrentYProperty); }
            set { SetValue(CurrentYProperty, value); }
        }

        /// <summary>
        /// Handles changes to the CurrentY property.
        /// </summary>
        private static void OnCurrentYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ZapGridPanel target = (ZapGridPanel)d;
            int oldCurrentY = (int)e.OldValue;
            int newCurrentY = target.CurrentY;
            target.OnCurrentYChanged(oldCurrentY, newCurrentY);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the CurrentY property.
        /// </summary>
        protected virtual void OnCurrentYChanged(int oldCurrentY, int newCurrentY)
        {
            OnCurrentPositionChanged();
        }

        /// <summary>
        /// Coerces the CurrentY value.
        /// </summary>
        private static object CoerceCurrentY(DependencyObject d, object value)
        {
            ZapGridPanel target = (ZapGridPanel)d;
            int desiredCurrentY = (int)value;

            if (desiredCurrentY < 0) desiredCurrentY = 0;
            if (desiredCurrentY >= target.GridHeight) desiredCurrentY = target.GridHeight;

            return desiredCurrentY;
        }

        #endregion

        public object CurrentItem
        {
            get 
            {
                var i = IndexFor(CurrentX, CurrentY);
                return items[i];
            }
        }

        private void OnItemChanged(int x, int y)
        {
            if (CurrentX == x && CurrentY == y)
            {
            }
            //TabItem ti;
            //ti.Cont
        }

        #region TabControl Implementation
        
        // Quick and clean
        private void OnCurrentPositionChanged()
        {
            l.Trace("OnCurrentPositionChanged");
// NEXTB // ZapGridPanel
            // TODO - ContentTemplate/Selector?

            //this.TabControl.SelectedI
        }        

        #endregion

        #region Zap Implementation (FUTURE)

        #endregion

        #region Misc

        private static readonly ILogger l = Log.Get();
		
        #endregion
    }
}
