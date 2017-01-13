using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LionFire.ExtensionMethods;
using LionFire.Structures;

namespace LionFire.Structures
{

    public interface IHasStateType
    {
        Type StateType { get; }
    }
    public interface IHasStateTypes
    {
        IEnumerable<Type> StateTypes { get; }
    }
}

namespace LionFire
{
    public static class IHasStateTypeExtensions
    {
        public static void SetState(this IHasStateType obj, object state)
        {
            if (state == null) return;
            obj.AssignPropertiesFrom(state);
        }
        public static object GetState(this IHasStateType obj)
        {
            var state = Activator.CreateInstance(obj.StateType);
            state.AssignPropertiesFrom(obj);
            return state;
        }

        public static void SetState(this IHasStateTypes obj, IEnumerable<object> states, bool assignAll = false)
        {
            if (states == null) return;

            if (assignAll)
            {
                throw new NotImplementedException();
            }
            else
            {
                foreach (var state in obj.StateTypes.Select(t => states.Where(s => s.GetType() == t)))
                {
                    // UNTESTED
                    obj.AssignPropertiesFrom(state);
                }
            }
        }
        public static List<object> GetStates(this IHasStateTypes obj)
        {
            List<object> states = new List<object>();
            foreach (var type in obj.StateTypes)
            {
                var state = Activator.CreateInstance(type);
                state.AssignPropertiesFrom(obj);
                states.Add(state);
            }
            return states;
        }
    }
}
