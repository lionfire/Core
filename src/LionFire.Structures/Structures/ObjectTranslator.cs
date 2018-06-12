using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Structures
{
    public delegate TTarget ObjectTranslator<TSource, TTarget>(TSource source, object context = null);
    public delegate object ObjectTranslator(object source, object context = null);

}
