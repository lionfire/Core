using System;
using LionFire.Persistence;

namespace LionFire.Vos
{
    public class MountHandleObject : INotifyOnSaving
    {
        public void OnSaving(object persistenceContext = null) => throw new NotSupportedException("Do not save this object");
    }
}
