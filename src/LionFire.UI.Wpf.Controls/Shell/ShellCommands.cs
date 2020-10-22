using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using LionFire.Shell;
//using Microsoft.Practices.Prism.Commands;

namespace LionFire.Avalon
{
    public static class ShellCommands
    {
        //public static DelegateCommand<object> Back
        //{
        //    get;
        //    private set;
        //}
        public static RoutedUICommand Back
        {
            get;
            private set;
        }
        public static RoutedUICommand Save
        {
            get;
            private set;
        }
        public static RoutedUICommand Menu {
            get;
            private set;
        }

        static ShellCommands()
        {
            //Back = new DelegateCommand<object>((x) => { LionFireShell.Instance.MainPresenter.CloseTab(); });
            Save = new RoutedUICommand("Save", "Save", typeof(TabbedWindowPresenter));
            Back = new RoutedUICommand("Back", "Back", typeof(TabbedWindowPresenter));
            Menu = new RoutedUICommand("Menu", "Menu", typeof(TabbedWindowPresenter));
        }

        //private class SaveAssetCommand : RoutedUICommand
        //{
            
        //}
        
    }

}
