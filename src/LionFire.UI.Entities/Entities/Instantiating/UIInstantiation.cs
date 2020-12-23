using System;

namespace LionFire.UI.Entities
{
    public class UIInstantiation
    {
        public string Path { get; set; }

        public UIReference Template { get; set; }

        public UIParameters Parameters { get; set; }

        public UIInstantiation(Type viewOrEntityType, string path = null, string key = null)
        {
            Type entityType = null;
            Type viewType = null;
            if (typeof(IUIObject).IsAssignableFrom(viewOrEntityType))
            {
                entityType = viewOrEntityType;
            }
            else
            {
                viewType = viewOrEntityType;
            }
            Parameters = new UIParameters
            {
                Path = path,
                Key = key,
            };
            Template = new UIReference
            {
                EntityType = entityType,
                ViewType = viewType,
            };
        }
    }
}
