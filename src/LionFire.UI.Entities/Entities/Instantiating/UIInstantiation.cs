using System;

namespace LionFire.UI.Entities
{
    public class UIInstantiation
    {
        public string Path { get; set; }

        public UIReference Template { get; set; }

        public UIParameters Parameters { get; set; }

        public UIInstantiation(Type viewType, string path = null, string key = null)
        {
            Parameters = new UIParameters
            {
                Path = path,
                Key = key,
            };
            Template = new UIReference
            {
                ViewType = viewType,
            };
        }
    }
}
