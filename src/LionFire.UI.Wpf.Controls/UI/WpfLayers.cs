using LionFire.Structures;
using LionFire.UI;
using LionFire.UI.Entities;
using System;
using System.Windows.Controls;

namespace LionFire.UI.Wpf
{
    public class WpfLayers : UICollection, ILayers
    {
        public Grid Grid { get; protected set; }

        public System.Collections.Generic.IReadOnlyDictionary<string, IUIKeyed> Object => throw new NotImplementedException();

        //string IKeyed<string>.Key => throw new NotImplementedException();

        //public ILayer GetLayer(LayerDefinition layerDefinition)
        //{
        //    throw new NotImplementedException();
        //}

        //ILayer ILayers.GetLayer(LayerDefinition layerDefinition) => throw new NotImplementedException();
    }
}
