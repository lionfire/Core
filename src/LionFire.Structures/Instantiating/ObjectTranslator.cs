using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Instantiating
{
    public delegate TTarget ObjectTranslator<TSource, TTarget>(TSource source, object context = null);

}
