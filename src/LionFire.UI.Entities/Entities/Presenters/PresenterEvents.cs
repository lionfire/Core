using System;

namespace LionFire.UI.Entities
{
    public static class PresenterEvents
    {
        public static Action<IUIObject> OnShowing = obj =>
        {
            if(obj is IActivatable a) { a.Active = true; }
            if(obj is IVisible v) { v.Visible = true; }
            if(obj is ICollapsible c) { c.Collapsed = false; }
        };

        public static Action<IUIObject> OnHiding = obj =>
        {
            if (obj is IVisible v) { v.Visible = false; }
            else if (obj is ICollapsible c) { c.Collapsed = true; } // only collapse if not IVisible

            if (obj is IActivatable a) { a.Active = false; }
        };
    }
}
