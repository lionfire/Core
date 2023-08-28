namespace LionFire.Overlays;

public static class OverlayX_NEW
{
    public static TValue? NextNonNull<TValue, TContainer>(this TContainer leaf, TValue? defaultValue = default)
        where TContainer : IParented<TContainer>, IHas<TValue>
    {
        var current = leaf;
        while (current != null)
        {
            if (current.Object != null) return current.Object;
            current = current.Parent;
        }
        return defaultValue;
    }

    //public static TValue GetEffective<TValue, TContainer>(this TContainer leaf, TValue? defaultValue = null)
    //where TContainer : IParented<TContainer>, IHas<TValue>
    //{
    //    //Func<TContainer, (TValue?, TContainer?)> getParent
    //    // TODO,maybe, though the proxy is better
    //}

    //public static TValue GetEffectiveProperty<TValue, TContainer>(Func<TContainer, (TValue?, TContainer?)> getParent, TValue defaultValue)
    //{

    //}

}


