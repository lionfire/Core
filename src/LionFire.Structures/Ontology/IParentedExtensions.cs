using LionFire.Ontology;
using System;


namespace LionFire
{
    public static class ParentedExtensions
    {
        public static object TopParent(this IParented obj)
        {
            object topParent = obj;
            while (topParent != null)
            {
                var parented = topParent as IParented;
                if (parented != null && parented.Parent != null)
                {
                    if (topParent == parented.Parent)
                    {
                        // Recursion detected
                        break;
                    }
                    topParent = parented.Parent;
                }
                else break;
            }
            return topParent;
        }

        public static T ParentOfType<T>(this IParented obj)
            where T : class
        {
            while (obj != null)
            {
                var result = obj as T;
                if (result != null) return result;
                var parented = obj as IParented;
                if (parented == null) return null;
                obj = parented.Parent as IParented;
            }
            return null;
        }

        public static object ParentOfType(this IParented obj, Type T)
        {
            while (obj != null)
            {
                if (T.IsAssignableFrom(obj.GetType())) { return obj; }

                var parented = obj as IParented;
                if (parented == null) return null;
                obj = parented.Parent as IParented;
            }
            return null;
        }

        //public static T FindParent<T>(this IParented parent)  OLD
        //    where T : class
        //{
        //    if (parent.Parent == null)
        //    {
        //        return null;
        //    }
        //    if (parent.Parent as T != null)
        //    {
        //        return (T)parent.Parent;
        //    }
        //    IParented parentAsParented = parent.Parent as IParented;
        //    if (parentAsParented == null) return null;
        //    return parentAsParented.FindParent<T>();
        //}
    }
}

namespace LionFire.Ontology
{

    //    public interface IParented
    //    {
    //        object Parent { get; set; }
    //    }

    //#if !AOT
    //    public interface IParented<ParentType> : IParented
    //    {
    //        new ParentType Parent { get; set; }
    //    }
    //#endif

    public interface IHasParent // RENAME these? Come up with a convention for RO and RW?
    {
        object Parent { get; }
    }
    public interface IHasParent<ParentType> : IHasParent
    {
        new ParentType Parent { get; }
    }

}
