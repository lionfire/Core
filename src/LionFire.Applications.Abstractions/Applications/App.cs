using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using LionFire.DependencyInjection;
using LionFire.Applications.Hosting;

namespace LionFire.Applications
{
    public static class App
    {
        public static T GetComponent<T>()
        {
            return InjectionContext.Current.GetService<IAppHost>().OfType<T>().SingleOrDefault();
        }
        public static IEnumerable<T> GetComponents<T>()
        {
            return InjectionContext.Current.GetService<IAppHost>().OfType<T>();
        }
        public static T GetService<T>()
        {
            return InjectionContext.Current.GetService<T>();
        }
        public static IEnumerable<T> GetServices<T>()
        {
            return InjectionContext.Current.GetServices<T>();
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
