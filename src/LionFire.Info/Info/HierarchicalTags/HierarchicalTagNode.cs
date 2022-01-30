using LionFire.FlexObjects;
using LionFire.Referencing;
using System.Collections.Concurrent;

namespace LionFire.Info
{
    //public interface INamedNode
    //{
    //    INamedNode? Parent { get; set; }
    //}

    public class HierarchicalNamedNodeBase<TConcrete>
        where TConcrete : HierarchicalNamedNodeBase<TConcrete>
    {

        public TConcrete? Parent { get; protected set; } = default;
        public ConcurrentDictionary<string, TConcrete>? Children { get; protected set; } = default;

        public int Depth
        {
            get
            {
                if (depth == int.MinValue)
                {
                    depth = Parent == null ? 0 : Parent.Depth + 1;
                }
                return depth;
            }
        }
        private int depth = int.MinValue;
    }

    public class HierarchicalNamedNode<TConcrete, TValue> : HierarchicalNamedNodeBase<TConcrete>
        where TConcrete : HierarchicalNamedNode<TConcrete, TValue>
    {
        public string? Key
        {
            get
            {
                if (key == null)
                {
                    if (Parent == null)
                    {
                        key = Name;
                    }
                    else
                    {
                        key = LionPath.Combine(Parent.Key, Name);
                    }
                }
                return key;
            }
        }
        private string? key = null;

        public string? Name { get; set; }

        public HierarchicalNamedNode()
        {
            Name = null;
            Value = default;
        }

        public HierarchicalNamedNode(TConcrete? parent, string name)
        {
            Parent = parent;
            Name = name;

            if (typeof(TValue) == typeof(string))
            {
                Value = (TValue)(object)name;
            }
            Value = default;
        }

        public TValue? Value { get; set; }

        public void Add(TConcrete node)
        {
            node.Parent = (TConcrete)this;
            if (Children == null) { Children = new(); }

            if (!Children.TryAdd(node.Name ?? throw new ArgumentNullException(nameof(node.Name)), node))
            {
                throw new AlreadySetException();
            }
        }

        public void Remove(TConcrete node)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return Name ?? "(null)";
        }
    }

    public class TagNode : HierarchicalNamedNode<TagNode, string>, IFlex
    {
        public TagNode() { }
        public TagNode(TagNode? parent, string name) : base(parent, name) { }

        object? IFlex.FlexData { get; set; } = null;
    }

}