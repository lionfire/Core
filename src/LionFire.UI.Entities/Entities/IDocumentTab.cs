using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.UI.Entities
{
    public interface ISelectorTab : INotifyClosing
    {
    }
    public interface IDocumentTab : INotifyClosing
    {
    }

    public interface IHasDocumentTabOptions
    {
        DocumentTabOptions DocumentTabOptions { get; }
    }

    public class DocumentTabOptions
    {
        public static DocumentTabOptions Default { get { return Singleton<DocumentTabOptions>.Instance; } }

        /// <summary>
        /// If true, when ShellContentPresenter closes an IDocumentTab, it will invoke ShellContentPresenter.Save(), which
        /// attempts to save the DataContext of the IDocumentTab.
        /// </summary>
        public bool SaveOnClose = true;
        //public bool WarnOnExitWithoutSaved = true;
    }
}
