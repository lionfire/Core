#if NOESIS
using Noesis;
#else
#define Windowing
using LionFire.UI.Wpf;
#endif
using System;
using System.Linq;
using System.Reflection;
using LionFire.Shell.Wpf;
using LionFire.Dependencies;
using System.Windows.Controls;
using System.Threading.Tasks;
using LionFire.Structures;
using System.Windows;
using LionFire.UI;
using LionFire.Ontology;
using LionFire.Referencing;
using System.Windows.Threading;

namespace LionFire.Shell
{

    [ViewType(typeof(WpfTabsView))]
    public class WpfTabs : WpfPresenter
    {
        Type ViewType { get; set; }

        public WpfTabs() : base()
        {

        }
    }

    //public class LayersWpfPresenter : WpfPresenter
    //{
    //}

    public class PresenterBase
    {
        #region Ontology

        public IPresenter Parent { get; }


        public string Path => LionPath.Combine(Parent.Path, Key);

        #endregion

        #region Parameters

        #region Key

        [SetOnce]
        public string Key
        {
            get => key;
            set
            {
                if (key == value) return;
                if (key != default) throw new AlreadySetException();
                key = value;
            }
        }
        private string key;

        #endregion

        public bool PreventAutoClose { get; set; }

        #endregion
    }


    public class WpfPresenter : PresenterBase, IPresenter, IKeyable, IParented
    {
        #region Dependencies

        public WpfWindowManager WpfWindowManager { get; }

        #endregion

        #region Construction

        public WpfPresenter(IPresenter parent)
        {
        }

        #endregion

        #region Children

        #region ParentView

        [SetOnce]
        public UIElement ParentView
        {
            get => parentView;
            set
            {
                if (parentView == value) return;
                if (parentView != default) throw new AlreadySetException();
                parentView = value;
            }
        }
        private UIElement parentView;

        #endregion

        #region PresenterView

        [SetOnce]
        public UIElement PresenterView
        {
            get => presenterView;
            set
            {
                if (presenterView == value) return;
                if (presenterView != default) throw new AlreadySetException();
                presenterView = value;
            }
        }
        private UIElement presenterView;

        #endregion



        #endregion

        #region State

        #region IsActive

        public bool IsActive
        {
            get => isActive;
            set
            {
                if (isActive == value) return;
                isActive = value;
                OnPropertyChanged(nameof(IsActive));
            }
        }
        private bool isActive;

        #endregion

        #endregion

        [UIThread]
        protected UIElement GetParentView()
        {
            if (PresenterView != null) { return PresenterView; }

            if (ParentView == null)
            {
                Window w;
                ParentView = w = WpfWindowManager.GetOrCreateWindow(this.Key);
            }
        }

        #region Show

        public async Task Show(ViewInstantiation instantiation)
        {

            var existing = this.QueryPath(instantiation.Parameters.Path);

            if(existing != null)
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    existing.Visible = true;

                }).ConfigureAwait(false);
                return;
            }
            
                IWpfViewLocator vl;
            
            
            ViewNameResolver.GetViewName(instantiation.Template.Type)
            string viewName = instantiation.Parameters.ViewName ?? instantiation.EffectiveName ?? ViewNameConventions.DefaultViewName;

            if (instantiation.ViewAction != null)
            {
                instantiation.ViewAction();
            }
            else if (instantiation.View != null)
            {
                throw new NotImplementedException(nameof(instantiation.View));
            }
            else if (instantiation.ViewType != null)
            {
                ShowControl(instantiation.ViewType, viewName, instantiation.DataContext);
            }
            else if (instantiation.ViewModelType != null)
            {
                var viewType = DependencyContext.Current.GetRequiredService<IViewLocator>().LocateTypeForModelType(instantiation.ViewModelType, null, null);
                if (viewType == null) { throw new Exception("Could not locate type for model type:" + instantiation.ViewModelType.FullName); }
                ShowControl(viewType, viewName, instantiation.DataContext);
            }
            else
            {
                throw new Exception("No default view or view action");
            }
        }

        #endregion

        #region Views

        public object ShowControl(Type type, string tabName = null, object dataContext = null)
        {

            ShowControlGenericMethodInfo ??= this.GetType().GetMethods().Where(mi => mi.Name == "ShowControl" && mi.ContainsGenericParameters).First();

            MethodInfo mi = ShowControlGenericMethodInfo.MakeGenericMethod(type ?? throw new ArgumentNullException(nameof(type)));
            return mi.Invoke(this, new object[] { tabName });
        }
        private static MethodInfo ShowControlGenericMethodInfo;

        public T ShowControl<T>(string tabName = null) where T : class => MainPresenter.PushTab<T>(tabName);
        public object ShowControl(Type type, string tabName = null) => MainPresenter.PushTab(type, tabName);
        public Task Show(ViewReference context, string viewName = null, ViewParameters options = null) => throw new NotImplementedException();
        public bool Close(string viewName = null) => throw new NotImplementedException();

        //public T GetControl<T>(string tabName = null OLD
        //    //, T frameworkElement = null
        //    , bool showControl = false) where T : FrameworkElement
        //{
        //    var result = MainPresenter.GetControl<T>(tabName
        //        //, frameworkElement
        //        , showControl);
        //}

        //public void AddControl<T>(T controlInstance, string tabName = null) where T : FrameworkElement // UNUSED 
        //{
        //    mainPresenter.AddControl(controlInstance, tabName);
        //}

        #endregion

    }


}
