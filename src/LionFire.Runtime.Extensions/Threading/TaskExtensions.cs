using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace System.Threading.Tasks
{
    public static class TaskExtensions
    {
        public static Task FireAndForget(this Task task)
        {
            return task;
        }
    }
}
