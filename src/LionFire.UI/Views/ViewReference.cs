using LionFire.Resolvables;
using LionFire.Resolves;
using System;

namespace LionFire.UI
{

    // REVIEW - what should this class be???

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Consider preferring ViewModel if you plan to make your application cross-platform.
    /// </remarks>
    public class ViewReference
    {
        public string EffectiveName => Name ?? ViewType?.Name ?? ViewModelType?.Name;

        public string Name { get; set; }

        /// <summary>
        /// See LionFire.Shell.Conventions.ShellLayer
        /// </summary>
        public int Layer { get; set; }

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

        public string Url { get; set; }

        public static implicit operator ViewReference(string url) => new ViewReference { Url = url };

    }
}
