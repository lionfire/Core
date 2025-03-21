#if UNUSED
using LionFire.Applications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LionFire.Avalon
{
    public class AvalonAssemblyInitializer : IAssemblyInitializer
    {
        public void Initialize()
        {
            //CommandManager.RequerySuggested += CommandManager_RequerySuggested; // REVIEW RECENTCHANGE - removed this in favor of attaching events to the actual WPF CommandManager.RequerySuggested  event
        }

        //void CommandManager_RequerySuggested(object sender, EventArgs e)
        //{
        //    LionFire.Events.CommandManager.RaiseRequerySuggested(sender, e);
        //}
    }
}
#endif