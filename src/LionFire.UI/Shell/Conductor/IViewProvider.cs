using System;

namespace LionFire.UI
{
    public interface IViewProvider
    {
        Type LocateTypeForModelType(Type type, object target, object context);
        object LocateForModelType(Type type, object target, object context);
        object LocateForModel(object model, object target, object context);
    }
}
