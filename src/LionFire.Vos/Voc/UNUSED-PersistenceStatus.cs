using System;

namespace LionFire.Vos
{
    [Flags]
    public enum PersistenceStatus 
    {
        None=0,
        UnsavedChanges = 1 << 0,
        
        NotLoaded = 1 << 1,

        SourceChanged = 1<<9,

        /// <summary>
        /// SourceChanged & UnsavedChanges and !AutoMerge or AutoMergeFail
        /// </summary>
        Conflict = 1<<10,
        
        AutoMerge = 1 << 11,
        AutoMergeFail = 1 << 12,

        Monitoring = 1 << 15,
    }

}
