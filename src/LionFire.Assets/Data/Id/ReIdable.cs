﻿#if UNUSED
using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Data.Id;

public class ReIdable<T> : IdedSerializable<T>, IReIdable<T, string>
    where T : ReIdable<T>, IIdentified<T>
{
    public event Action<IIdentified<string>, string, string> IdChangedFromTo;

    public ReIdable(string id) : base(id) { }

    public void ReId(string newId)
    {
        var oldId = id;
        id = newId;
        if (oldId != null)
        {
            IdChangedFromTo?.Invoke(this, oldId, id);
        }
    }
}
#endif