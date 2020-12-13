using LionFire.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LionFire.UI
{
    public class UICollection : UICollection<IUIKeyed>
    {
    }

    public interface IReadOnlyDictionary3<TKey, out TValue>
    {

    }

    public interface IReadOnlyUICollection<TChild> : IReadOnlyDictionary3<string, TChild>
        where TChild : IUIKeyed
    {

    }

    public class UICollection<TChild> : UIKeyed, IUICollection, IReadOnlyUICollection<IUIKeyed>
        where TChild : IUIKeyed
    {
        #region Children

        protected ObservableDictionary<string, TChild> Children { get; set; }
        System.Collections.Generic.IReadOnlyDictionary<string, IUIKeyed> IHierarchyOfKeyed<IUIKeyed>.Children => CovariantChildren;
        //protected IReadOnlyDictionary<string, TChild> ReadOnlyChildren => Children;

        public ReadOnlyDictionaryWrapper<string, TChild, IUIKeyed> CovariantChildren { get; private set; }

        #endregion

        private void CreateChildren()
        {
            Children = new ObservableDictionary<string, TChild>();
            CovariantChildren = new ReadOnlyDictionaryWrapper<string, TChild, IUIKeyed>(Children);
        }

        void IUICollection.Add(IUIKeyed node) => Add((TChild)node);
        public void Add(TChild node)
        {
            if (Children?.ContainsKey(node.Key) == true) throw new DuplicateNotAllowedException(node.Key);
            if (Children == null) { CreateChildren(); }

            if (node is INotifyDisposing nd)
            {
                nd.Disposing += OnChildDisposing;
            }
            Children.Add(node.Key, node);
        }

        public bool Remove(string key) => Children.Remove(key);
        public void RemoveAll() => Children.Clear();

        #region Event Handling

        protected virtual void OnChildDisposing(object obj)
        {
            var presenter = (IUIKeyed)obj;
            Children.Remove(presenter.Key);
        }

        #endregion

#if ICloseable
        #region ICloseable

        public Task Close(bool force = false)
        {
        #region Closing event (with cancel)

            var cea = new CancelableEventArgs();
            Closing?.InvokeCancelable(cea);
            if (cea.CancelRequested) { return; }

        #endregion

            var presenters = Children.Values.ToArray();
            Children.Clear();
            foreach (IPresenter presenter in presenters)
            {
                //if (presenter == MainPresenter) continue; // Close MainPresenter last
                try
                {
                    presenter.Close();
                }
                catch (Exception ex)
                {
                    l.Error("Exception when closing child presenter", ex);
                }
            }
            //if (MainPresenter != null)
            //{
            //    try
            //    {
            //        MainPresenter.Close();
            //    }
            //    catch (Exception ex)
            //    {
            //        // TOALERT
            //        l.Error("Exception when closing main presenter", ex);
            //    }
            //}

            OnClosed();
            return Task.CompletedTask;
        }

        #region Closing

        protected virtual void OnClosed()
        {
            // Idea: If this is the main shell (probably is), also try to CloseMainWindow:
            //try
            //{
            //    System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
            //}
            //catch // EMPTYCATCH
            //{
            //}
            //    WindowsSettings.Save(); // TODO - make autosaved

            Closed?.Invoke(this);
        }




        protected virtual bool RaiseCanceling()
        {
            var cea = new CancelableEventArgs();
            Closing?.InvokeCancelable(cea);
            return !cea.CancelRequested;
        }

        protected virtual void OnSelfInitiatedClose(bool cancelable = true)
        {
            var canceled = RaiseCanceling();
            if (cancelable && canceled) return;

            OnClosed();
        }

        #endregion

        #endregion
#endif
    }

}
