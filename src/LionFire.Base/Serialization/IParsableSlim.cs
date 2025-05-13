#if NET9_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Serialization;

public interface IParsableSlim<T> where T : IParsableSlim<T>
{

    public abstract static T Parse(string s);
}

#endif