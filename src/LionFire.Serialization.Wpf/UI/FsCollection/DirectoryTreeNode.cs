using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace LionFire.Serialization.UI
{
    public class DirectoryTreeNode
    {
        public DirectoryTreeNode() { }
        public DirectoryTreeNode(string path) { this.Path = path; }

        public string Path { get; set; }
        public string Name { get => System.IO.Path.GetFileName(Path); }

        public IEnumerable<DirectoryTreeNode> Children
        {
            get
            {
                if (children == null)
                {
                    Refresh();
                }
                return children;
            }
        }
        private IEnumerable<DirectoryTreeNode> children;

        public void Refresh()
        {
            if (Path == null || !Directory.Exists(Path))
            {
                children = Enumerable.Empty<DirectoryTreeNode>();
            }
            else
            {
                children = Directory.GetDirectories(Path).Select(d => new DirectoryTreeNode
                {
                    Path = d
                });
            }
        }
    }
}
