using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Structures
{
//#if AOT // TEMP
	public delegate void ActionBool(bool isEnabled); // MOVE
//#endif
	
    public interface SEnableable
    {
        bool Enabled { get; set; }
        bool CanEnable { get; }
        string CannotEnableMessage { get; }

        //bool TryEnable(); 
    }

    public interface IEnableable : SEnableable
    {
        event Action<object, bool> EnabledChanged;
        event Action<object, bool> EnabledChanging;

        /// <summary>
        /// Indicates dependencies aren't met.  If true, try to enable other things before enabling this.
        /// </summary>
        bool DeferEnable { get; }

        // REVIEW this pattern.  Who makes sure this is set? It shouldn't be recalculated every time the message is requested.  (anti-pattern)
        IEnumerable<RuleError> CannotEnableReasons { get; }

        event Action CannotEnableReasonsChanged;
#if AOT
		event ActionBool CanEnableChanged;
#else
        event Action<bool> CanEnableChanged;
#endif

        
        event Action<string> CannotEnableMessageChanged; 
        
    }
}

namespace LionFire
{
    public static class IEnableableExtensions
    {
        public static string GetMessages(this IEnumerable<RuleError> ruleErrors)
        {
            return ruleErrors.Select(ruleError => ruleError.Message).Aggregate((x, y) => x + ", " + y);
        }

        public static bool TryEnable(this SEnableable e, bool desiredEnabled=true)
        {
            if (desiredEnabled)
            {
                if (e.CanEnable)
                {
                    e.Enabled = true; // TODO FIXME - make atomic/locking version.  IEnableable.EnableLock?
                }
            }
            else
            {
                e.Enabled = false;
            }
            return e.Enabled;
        }
    }
}