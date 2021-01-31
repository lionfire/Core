using LionFire.Ontology;
using System.Collections.Generic;

namespace LionFire.UI.Workspaces
{
    public class WorkspaceTab : IWorkspaceTab
    {
        public IEnumerable<object> Navs { get { if (Tab != null) yield return Tab; else yield break; } }

        public IEnumerable<object> ContentPanes { get { if (Contents != null) yield return Contents; else yield break; } }

        public IWorkspaceTab Tab { get; set; }
        public object Contents { get; set; }
        public IWorkspaceItem WorkspaceItem { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public IUIObject ContentPane { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public string Title => throw new System.NotImplementedException();

        public string Tooltip { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public bool IsFocused { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public bool IsVisible { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public bool IsTabPaneFocused { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public bool IsTabPaneVisible { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public IUIObject Parent { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        IUIObject IParented<IUIObject>.Parent => throw new System.NotImplementedException();

        public void OnClosing() => throw new System.NotImplementedException();
    }
}
