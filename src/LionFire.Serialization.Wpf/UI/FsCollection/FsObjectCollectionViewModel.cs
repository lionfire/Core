using System.IO;
using Caliburn.Micro;
using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LionFire.UI;
using System.Diagnostics;

namespace LionFire.Serialization.UI
{
    public class FsObjectCollectionViewModel : Screen
    {
        public static FsObjectCollectionViewModel Instance => Singleton<FsObjectCollectionViewModel>.Instance;

        public FsObjectCollectionViewModel()
        {
            Root = new DirectoryTreeNode(LionFireEnvironment.AppProgramDataDir);
            objects.AutoResolveObjects = true;
            objects.IsObjectsEnabled = true;
            objects.Handles.CollectionChanged += Handles_CollectionChanged;
        }

        private void Handles_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Debug.WriteLine(e.NewItems);
        }


        #region Root

        public DirectoryTreeNode Root
        {
            get { return root; }
            set
            {
                if (root == value) return;
                root = value;

                // REVIEW: Case sensitive comparison on Windows?
                if (root != null && SelectedDirectory == null || !SelectedDirectory.StartsWith(root.Path))
                {
                    SelectedDirectoryNode = root;
                }
                NotifyOfPropertyChange(() => Root);
            }
        }
        private DirectoryTreeNode root;

        #endregion


        #region SelectedDirectoryNode

        public DirectoryTreeNode SelectedDirectoryNode
        {
            get { return selectedDirectoryNode; }
            set
            {
                if (selectedDirectoryNode == value) return;
                selectedDirectoryNode = value;
                objects.RootPath = selectedDirectoryNode?.Path;
                NotifyOfPropertyChange(() => SelectedDirectoryNode);
                NotifyOfPropertyChange(() => SelectedDirectory);
            }
        }
        private DirectoryTreeNode selectedDirectoryNode;

        #endregion



        #region SelectedDirectory

        public string SelectedDirectory
        {
            get { return SelectedDirectoryNode?.Path; }
        }

        #endregion


        // FUTURE: Genericize this?
        //public Type ObjectCollectionGenericType { get; set; } = typeof(FsObjectCollection<>);
        //public Type ObjectType { get; set; } = typeof(object);
        //public Type ObjectCollectionType { }

        public FsObjectCollection<object> ObjectCollection
        {
            get => objects;
        }
        FsObjectCollection<object> objects = new FsObjectCollection<object>();


        #region SelectedHandle

        public R<object> SelectedHandle
        {
            get { return selectedHandle; }
            set
            {
                if (selectedHandle == value) return;
                selectedHandle = value;
                SelectedObject = selectedHandle?.Object;
                NotifyOfPropertyChange(() => SelectedHandle);
            }
        }
        private R<object> selectedHandle;

        #endregion

        #region SelectedObject

        public object SelectedObject
        {
            get { return selectedObject; }
            set
            {
                if (selectedObject == value) return;
                selectedObject = value;
                NotifyOfPropertyChange(() => SelectedObject);
                SelectedObjectViewModel = ViewModelProvider?.ProvideViewModelFor(selectedObject) ?? selectedObject;
            }
        }
        private object selectedObject;

        #endregion


        public IViewModelProvider ViewModelProvider { get; set; } = new LionFire.UI.Wpf.WpfToolkit.PropertyGridViewModelProvider();


        #region SelectedObjectViewModel

        public object SelectedObjectViewModel
        {
            get { return selectedObjectViewModel; }
            set
            {
                if (selectedObjectViewModel == value) return;
                selectedObjectViewModel = value;
                NotifyOfPropertyChange(() => SelectedObjectViewModel);
            }
        }
        private object selectedObjectViewModel;

        #endregion

    }
}

