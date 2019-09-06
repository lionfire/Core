using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Shell
{
    public interface INotifyClosing
    {
        /// <summary>
        /// Can return false to cancel closing.  The tab should then consider calling close again later (with an option to bypass the Closing event - FUTURE)
        /// </summary>
        /// <returns></returns>
        bool OnClosing();
    }

    

    //public interface IClosable
    //{
    //    event Action<object> Closed;
    //}
}
