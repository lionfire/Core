using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Behaviors.Conditions
{
    public abstract class MustStayTrueCondition : WaitForCondition 
    {
        public override bool FinishOnFailing { get { return true; } }
        public override bool FinishOnPassing { get { return false; } }
    }
}
