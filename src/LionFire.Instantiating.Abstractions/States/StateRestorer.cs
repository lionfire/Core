using LionFire.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using LionFire.Instantiating;

namespace LionFire.States
{
    public class StateRestorer : IInstantiator
    {
        #region Static

        // REVIEW
        public static Func<object, object, object> CreateFunction = (o, ctx) => new ExpandoObject();

        #endregion

#region Construction

        public StateRestorer(Dictionary<string, object> state)
{
this.State = state;
}
public StateRestorer( object obj)
{
this.State = obj.GetState();
}

#endregion

        public Dictionary<string, object> State { get; set; }

        /// <summary>
        /// If true, will create an ExpandoObject
        /// </summary>
        [DefaultValue(false)]
        public bool AllowCreate { get; set; }

        public object Affect(object obj, InstantiationContext context = null)
        {
            if (obj == null)
            {
                if (AllowCreate)
                {
                    obj = CreateFunction(obj, context);
                }
                else
                {
                    throw new ArgumentNullException(nameof(obj));
                }
            }

            if (State != null)
            {
                obj.AssignPropertiesFrom(State);
            }

            return obj;
        }
    }

}
