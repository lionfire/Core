using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Behaviors
{
    public abstract class RunOnceCondition : Condition
    {
        #region RunOnceAndExit

        public bool RunOnceAndExit
        {
            get { return runOnceAndExit; }
            set { runOnceAndExit = value; }
        } private bool runOnceAndExit;

        #endregion

        //protected override BehaviorStatus OnStart()
        //{
        //    if (Context == null) Fault(ContextMissingMessage);

        //    if (Context.IsAlive) Fail(EntityDiedMessage);

        //    if (RunOnceAndExit)
        //    {
        //        return BehaviorStatus.Succeeded;
        //    }
        //    else
        //    {
        //        Context.FlagsChangedForFromTo += Context_FlagsChangedForFromTo;
        //        return BehaviorStatus.Running;
        //    }
        //}
    }

    // REVIEW:
    //public abstract class Monitor : Behavior
    //{
    //    public Monitor()
    //    {
    //        SucceedWhenDone = false;
    //    }

    //    #region FinishOnPass

    //    public bool FinishOnPass
    //    {
    //        get { return finishOnPass; }
    //        set { finishOnPass = value; }
    //    } private bool finishOnPass;

    //    #endregion

    //    #region FinishOnNotPassing

    //    public bool FinishOnNotPassing
    //    {
    //        get { return finishOnNotPassing; }
    //        set
    //        {
    //            finishOnNotPassing = value;
    //        }
    //    } private bool finishOnNotPassing;

    //    #endregion
        
    //}

    //public abstract class WaitForCondition 



    // REVIEW: potential parameters / alternate classes?
    //  Finish on True
    //  Finish on False
    //  Inverse
    public abstract class Condition : Behavior
    {
        

        public override string StatusWord
        {
            get
            {
                if (IsMonitoring)
                {
                    switch (Status)
                    {
                        //case BehaviorStatus.Uninitialized:
                        //    break;
                        //case BehaviorStatus.Initialized:
                        //    break;
                        //case BehaviorStatus.Running:
                        //    break;
                        case BehaviorStatus.Failed:
                            return "Watching";
                        case BehaviorStatus.Succeeded:
                            return "Active"; 
                        //case BehaviorStatus.Disposed:
                        //    break;
                        //case BehaviorStatus.Suspended:
                        //    break;
                        default:
                            break;
                    }
                }
                return base.StatusWord;
            }
        }

        
    }

    
}
