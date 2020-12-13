using System.Collections.Generic;

namespace LionFire.UI.Entities
{
    public interface IViewEntity : IUIObject
    {
        /// <summary>
        /// Object type must return true when passed to IUIPlatform.IsViewType()
        /// </summary>
        object View { get; set; }
    }
    public static class IViewEntityExtensions
    {
        public static bool CanAdd(this IViewEntity viewEntity) => viewEntity.View != null;
    }

    public interface IViewsEntity : IUIObject
    {
        IEnumerable<object> Views { get; }
        void Add(object view);
        bool CanAdd { get; }
    }
}

