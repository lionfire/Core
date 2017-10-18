﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using LionFire.Types;

namespace LionFire.ObjectBus // RENAME to LionFire.Handles? or LionFire.Referencing?
{
    public class Handle<T> : OBusHandleBase<T>
        where T : class//, new()
    {
        #region Construction

        internal Handle(string uri, T obj = null) : base(uri, obj)
        {
        }

        public Handle(IReference reference, T obj = null) : base(reference, obj)
        {
        }

        public Handle(IReferencable referencable, T obj = null) : base(referencable, obj)
        {
        }

        public Handle(T obj = null, bool freezeObjectIfProvided = true) : base(obj, freezeObjectIfProvided)
        {
        }

        #endregion

        public virtual RetrieveInfo RetrieveInfo { get; set; }

        public virtual bool IsRetrieveInfoEnabled
        {
            get { return RetrieveInfo != null; }
            set
            {
                if (value) { if (RetrieveInfo == null) { RetrieveInfo = new RetrieveInfo(); } }
                else
                {
                    RetrieveInfo = null;
                }
            }
        }
    }

    /// <summary>
    /// Freeze behavior: After frozen, Reference can only be changed from null to non-null.  No other restrictions.
    /// </summary>
    public class Handle : HandleBase<object>
    {

        #region Construction

        internal Handle(string uri, object obj = null)
            : base(uri, obj)
        {
        }

        public Handle(IReference reference, object obj = null)
            : base(reference, obj)
        {
        }

        public Handle(IReferencable referencable, object obj = null)
            : base(referencable, obj)
        {
        }

        #endregion

        public override RetrieveInfo RetrieveInfo { get; set; }
    }
}
