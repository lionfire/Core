using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace LionFire.Avalon
{
    public interface IDetailViewProvider
    {
        FrameworkElement GetDetailView(object masterObject);
    }
    
}
