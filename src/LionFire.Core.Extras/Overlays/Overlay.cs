
using System;
using System.Text;
#if !NET35
#endif

namespace LionFire.Overlays
{
    public interface IOverlay
    {
        void SetPropertyValue(string propertyName, object value, object[] index);

        object GetPropertyValue(string propertyName, object[] index);
    }

    public interface IOverlay<T> : INotifyDisposing<IOverlay<T>>, IDisposable, IOverlay
    {

        // IDEA: Use [NullValue(int.MinValue)] to avoid using int?

        /// <summary>
        /// Returns the flattened version of the object stack.
        /// Every time 
        /// </summary>
        T Proxy { get; }
        void Add(T instance, string label = null);
        void Insert(T instance, string label = null);
        void Set(int priority, T overlayObject, string label = null);

        OverlayParameters Parameters { get; }

        //T EffectiveInstance(PropertyInfo pi);

    }


}
