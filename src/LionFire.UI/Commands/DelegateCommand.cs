// REVIEW - Consider using DelegateCommand from Microsoft.Practices.Prism.Commands

// Retrieved from http://blogs.msdn.com/b/morgan/archive/2010/06/24/simplifying-commands-in-mvvm-and-wpf.aspx
// on May 3, 2012

// THIS SOFTWARE COMES "AS IS", WITH NO WARRANTIES.  THIS
// MEANS NO EXPRESS, IMPLIED OR STATUTORY WARRANTY, INCLUDING
// WITHOUT LIMITATION, WARRANTIES OF MERCHANTABILITY OR FITNESS
// FOR A PARTICULAR PURPOSE OR ANY WARRANTY OF TITLE OR
// NON-INFRINGEMENT.
//
// MICROSOFT WILL NOT BE LIABLE FOR ANY DAMAGES RELATED TO
// THE SOFTWARE, INCLUDING DIRECT, INDIRECT, SPECIAL,
// CONSEQUENTIAL OR INCIDENTAL DAMAGES, TO THE MAXIMUM EXTENT
// THE LAW PERMITS, NO MATTER WHAT LEGAL THEORY IT IS
// BASED ON.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using LionFire.UI.Commands;

namespace LionFire.Avalon // RENAME LionFire.UI.Commands
{
    /// <summary>
    /// Implements ICommand in a delegate friendly way
    /// </summary>
    public class LionDelegateCommand : ICommand
    {
        /// <summary>
        /// Create a command that can always be executed
        /// </summary>
        /// <param name="executeMethod">The method to execute when the command is called</param>
        public LionDelegateCommand(Action<object> executeMethod) : this(executeMethod, null) { }

        /// <summary>
        /// Create a delegate command which executes the canExecuteMethod before executing the executeMethod
        /// </summary>
        /// <param name="executeMethod"></param>
        /// <param name="canExecuteMethod"></param>
        public LionDelegateCommand(Action<object> executeMethod, Predicate<object> canExecuteMethod)
        {
            if (null == executeMethod)
                throw new ArgumentNullException("executeMethod");

            this._executeMethod = executeMethod;
            this._canExecuteMethod = canExecuteMethod;
        }

        public LionDelegateCommand(Action executeMethod, Func<bool> canExecuteMethod) : this(_ => executeMethod(), _ => canExecuteMethod())
        {
        }


        public bool CanExecute(object parameter)
        {
            return (null == _canExecuteMethod) ? true : _canExecuteMethod(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { LionFireCommandManager.RequerySuggested += value; }
            remove { LionFireCommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            _executeMethod(parameter);
        }

        private Predicate<object> _canExecuteMethod;
        private Action<object> _executeMethod;

        public void RaiseCanExecuteChanged() =>  LionFireCommandManager.InvalidateRequerySuggested();
    }
}
