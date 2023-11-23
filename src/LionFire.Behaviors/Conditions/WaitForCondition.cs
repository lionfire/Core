using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Behaviors
{

    public enum MonitoringStatus
    {
        Unspecified,
        /// <summary>
        /// TestCondition is returning true
        /// </summary>
        Passing,

        /// <summary>
        /// TestCondition is returning false (or 
        /// </summary>
        Failling,
    }

    /// <summary>
    /// Typical scenario: evaluate whether condition is true right away.  If not, attach
    /// to events and wait for it.  Then detach.  
    /// </summary>
    public abstract class WaitForCondition : Condition
    {
        public virtual bool FinishOnPassing { get { return true; } }
        public virtual bool FinishOnFailing { get { return false; } }

        protected override BehaviorStatus OnStart()
        {
            Refresh();
            //bool result = TestCondition();
            //OnCondition(result);

            if (!IsFinished) // THREADSAFETY.  If condition got flipped between here and above we will miss it
            {
                IsAttached = true;
                return BehaviorStatus.Running;
            }
            else
            {
                return Status;
            }
        }

        public void Refresh()
        {
            OnCondition(TestCondition());
        }

        private void OnCondition(bool state)
        {
            //bool isDone = state;
                //(state && DoneWhenTestsTrue) || (!state && !DoneWhenTestsTrue);

            //if (InvertConditionResult) state ^= true;

            if (state)
            {
                if (FinishOnPassing)
                {
                    Succeed();
                }
            }
            else
            {
                if (FinishOnFailing)
                {
                    Fail();
                }
            }

            if (state)
            //{
                if (SucceedWhenDone)
                //{
                    Succeed();
                //}
                else
                //{
                    Fail();
                //}
                //return false;
            //}
            //return true;
        }
        
        public abstract bool TestCondition();

        ///// <summary>
        ///// If false, TestCondition == true means passing/succeed, and TestCondition==false means waiting/fail.  If true, those are reversed.
        ///// </summary>
        //public virtual bool InvertConditionResult { get { return false; } }

        protected override void OnFinished()
        {
            IsAttached = false;
            base.OnFinished();
        }

        protected abstract bool IsAttached // REVIEW: Replace with IsMonitoring?
        {
            set;
        }
    }
}
