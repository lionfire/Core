#if FUTURE
using System;

namespace LionFire.FlexObjects
{
    public class FlexChangeListener
    {
        public event Action<IFlex> FlexImplementationChanging;
        public event Action<IFlex> FlexImplementationChanged;

        public event Action<IFlex, Type, string, object, object> FlexValueChangedForFromTo;
    }
}
#endif