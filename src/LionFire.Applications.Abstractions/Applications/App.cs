using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using LionFire.Dependencies;
using LionFire.Applications.Hosting;

namespace LionFire.Applications
{
    public static class App
    {
        public static T GetComponent<T>()
        {
            return DependencyContext.Current.GetService<IAppHost>().Children.OfType<T>().SingleOrDefault();
        }
        public static IEnumerable<T> GetComponents<T>()
        {
            return DependencyContext.Current.GetService<IAppHost>().Children.OfType<T>();
        }
        public static T GetService<T>()
        {
            return DependencyContext.Current.GetService<T>();
        }
        public static IEnumerable<T> GetServices<T>()
        {
            return DependencyContext.Current.GetServices<T>();
        }
        public static T Get<T>()
            where T : class
        {
            return GetService<T>() ?? GetComponent<T>();
        }
        public static IEnumerable<T> GetAll<T>()
        {
            return GetServices<T>().Concat(GetComponents<T>());
        }
    }
}
