using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LionFire.Persistence;
using LionFire.Referencing;

namespace LionFire.Vos
{

    //#error When retrieve vh.Object, check option to register Object in ConditionalWeakTable
    //#error When retrieve vh.Object, set ReadMount


    

    public interface IVobHandle<T> : IReadWriteHandle<T> {
        
        //event PropertyChangedEventHandler ObjectPropertyChanged;

        IVob Vob { get; }

        /// <summary>
        /// Vob path
        /// </summary>
        string Path { get; }

        new T Value { get; set; }

        ///// <summary>
        ///// Mount that was used to load (FUTURE: or save) Object, or that should be used.
        ///// FUTURE TODO: Mount that should be used to load the vob.  Make immutable.
        ///// </summary>
        //Mount Mount { get; set; }

        ///// <summary>
        ///// FUTURE TODO: Store that should be used to load the vob.  Make immutable.
        ///// </summary>
        //string Store { get; set; }

        ///// <summary>
        ///// FUTURE TODO: Package that should be used to load the vob.  Make immutable.
        ///// </summary>
        //string Package { get; set; }
        ////VobHandle<SisterType> AsVobHandleType<SisterType>(bool allowCast = true)
        ////where SisterType : class,new();

        //Type Type { get; }

        //bool IsObjectReferenceFrozen { get; set; }
        //bool AutoLoad { get; set; }
        //VosRetrieveInfo VosRetrieveInfo { get; }

        //void Unload();

        //void MergeWith(IVobHandle existing);
        //void MergeInto(IVobHandle mainHandle);

        //IVobHandle GetSibling(string name); // Can this be moved to IHandle?
        //bool CanWriteToReadSource();

        void OnRenamed(IVobHandle<T> newHandle);
    }

    public interface IVobHandle : IVobHandle<object>  // RENAME to VH?
    {
    }

}
