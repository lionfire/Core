// FUTURE?  For now, rely on OBusHandle.  Figure out alternative mechanism framework?? Or is that for someone else?  Rename OBus to something more generic/boring?
//using LionFire.Referencing.Persistence;
//using System;
//using System.Threading.Tasks;

//namespace LionFire.Referencing
//{

//    /// <summary>
//    /// Uses ReferenceResolver (with fallback to ReferencingConfig.DefaultReferenceResolver()) to resolve Object.
//    /// </summary>
//    /// <typeparam name="ObjectType"></typeparam>
//    public class RDynamic<ObjectType> :
//        RBase<ObjectType>
//        //HRetrieveInfoBase<ObjectType>, IHasObjectRetrievalInfo<ObjectType>,
//        where ObjectType : class
//    {
//        #region Construction

//        public RDynamic() { }
//        public RDynamic(IReference reference, ObjectType obj = null) : base(reference, obj)
//        {
//        }

//        #endregion

//        #region Dynamic Retrieve

//        public virtual IReferenceRetriever EffectiveRetriever
//        {
//            get
//            {
//                return this.Retriever 
//                    ?? (this.Reference as IResolvingReference)?.Retriever 
//                    ?? ReferencingConfig.DefaultRetriever();
//            }
//        }

//        public virtual IReferenceRetriever Retriever
//        {
//            get; set;
//        }

//        public override async Task<bool> TryRetrieveObject()
//        {
//            return (await TryRetrieveObjectWithInfo().ConfigureAwait(false)).IsSuccess;
//        }

//        public override async Task<RetrieveReferenceResult<ObjectType>> TryRetrieveObjectWithInfo()
//        {
//            var result = await EffectiveRetriever.Retrieve<ObjectType>(this.Reference).ConfigureAwait(false);
//            if (result.IsSuccess)
//            {
//                this.Object = result.Result;
//                this.ResolveHandleResult = result;
//                this.IsReachable = true;
//            }
//            else
//            {
//                ResolveHandleResult = null;
//                IsReachable = false;
//                //if (forgetOnFail)
//                //{
//                //    this.DiscardObject();
//                //}
//            }
//            return result;
//        }

//        #endregion


//        //#region PersistenceContext

//        //// TODO: Make this MultiTyped and allow OBases to add to it for different reasons?
//        //// Or use ConditionalWeakTable?
//        //public object PersistenceContext
//        //{
//        //    get { return persistenceContext; }
//        //    set
//        //    {
//        //        if (persistenceContext == value)
//        //        {
//        //            return;
//        //        }

//        //        if (persistenceContext != default(object))
//        //        {
//        //            throw new AlreadySetException();
//        //        }

//        //        persistenceContext = value;
//        //    }
//        //}
//        //private object persistenceContext;

//        //#endregion

//        //public void SetObject(T value)
//        //{
//        //    this.Object = value;
//        //    if (value == null)
//        //    {
//        //        OutgoingDeletePending = true;
//        //    }
//        //}
//    }
//}
