using LionFire.Resolvables;
using LionFire.Resolves;
using System;

namespace LionFire.UI
{
    public class UIReference
    {
        public string EffectiveName => Name ?? ViewType?.Name ?? ViewModelType?.Name;

        public string Name { get; set; }

        /// <summary>
        /// (TODO) Null: auto-generate presenter names, starting with MainPresenter
        /// </summary>
        public string PresenterName { get; set; }

        public string TabName { get; set; }

        /// <summary>
        /// Must be a UI type that the ILionFireShell is capable of showing, or else a IHostedService
        /// </summary>
        public Type ViewType { get; set; }

        /// <summary>
        /// Must be a UI object that the ILionFireShell is capable of showing, or else a IHostedService
        /// </summary>
        public object View { get; set; }

        public Action ViewAction { get; set; }

        public Type ViewModelType { get; set; }

        public object ViewModel { get; set; }

        public IResolves<object> DataContextHandle { get; set; }
        public object DataContext
        {
            get => dataContext ??= DataContextHandle?.Resolve();
            set => dataContext = value;
        }
        private object dataContext;

    }
}
