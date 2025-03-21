// Retrieved from http://www.codeproject.com/Articles/31592/Editable-TextBlock-in-WPF-for-In-place-Editing#xx
// Under CPOL

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
using System.Windows.Threading;

namespace LionFire.Avalon
{
    public partial class EditableTextBlock : UserControl
    {

        #region Constructor

        public EditableTextBlock()
        {
            InitializeComponent();
            base.Focusable = true;
            base.FocusVisualStyle = null;

            DisplayModeTemplate = this.TryFindResource("DefaultDisplayModeTemplate") as DataTemplate;
            EditModeTemplate = this.TryFindResource("DefaultEditModeTemplate") as DataTemplate;
        }

        #endregion Constructor

        #region Member Variables

        // We keep the old text when we go into editmode
        // in case the user aborts with the escape key
        private string oldText;

        #endregion Member Variables

        #region Properties

        #region DisplayModeTemplate

        /// <summary>
        /// DisplayModeTemplate Dependency Property
        /// </summary>
        public static readonly DependencyProperty DisplayModeTemplateProperty =
            DependencyProperty.Register("DisplayModeTemplate", typeof(DataTemplate), typeof(EditableTextBlock),
                new FrameworkPropertyMetadata((DataTemplate)null,
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets or sets the DisplayModeTemplate property. This dependency property 
        /// indicates ....
        /// </summary>
        public DataTemplate DisplayModeTemplate
        {
            get { return (DataTemplate)GetValue(DisplayModeTemplateProperty); }
            set { SetValue(DisplayModeTemplateProperty, value); }
        }

        #endregion

        #region EditModeTemplate

        /// <summary>
        /// EditModeTemplate Dependency Property
        /// </summary>
        public static readonly DependencyProperty EditModeTemplateProperty =
            DependencyProperty.Register("EditModeTemplate", typeof(DataTemplate), typeof(EditableTextBlock),
                new FrameworkPropertyMetadata((DataTemplate)null,
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets or sets the EditModeTemplate property. This dependency property 
        /// indicates ....
        /// </summary>
        public DataTemplate EditModeTemplate
        {
            get { return (DataTemplate)GetValue(EditModeTemplateProperty); }
            set { SetValue(EditModeTemplateProperty, value); }
        }

        #endregion


        #region Text

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(EditableTextBlock),
            new PropertyMetadata(""));

        #endregion

        #region IsEditable

        public bool IsEditable
        {
            get { return (bool)GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }
        public static readonly DependencyProperty IsEditableProperty =
            DependencyProperty.Register(
            "IsEditable",
            typeof(bool),
            typeof(EditableTextBlock),
            new PropertyMetadata(true));

        #endregion

        #region IsInEditMode

        public bool IsInEditMode
        {
            get 
            {
                if (IsEditable)
                    return (bool)GetValue(IsInEditModeProperty);
                else
                    return false;
            }
            set
            {
                if (IsEditable)
                {
                    if (value) oldText = Text;
                    SetValue(IsInEditModeProperty, value);
                }
            }
        }
        public static readonly DependencyProperty IsInEditModeProperty =
            DependencyProperty.Register(
            "IsInEditMode",
            typeof(bool),
            typeof(EditableTextBlock),
            new PropertyMetadata(false));

        #endregion

        #region TextFormat / FormattedText

        public string TextFormat
        {
            get { return (string)GetValue(TextFormatProperty); }
            set
            {
                if (value == "") value = "{0}";
                SetValue(TextFormatProperty, value);
            }
        }
        public static readonly DependencyProperty TextFormatProperty =
            DependencyProperty.Register(
            "TextFormat",
            typeof(string),
            typeof(EditableTextBlock),
            new PropertyMetadata("{0}"));

        public string FormattedText
        {
            get { return String.Format(TextFormat, Text); }
        }

        #endregion

        #endregion Properties

        #region Event Handlers

        // Invoked when we enter edit mode.
        void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox txt = sender as TextBox;

            // Give the TextBox input focus
            txt.Focus();

            txt.SelectAll();
        }

        // Invoked when we exit edit mode.
        void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            this.IsInEditMode = false;
        }

        // Invoked when the user edits the annotation.
        void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.IsInEditMode = false;
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                this.IsInEditMode = false;
                Text = oldText;
                e.Handled = true;
            }
        }

        #endregion Event Handlers

        private void TextBlock_MouseUp_1(object sender, MouseButtonEventArgs e)
        {
            IsInEditMode = true;
        }

    }
}
