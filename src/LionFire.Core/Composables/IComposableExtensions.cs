using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Composables
{
    public static class IComposableExtensions
    {
        public static TComposable Add<TComposable, TComponent>(this TComposable composable)
            where TComposable : IComposable<TComposable>
            where TComponent : new()
        {
            var component = new TComponent();
            composable.Add(component);
            return composable;
        }
    }
}
