//using System;

//namespace LionFire.Referencing
//{
//    public class HReferenceDynamic<ObjectType> : HDynamic<ObjectType>, IH<ObjectType>
//        where ObjectType : class
//    {
//        #region Reference

//        #region Key

//        public override string Key
//        {
//            get => Reference?.ToString();
//            set => throw new NotSupportedException("Set Reference instead");
//        }

//        #endregion

//        [SetOnce]
//        public override IReference Reference
//        {
//            get { return reference; }
//            set
//            {
//                if (reference == value)
//                {
//                    return;
//                }

//                if (reference != default(IReference))
//                {
//                    throw new AlreadySetException();
//                }

//                reference = value;
//            }
//        }
//        private IReference reference;

//        #endregion

//        #region Construction

//        public HReferenceDynamic(IReference reference) 
//        {
//            this.reference = reference;
//        }

//        public HReferenceDynamic(IReference reference, ObjectType obj) : base(obj)
//        {
//            this.reference = reference;
//        }

//        #endregion
//    }
//}
