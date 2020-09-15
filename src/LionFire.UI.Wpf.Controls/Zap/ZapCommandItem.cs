//#if TOPORT

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using PixelLab.Common;
#if CONTRACTS_FULL
using System.Diagnostics.Contracts;
#else
using PixelLab.Contracts;
#endif

namespace LionFire.Avalon
{
    public class ZapCommandItem : Changeable, ICommand
    {
        protected internal ZapCommandItem(IZapScroller zapScroller, int index)
        {
            //Contract.Requires<ArgumentNullException>(zapScroller != null);
            //Contract.Requires<ArgumentOutOfRangeException>(index >= 0);

            m_zapScroller = zapScroller;

            m_zapScroller.CurrentItemChanged += delegate(object sender, RoutedPropertyChangedEventArgs<int> e)
            {
                OnCanExecuteChanged(EventArgs.Empty);
            };

            m_index = index;

            m_content = m_zapScroller.Items[m_index];
        }

        public object Content
        {
            get { return m_content; ; }
            protected internal set
            {
                if (m_content != value)
                {
                    m_content = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(Content)));
                }
            }
        }

        public int Index => m_index;

        /// <remarks>
        ///     For public use. Most people don't like zero-base indices.
        /// </remarks>
        public int Number => m_index + 1;

        public bool CanExecute => (m_index != m_zapScroller.CurrentItemIndex);

        bool ICommand.CanExecute(object parameter) => CanExecute;

        public event EventHandler CanExecuteChanged;

        protected virtual void OnCanExecuteChanged(EventArgs e)
        {
            OnPropertyChanged(new PropertyChangedEventArgs("CanExecute"));

            EventHandler handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void MakeCurrent()
        {
            m_zapScroller.CurrentItemIndex = m_index;
        }

        void ICommand.Execute(object parameter)
        {
            MakeCurrent();
        }

        public override string ToString()
        {
            string output = (Content == null) ? "*null*" : Content.ToString();

            return string.Format("ZapCommandItem - Index: {0}, Content: {1}", m_index, output);
        }

#region Implementation

        private object m_content;

        private readonly int m_index;
        private readonly IZapScroller m_zapScroller;

#endregion
    } //*** class ZapCommandItem
}

//#endif