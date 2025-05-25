#if LEGACY
#if true // FIXME // TODO
//#define DEBUG_LOAD
//#define TRACE_LOAD_FAIL
using LionFire.Referencing;
using System.Threading.Tasks;
//using LionFire.Input;
//using LionFire.Extensions.DefaultValues;

namespace LionFire.Referencing
{

    /// <summary>
    /// Typical base for a handle
    /// TODO: This class is not useless -- refactor it out
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class HandleBase<T> : HandleBase2<T>
        where T : class
    {

        #region Construction

        public HandleBase(T obj = null, bool freezeObjectIfProvided = true)
            : base(obj, freezeObjectIfProvided)
        {
        }

        internal HandleBase(string uri, T obj = null, bool freezeObjectIfProvided = true)
            : base(uri, obj, freezeObjectIfProvided)
        {
        }

        public HandleBase(IReference reference, T obj = null, bool freezeObjectIfProvided = true)
            : base(reference, obj, freezeObjectIfProvided)
        {
        }

        public HandleBase(IReferenceable referencable, T obj = null, bool freezeObjectIfProvided = true)
            : base(referencable.Reference, obj, freezeObjectIfProvided)
        {
        }

        #endregion

        //#region Reference // MOVE to derived classes like OBusHandleBase

        //        public override IReference Reference {
        //            get { return reference; }
        //            set {
        //                if (reference == value) return;
        //                if (isFrozen)
        //                {
        //                    if (reference != default(IReference)) throw new NotSupportedException("IsFrozen == true.  Reference can only be set once.");
        //                }
        //#if DEBUG
        //                //if (value as VosReference != null) throw new InvalidOperationException("vh not valid here");
        //#endif

        //                var oldReference = reference;

        //                if (value != null)
        //                {
        //                    if (value.Type == null)
        //                    {
        //                        //IChangeableReferenceable cr = reference as IChangeableReferenceable;
        //                        //if (cr != null)
        //                        //{
        //                        //    cr.Type = typeof(T);
        //                        //}
        //                    }
        //                    else
        //                    {
        //                        if (value.Type != typeof(T))
        //                        {
        //                            if (!typeof(T).IsAssignableFrom(value.Type))
        //                            {
        //                                throw new ArgumentException("!typeof(T).IsAssignableFrom(value.Type)");
        //                            }
        //                        }
        //                    }
        //                }
        //                reference = value;
        //                OnReferenceChangedFrom(oldReference);
        //            }
        //        }
        //        private IReference reference;

        //#endregion

    }
}


#if false // MOVED to Referencing from OBus -- REVIEW the changes I made a while ago if any

//#define DEBUG_LOAD
//#define TRACE_LOAD_FAIL
using System;
using System.Text;
using LionFire.Types;
using LionFire.Collections;
using LionFire.Instantiating;
using System.Collections.Concurrent;
//using LionFire.Input;
//using LionFire.Extensions.DefaultValues;

namespace LionFire.ObjectBus
{

    // TODO: Eliminate OBus stuff and move to Referencing??

    /// <summary>
    /// Typical base for a handle
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class HandleBase<T> : HandleBase2<T>
        , IChangeableReferenceable
        , IFreezable
        where T : class//, new()
    {

#region Construction

        public HandleBase(T obj = null, bool freezeObjectIfProvided = true)
            : base(obj, freezeObjectIfProvided)
        {
        }

        internal HandleBase(string uri, T obj = null, bool freezeObjectIfProvided = true)
            : base(uri, obj, freezeObjectIfProvided)
        {
        }

        public HandleBase(IReference reference, T obj = null, bool freezeObjectIfProvided = true)
            : base(reference, obj, freezeObjectIfProvided)
        {
        }

        public HandleBase(IReferenceable referencable, T obj = null, bool freezeObjectIfProvided = true)
            : base(referencable, obj, freezeObjectIfProvided)
        {
            IReference reference = referencable.Reference;

            if (!reference.IsValid())
            {
                throw new ArgumentException("referencable.Reference must be valid");
            }

            this.Reference = referencable.Reference;
        }

#endregion

#region Reference

#region Reference

        public override IReference Reference {
            get { return reference; }
            set {
                if (reference == value) return;
                if (isFrozen)
                {
                    if (reference != default(IReference)) throw new NotSupportedException("IsFrozen == true.  Reference can only be set once.");
                }
#if DEBUG
                if (value as VosReference != null) throw new InvalidOperationException("vh not valid here");
#endif

                var oldReference = reference;

                if (value != null)
                {
                    if (value.Type == null)
                    {
                        //IChangeableReferenceable cr = reference as IChangeableReferenceable;
                        //if (cr != null)
                        //{
                        //    cr.Type = typeof(T);
                        //}
                    }
                    else
                    {
                        if (value.Type != typeof(T))
                        {
                            if (!typeof(T).IsAssignableFrom(value.Type))
                            {
                                throw new ArgumentException("!typeof(T).IsAssignableFrom(value.Type)");
                            }
                        }
                    }
                }
                reference = value;
                OnReferenceChangedFrom(oldReference);
            }
        }
        private IReference reference;

#endregion

#endregion

#region IFreezable

        public bool IsFrozen {
            get {
                return isFrozen;
            }
            set {
                if (isFrozen == value) return;

                if (isFrozen && !value) { throw new NotSupportedException("Unfreeze not supported"); }

                isFrozen = value;
            }
        }
        private bool isFrozen;

#endregion
    }
}

#endif
#endif
#endif