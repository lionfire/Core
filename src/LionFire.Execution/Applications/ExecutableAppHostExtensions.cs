using System;
using System.Collections.Generic;
using System.Text;
using LionFire.Applications;
using LionFire.Applications.Hosting;

namespace LionFire.Execution
{
    public static class ExecutableAppHostExtensions
    {
        /// <summary>
        /// TODO: Add this once I revisit how/whether IExecutable2 works
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="app"></param>
        /// <returns></returns>
        //public static IAppHost Execute<T>(this IAppHost app)
        //    where T : IExecutable2, new()
        //{
        //    //T exec = new T();

        //    //app.Add(exec);

        //    //var task = new AppTask(() =>
        //    //{
        //    //    exec.Start
        //    //    exec.WaitForRunCompletion
        //    //})
        //    //task.
        //}
    }
}
