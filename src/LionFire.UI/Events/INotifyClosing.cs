using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.UI
{
    //public class CancelableEventArgs<T>
    //{
    //    public bool CancellationRequested { get; set; }
    //    public T Context { get; set; }
    //}
    //public class CancelableEventArgs
    //{
    //    public bool CancellationRequested { get; set; }
    //}

    public interface INotifyClosing
    {
        /// <summary>
        /// Can return false to cancel closing.  The tab should then consider calling close again later (with an option to bypass the Closing event - FUTURE)
        /// </summary>
        /// <returns></returns>
        bool OnClosing();

        //void OnClosing(CancelableEventArgs<object> cancelableEventArgs); // ENH?
    }

    

    //public interface IClosable
    //{
    //    event Action<object> Closed;
    //}
}
