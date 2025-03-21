// Retrieved on January 8, 2013 under Ms-PL from Codeplex

#region File Header

// -------------------------------------------------------------------------------
// 
// This file is part of the WPFSpark project: http://wpfspark.codeplex.com/
//
// Author: Ratish Philip
// 
// WPFSpark v1.1
//
// -------------------------------------------------------------------------------

#endregion

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace LionFire.Avalon
{
    /// <summary>
    /// Defines the Drag Behavior in the FluidWrapPanel using the Mouse
    /// </summary>
    public class FluidMouseDragBehavior : Behavior<UIElement>
    {
        #region Fields

        FluidWrapPanel parentFWPanel = null;
        ListBoxItem parentLBItem = null;
        ContentPresenter parentContentPresenter = null;
        IFluidItem fluidItem = null;

        FrameworkElement ParentContentControl { get { return parentLBItem ?? (FrameworkElement)parentContentPresenter; } }

        #endregion

        #region Dependency Properties

        #region DragButton

        /// <summary>
        /// DragButton Dependency Property
        /// </summary>
        public static readonly DependencyProperty DragButtonProperty =
            DependencyProperty.Register("DragButton", typeof(MouseButton), typeof(FluidMouseDragBehavior),
                new FrameworkPropertyMetadata(MouseButton.Left));

        /// <summary>
        /// Gets or sets the DragButton property. This dependency property 
        /// indicates which Mouse button should participate in the drag interaction.
        /// </summary>
        public MouseButton DragButton
        {
            get { return (MouseButton)GetValue(DragButtonProperty); }
            set { SetValue(DragButtonProperty, value); }
        }

        #endregion

        #endregion

        #region Overrides

        /// <summary>
        /// 
        /// </summary>
        protected override void OnAttached()
        {
            // Subscribe to the Loaded event
            (this.AssociatedObject as FrameworkElement).Loaded += new RoutedEventHandler(OnAssociatedObjectLoaded);
        }

        void OnAssociatedObjectLoaded(object sender, RoutedEventArgs e)
        {
            // Get the parent FluidWrapPanel and check if the AssociatedObject is
            // hosted inside a ListBoxItem (this scenario will occur if the FluidWrapPanel
            // is the ItemsPanel for a ListBox).
            GetParentPanel();

            // Subscribe to the Mouse down/move/up events
            if (ParentContentControl != null)
            {
                ParentContentControl.PreviewMouseDown += new MouseButtonEventHandler(OnPreviewMouseDown);
                ParentContentControl.PreviewMouseMove += new MouseEventHandler(OnPreviewMouseMove);
                ParentContentControl.PreviewMouseUp += new MouseButtonEventHandler(OnPreviewMouseUp);
            }
            else
            {
                this.AssociatedObject.PreviewMouseDown += new MouseButtonEventHandler(OnPreviewMouseDown);
                this.AssociatedObject.PreviewMouseMove += new MouseEventHandler(OnPreviewMouseMove);
                this.AssociatedObject.PreviewMouseUp += new MouseButtonEventHandler(OnPreviewMouseUp);
            }
        }

        /// <summary>
        /// Get the parent FluidWrapPanel and check if the AssociatedObject is
        /// hosted inside a ListBoxItem (this scenario will occur if the FluidWrapPanel
        /// is the ItemsPanel for a ListBox).
        /// </summary>
        private void GetParentPanel()
        {
            FrameworkElement ancestor = this.AssociatedObject as FrameworkElement;

            while (ancestor != null)
            {
                if (ancestor is ListBoxItem)
                {
                    parentLBItem = ancestor as ListBoxItem;
                }
                if (ancestor is ContentPresenter)
                {
                    parentContentPresenter = ancestor as ContentPresenter;

                    this.fluidItem = parentContentPresenter.Content as IFluidItem;
                }

                if (ancestor is FluidWrapPanel)
                {
                    parentFWPanel = ancestor as FluidWrapPanel;
                    // No need to go further up
                    return;
                }

                // Find the visual ancestor of the current item
                ancestor = VisualTreeHelper.GetParent(ancestor) as FrameworkElement;
            }
        }

        protected override void OnDetaching()
        {
            (this.AssociatedObject as FrameworkElement).Loaded -= OnAssociatedObjectLoaded;
            if (ParentContentControl != null)
            {
                ParentContentControl.PreviewMouseDown -= OnPreviewMouseDown;
                ParentContentControl.PreviewMouseMove -= OnPreviewMouseMove;
                ParentContentControl.PreviewMouseUp -= OnPreviewMouseUp;
            }
            else
            {
                this.AssociatedObject.PreviewMouseDown -= OnPreviewMouseDown;
                this.AssociatedObject.PreviewMouseMove -= OnPreviewMouseMove;
                this.AssociatedObject.PreviewMouseUp -= OnPreviewMouseUp;
            }
        }

        #endregion

        #region Event Handlers

        void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!this.parentFWPanel.AllowReorder) return;
            if (fluidItem.DisableReorder) return;
            if (e.ChangedButton == DragButton)
            {
                Point position = ParentContentControl != null ? e.GetPosition(ParentContentControl) : e.GetPosition(this.AssociatedObject);

                FrameworkElement fElem = this.AssociatedObject as FrameworkElement;
                if ((fElem != null) && (parentFWPanel != null))
                {
                    if (ParentContentControl != null)
                        parentFWPanel.BeginFluidDrag(ParentContentControl, position);
                    else
                        parentFWPanel.BeginFluidDrag(this.AssociatedObject, position);
                }
            }
        }

        void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!this.parentFWPanel.AllowReorder) return;
            if (fluidItem.DisableReorder) return;
            bool isDragging = false;
            
            switch (DragButton)
            {
                case MouseButton.Left:
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        isDragging = true;
                    }
                    break;
                case MouseButton.Middle:
                    if (e.MiddleButton == MouseButtonState.Pressed)
                    {
                        isDragging = true;
                    }
                    break;
                case MouseButton.Right:
                    if (e.RightButton == MouseButtonState.Pressed)
                    {
                        isDragging = true;
                    }
                    break;
                case MouseButton.XButton1:
                    if (e.XButton1 == MouseButtonState.Pressed)
                    {
                        isDragging = true;
                    }
                    break;
                case MouseButton.XButton2:
                    if (e.XButton2 == MouseButtonState.Pressed)
                    {
                        isDragging = true;
                    }
                    break;
                default:
                    break;
            }

            if (isDragging)
            {
                Point position = ParentContentControl != null ? e.GetPosition(ParentContentControl) : e.GetPosition(this.AssociatedObject);

                FrameworkElement fElem = this.AssociatedObject as FrameworkElement;
                if ((fElem != null) && (parentFWPanel != null))
                {
                    Point positionInParent = e.GetPosition(parentFWPanel);
                    if (ParentContentControl != null)
                        parentFWPanel.FluidDrag(ParentContentControl, position, positionInParent);
                    else
                        parentFWPanel.FluidDrag(this.AssociatedObject, position, positionInParent);
                }
            }
        }

        void OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!this.parentFWPanel.AllowReorder) return;
            if (fluidItem.DisableReorder) return;
            if (e.ChangedButton == DragButton)
            {
                Point position = ParentContentControl != null ? e.GetPosition(ParentContentControl) : e.GetPosition(this.AssociatedObject);

                FrameworkElement fElem = this.AssociatedObject as FrameworkElement;
                if ((fElem != null) && (parentFWPanel != null))
                {
                    Point positionInParent = e.GetPosition(parentFWPanel);
                    if (ParentContentControl != null)
                        parentFWPanel.EndFluidDrag(ParentContentControl, position, positionInParent);
                    else
                        parentFWPanel.EndFluidDrag(this.AssociatedObject, position, positionInParent);
                }
            }
        }

        #endregion
    }
}
