using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.LionRing
{
    public interface IClientProvider
    {
        T GetClient<T>(string path = null)
            where T : class;

        //object GetClient(Type type, string path = null);

    }
}
