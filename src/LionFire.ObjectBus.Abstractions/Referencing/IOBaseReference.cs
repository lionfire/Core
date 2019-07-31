using LionFire.ObjectBus;
using LionFire.Ontology;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Referencing
{
    /// <summary>
    /// Typically, reference types will know their corresponding IOBus, and that will be able to provide IOBase. 
    /// Therefore, reference types typically know their IOBase, and should implement this interface.
    /// </summary>
    /// <remarks>
    /// REVIEW: Do I really want to be using IHas<IOBase>?
    /// </remarks>
    public interface IOBaseReference : IReference, IHas<IOBase>
    {
    
        //IOBase OBase { get; }
    }


    // LEGACY - Not sure I want to impose IReferenceWithLocation anymore
//        public interface IOBaseReference : IReferenceWithLocation
//        {
//#if LEGACY // TOMIGRATE
//        /// <summary>
//        /// Specialized implementations of IReference may be tied to a particular ObjectStoreProvider: Eg. FileReference may be tied to FilesystemObjectStoreProvider
//        /// </summary>
//        IOBaseProvider DefaultObjectStoreProvider { get; }
//#endif
//        }

}
