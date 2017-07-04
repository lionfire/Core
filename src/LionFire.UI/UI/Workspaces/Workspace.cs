using LionFire.Instantiating;
using LionFire.Execution.Executables;
using System.Collections.ObjectModel;
using LionFire.Composables;
using System.Collections.Generic;

namespace LionFire.UI.Workspaces
{
    // MOVE? Split Caliburn.Micro app stuff into its own DLL?
    public class Workspace<TTemplate, TChild> : ExecutableBase, ITemplateInstance<TTemplate>, IComposition, IWorkspace
        where TTemplate : class, ITemplate
    {
        IEnumerable<object> IComposition.Children => Children;

        public ObservableCollection<object> Children { get; private set; } = new ObservableCollection<object>();


        public TTemplate Template
        {
            get { return template; }
            set
            {
                if (template == value) return;
                var oldValue = template;
                template = value;
                OnTemplateChanged(oldValue, template);
            }
        }
        private TTemplate template;


        protected virtual void OnTemplateChanged(TTemplate oldValue, TTemplate newValue) { }
    }

    // TODO: Children Executable Visitor: attach to object, then on onstarting/onstopping, crawl thru the hierarchy
    // TODO: Init call a global executablemanager to say that an executable is intializing, to give it a chance to add state listeners

}
