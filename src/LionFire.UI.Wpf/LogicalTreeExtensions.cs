using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LionFire.Avalon
{
    public static class LogicalTreeExtensions
    {
        public static T AncestorOfType<T>(this FrameworkElement fe)
            where T : class
        {
            T result;
            do
            {
                if (fe != null)
                {
                    result = fe as T;
                    if(result!=null) return result;
                    
                    fe = fe.Parent as FrameworkElement;
                }
            } while (fe != null);
            return null;
        }
    }
}
