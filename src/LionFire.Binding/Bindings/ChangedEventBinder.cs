using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Bindings
{
    public class ChangedEventBinder
    {

        public object BindingObject
        {
            get { return bindingObject; }
            set
            {
                if (bindingObject == value) return;

                if (bindingObject != null)
                {
                }
                
                bindingObject = value;

                if (bindingObject != null)
                {
                }
            }
        } private object bindingObject;
    }
}
