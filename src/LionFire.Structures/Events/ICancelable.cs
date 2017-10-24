using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Events
{
    public interface ICancelable
    {
        /// <summary>
        /// Request that the operation be canceled, optionally providing a reason for the cancelation.
        /// </summary>
        /// <param name="reason">A string or object containing info about why the cancelation is requested.</param>
        void Cancel(object reason = null);
    }
}
