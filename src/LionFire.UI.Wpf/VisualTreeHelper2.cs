using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LionFire.Avalon
{

    /// <summary>
    /// Retrieved from
    /// http://social.msdn.microsoft.com/Forums/en/wpf/thread/a945608d-d072-4d3c-8f33-601d6e818457
    /// on June 27, 2012
    /// </summary>
    public static class VisualTreeHelper2
    {
        /// <summary>
        /// Get visual tree children of a type
        /// </summary>
        /// <typeparam name="T">Visual tree children type</typeparam>
        /// <param name="visual">A DependencyObject reference</param>
        /// <param name="children">A collection of one visual tree children type</param>
        private static void GetVisualChildren<T>(DependencyObject current, Collection<T> children) where T : DependencyObject
        {
            if (current != null)
            {
                if (current.GetType() == typeof(T))
                {
                    children.Add((T)current);
                }

                for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(current); i++)
                {
                    GetVisualChildren<T>(System.Windows.Media.VisualTreeHelper.GetChild(current, i), children);
                }
            }
        }

        /// <summary>
        /// Get visual tree children of a type
        /// </summary>
        /// <typeparam name="T">Visaul tree children type</typeparam>
        /// <param name="visual">A DependencyObject reference</param>
        /// <returns>Returns a collection of one visual tree children type</returns>
        public static Collection<T> GetVisualChildren<T>(DependencyObject current) where T : DependencyObject
        {
            if (current == null)
            {
                return null;
            }

            Collection<T> children = new Collection<T>();

            GetVisualChildren<T>(current, children);

            return children;
        }

        /// <summary>
        /// Get the first visual child from a FrameworkElement Template
        /// </summary>
        /// <typeparam name="P">FrameworkElement type</typeparam>
        /// <typeparam name="T">FrameworkElement type</typeparam>
        /// <param name="p">A FrameworkElement typeof P</param>
        /// <returns>Returns a FrameworkElement visual child typeof T if found one; returns null otherwise</returns>
        public static T GetVisualChild<T, P>(P templatedParent)
            where T : FrameworkElement
            where P : FrameworkElement
        {
            Collection<T> children = VisualTreeHelper2.GetVisualChildren<T>(templatedParent);

            foreach (T child in children)
            {
                if (child.TemplatedParent == templatedParent)
                {
                    return child;
                }
            }

            return null;
        }
    }


}
