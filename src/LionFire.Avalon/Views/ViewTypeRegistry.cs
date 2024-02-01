using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.Collections;

namespace LionFire.Avalon
{
    public class ViewTypeRegistration
    {
        public int Priority = 0;
        public Type Type;
    }

    public class ViewTypeRegistry
    {
        #region (Static) Instance Accessor

        public static ViewTypeRegistry Instance
        {
            get
            {
                return Singleton<ViewTypeRegistry>.Instance;
            }
        }

        #endregion

        public object Lock { get { return @lock; } } private object @lock = new object();

        public MultiValueDictionary<Type, ViewTypeRegistration> ViewTypesByObjectType
        {
            get { return detailViewsByDataContext; }
        } private MultiValueDictionary<Type, ViewTypeRegistration> detailViewsByDataContext = new MultiValueDictionary<Type, ViewTypeRegistration>();


        public void RegisterViewType(Type objectType, Type viewType, int priority = 50)
        {
            lock (Lock)
            {
                ViewTypesByObjectType.Add(objectType, new ViewTypeRegistration { Priority = priority, Type = viewType });
            }
        }
    }
}
