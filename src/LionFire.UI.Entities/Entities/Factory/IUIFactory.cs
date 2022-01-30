using LionFire.Dependencies;
using LionFire.ExtensionMethods;
using LionFire.UI.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.UI.Entities
{

    public interface IUIFactory : IFactory<UIInstantiation, IUIObject>
    {
    }

    public static class IUIFactoryExtensions
    {
        public static IUIObject Show(this IUIParent parent, UIInstantiation instantiation, IUIFactory factory = null)
            => parent.Child = (factory ?? DependencyContext.Current.GetService<IUIFactory>()).Create(instantiation);
        
        public static IUIObject Show(this IUICollection parent, UIInstantiation instantiation, IUIFactory factory = null)
        {
            var obj = (IUIKeyed)(factory ?? DependencyContext.Current.GetService<IUIFactory>()).Create(instantiation);
            parent.Add(obj);
            return obj;
        }

        public static IEnumerable<IUIObject> CreateMany(this IUIFactory factory, IEnumerable<UIInstantiation> instantiations)
            => new List<IUIObject>(instantiations.Select(i => factory.Create(i)));
    }
}
