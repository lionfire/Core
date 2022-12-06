﻿using LionFire.Referencing;

namespace LionFire.Referencing;

public class ReferenceCast<TValue> : IReference<TValue>
{
    public ReferenceCast(IReference reference)
    {
        InnerReference = reference;
    }

    public IReference InnerReference { get; }

    public string Scheme => InnerReference.Scheme;

    public string Path => InnerReference.Path;

    public string Url => InnerReference.Url;

    public string Key => InnerReference.Key;
}