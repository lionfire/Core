//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace LionFire.Execution
//{
//    public static class ExecutionCloner
//    {
//        public static ExecutionContext Clone(this ExecutionContext context)
//        {
//            var ec = new ExecutionContext();
//            ec.AssignFrom(context);
//            //ec.Reset();

//            ec.ExecutionLocationUri = null;
//            ec.Executor = null;

//            return ec;
//        }
//    }
//}
