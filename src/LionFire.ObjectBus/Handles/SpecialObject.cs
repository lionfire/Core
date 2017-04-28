using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.ObjectBus
{
    public enum SpecialObjectCode
    {
        Unspecified = 0,
        Null = 1,
        LoadOnDemand = 1,
    }

    public struct SpecialObject
    {
        public SpecialObject(SpecialObjectCode code)
        {
            this.Code = code;
        }

        public SpecialObjectCode Code;

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            try
            {
                SpecialObject other = (SpecialObject)obj;
                return other.Code == this.Code;
            }
            catch (InvalidCastException)
            {
                return false;
            }
        }
        public override int GetHashCode()
        {
            return Code.GetHashCode();
        }


        #region Static

        static SpecialObject()
        {
            nullObject = new SpecialObject(SpecialObjectCode.Null);
            //loadOnDemand = new SpecialObject(SpecialObjectCode.LoadOnDemand);
        }

        public static SpecialObject NullObject { get { return nullObject; } }
        private static readonly SpecialObject nullObject;

        //public static SpecialObject LoadOnDemand { get { return loadOnDemand; } }
        //private static readonly SpecialObject loadOnDemand;

        #endregion

    }
}
