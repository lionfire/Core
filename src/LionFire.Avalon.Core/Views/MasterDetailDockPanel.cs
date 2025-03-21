using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace LionFire.Avalon
{
    

    public class MasterDetailDockPanel : ContentControl
    {
        #region Static

        static MasterDetailDockPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MasterDetailDockPanel), new FrameworkPropertyMetadata(typeof(MasterDetailDockPanel)));
        }

        #endregion

        public MasterDetailDockPanel()
        {
        }

        #region Dock

        /// <summary>
        /// Dock Dependency Property
        /// </summary>
        public static readonly DependencyProperty DockProperty =
            DependencyProperty.Register("Dock", typeof(Dock), typeof(MasterDetailDockPanel),
                new FrameworkPropertyMetadata((Dock)Dock.Left,
                    FrameworkPropertyMetadataOptions.AffectsArrange));

        /// <summary>
        /// Gets or sets the Dock property. This dependency property 
        /// indicates where the master view is displayed.
        /// </summary>
        public Dock Dock
        {
            get { return (Dock)GetValue(DockProperty); }
            set { SetValue(DockProperty, value); }
        }

        #endregion

    }
}
