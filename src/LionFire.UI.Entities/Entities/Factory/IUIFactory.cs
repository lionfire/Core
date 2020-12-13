using LionFire.Alerting;
using LionFire.Collections;
using LionFire.Dependencies;
using LionFire.ExtensionMethods;
using LionFire.Referencing;
using LionFire.UI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.UI.Entities
{
    // OLD
    ///// <summary>
    ///// Navigate to the context
    ///// </summary>
    ///// <param name="instantiation">Examples: A URI string for web browsers.  For WPF, it could be a custom UserControl.  If MVVM support is available, it can be a custom ViewModel type</param>
    ///// <param name="viewName"></param>
    ///// <param name="options"></param>
    //IUIObject Show(IUIParent parent, UIInstantiation instantiation);

    public interface IFactory<in TInput, out TOutput>
    {
        TOutput Create(TInput instantiation);
    }

    public interface IUIFactory : IFactory<UIInstantiation, IUIObject>
    {
        //IUIObject Create(UIInstantiation instantiation);
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

    public class UIFactory : IUIFactory
    {
        #region Dependencies

        public IServiceProvider ServiceProvider { get; }
        public IUIPlatform Platform { get; }
        public IUIRoot Root { get; }

        #endregion

        #region Construction

        public UIFactory(IServiceProvider serviceProvider, IUIPlatform uIPlatform, IUIRoot root)
        {
            ServiceProvider = serviceProvider;
            Platform = uIPlatform;
            Root = root;
        }

        #endregion

        #region Add View

        public bool CanAddView(IUIKeyed parent)
        {
            if (parent is IViewEntity ve && ve.View == null) { return true; }
            if (parent is IViewsEntity vse && vse.CanAdd) { return true; }
            return false;
        }

        public void AddView(IUIKeyed parent, object view)
        {
            if (view == null) throw new ArgumentNullException();
            if (parent is IViewEntity ve && ve.View == null)
            {
                if (!ve.CanAdd()) { throw new InvalidOperationException("IViewEntity.View != null.  Cannot add view."); }
                ve.View = view;
                return;
            }
            else if (parent is IViewsEntity vse)
            {
                if (!vse.CanAdd) { throw new InvalidOperationException("IViewsEntity.CanAdd == false.  Cannot add view."); }
                vse.Add(view);
                return;
            }
            else
            {
                throw new NotSupportedException($"Do not know how to add a view to parent of type {parent.GetType().FullName}");
            }
        }

        #endregion

        #region Add Entity

        public bool CanAddEntity(IUIKeyed parent)
        {
            if (parent is IUICollection c)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void AddEntity(IUIKeyed parent, IUIKeyed child)
        {
            if (child == null) throw new ArgumentNullException();
            if (parent is IUICollection c)
            {
                c.Add(child);
                return;
            }
            else
            {
                throw new NotSupportedException($"Do not know how to add {nameof(IUIKeyed)} to parent of type {parent.GetType().FullName}");
            }
        }

        #endregion

        private IUIObject Instantiate_ViewType(UIInstantiation instantiation)
        {
            var view = Activator.CreateInstance(instantiation.Template.ViewType);
            throw new NotImplementedException(); // NEXT
        }

        private IUIObject Instantiate_UIEntity(UIInstantiation instantiation)
        {
            var entity = (IUIKeyed) Activator.CreateInstance(instantiation.Template.EntityType);
            throw new NotImplementedException(); // NEXT
            return entity;
        }
        private IUIObject Instantiate_ViewModel(UIInstantiation instantiation)
        {
            throw new NotImplementedException();
        }
        public IUIObject Instantiate(UIInstantiation instantiation)
        {
            var type = instantiation.Template?.ViewType;
            if (type != null)
            {
                if(instantiation.Template.EntityType != null)
                {
                    return Instantiate_UIEntity(instantiation);
                }
                else if (Platform.IsViewType(type))
                {
                    return Instantiate_ViewType(instantiation);
                }
                else if(instantiation.Template.ViewModel != null)
                {
                    return Instantiate_ViewModel(instantiation);
                }
            }
            throw new ArgumentException($"{nameof(instantiation)} is missing information for creation, or the information is not supported.");
        }

        public IUIObject Create(UIInstantiation instantiation)
        {

            var entity = Instantiate(instantiation);

            var path = instantiation?.Parameters.Path;

            var existing = Root.QuerySubPath(path);
            if (existing != null) throw new AlreadyException("UI already exists at path: " + path);

            var parentPath = LionPath.GetParent(path, nullIfBeyondRoot: true);

            var parent = parentPath == null ? Root : Root.QuerySubPath(parentPath);
            if (parent == null) { throw new ArgumentException("Parent not found for path: " + path); }

            parent.Add(entity);

            return entity;
        }

        private T SetChildForNode<T>(IUIParent parent)
            where T : IUIObject
        {
            var child = (T)ServiceProvider.GetService(typeof(T));
            parent.Child = child;
            return child;
        }

        private T ShowForNode<T>(IUIObject node, string pathOrName)
            where T : IUIObject
        {
            throw new NotImplementedException();
        }

        private IUIObject ShowForNode(IUIObject node, UIInstantiation instantiation)
        {
            throw new NotImplementedException();

        }

        public void Show(IEnumerable<UIInstantiation> instantiations, IUICollection root = null)
        {
            root ??= Root;

            foreach (var instantiation in instantiations)
            {
                IUIObject parent = root.QuerySubPath(instantiation.Parameters.Path);

                try
                {
                    if (parent == null)
                    {
                        throw new ArgumentException("Could not find UI path:" + instantiation.Parameters.Path);
                    }
                }
                catch (Exception ex)
                {
                    Alerter.Alert("Failed to show interface", ex);
                    continue;
                }

                ShowForNode(parent, instantiation);
            }

        }

        public T Show<T>(IUIObject parent = null) where T : IUIObject
        {
            parent ??= Root;

            //if (parent is IUIFactoryNode fn) { return fn.Show<T>(); }

            throw new NotImplementedException(); // TODO: Support lightweight view-injection in UINodes that do not implement IUIFactoryNode.


            //var view = ServiceProvider.GetRequiredService<T>();
            ////if(view == null) { throw new ArgumentException($"No concrete "); }


            ////if(parent is IPresenter parentPresenter)
            ////{
            ////    parentPresenter.Show(view);
            ////}


            //if (Platform.IsViewType(typeof(T)))
            //{
            //    if (parent is IPresenter parentPresenter)
            //    {
            //        parentPresenter.View = view;
            //        return;
            //    }
            //    else if ()
            //    {

            //    }
            //}
            //else
            //{

            //}
        }

        public IUIObject Show(UIInstantiation context, UIParameters options = null) => throw new NotImplementedException();

    }
}

#region OLD - startup task
//Func<Task> startup; // StartAsync waits for ctor tasks to finish
//startup = () => Task.Run(async () =>
//{
//    WindowSettings = await windowSettings.GetNonDefaultValue().ConfigureAwait(false);
//    // ENH Make this a Participant that contributes to CanStartShell?
//});

//await startup().ConfigureAwait(false);
//startup = null;
#endregion
