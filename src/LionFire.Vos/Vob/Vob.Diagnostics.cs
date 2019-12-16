using System;
using System.Collections.Generic;
using System.Text;
using LionFire.Referencing;
using LionFire.Extensions.ObjectBus;

namespace LionFire.Vos
{
    public partial class Vob
    {
        #region (Diagnostics) Dump

        #region Read

//        public string DumpNonVirtualReadLocations
//        {
//            get
//            {
//                return GetNonVirtualReadLocations
//#if NET35
//.Cast<object>()
//#endif
//                .ToStringList();
//            }
//        }
//        public IEnumerable<IReference> GetNonVirtualReadLocations
//        {
//            get
//            {
//                throw new NotImplementedException();
//                foreach (var handle in ReadHandles)
//                {
//                    var references = handle.ToNonVirtualReferences();
//                    foreach (var reference in references)
//                    {
//                        if (reference != null)
//                        {
//                            yield return reference;
//                        }
//                    }
//                }
//            }
//        }

        #endregion

        #region Write

//        public string DumpNonVirtualWriteLocations
//        {
//            get
//            {
//                return GetNonVirtualWriteLocations
//#if NET35
//                .Cast<object>()
//#endif
//                .ToStringList();
//            }
//        }
//        public IEnumerable<IReference> GetNonVirtualWriteLocations
//        {
//            get
//            {
//                throw new NotImplementedException();
//                //foreach (var handle in WriteHandles)
//                //{
//                //    var references = handle.ToNonVirtualReferences();
//                //    foreach (var reference in references)
//                //    {
//                //        if (reference != null)
//                //        {
//                //            yield return reference;
//                //        }
//                //    }
//                //}
//            }
//        }

        #endregion

        #endregion

    }
}
