// Might be useful
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Windows.Markup;
//using System.Windows;

//namespace LionFire.Avalon
//{
//    // Retrieved from StackOverflow:
//    // http://stackoverflow.com/questions/2901664/how-do-i-alter-the-default-style-of-a-button-without-wpf-reverting-from-aero-to-c

//    [MarkupExtensionReturnType(typeof(object))]
//    public class StaticApplicationResource : MarkupExtension
//    {
//        public StaticApplicationResource(object pResourceKey)
//        {
//            mResourceKey = pResourceKey;
//        }

//        private object _ResourceKey;

//        [ConstructorArgument("pResourceKey")]
//        public object mResourceKey
//        {
//            get { return _ResourceKey; }
//            set { _ResourceKey = value; }
//        }

//        public override object ProvideValue(IServiceProvider serviceProvider)
//        {
//            if (mResourceKey == null)
//                return null;

//            object o = Application.Current.TryFindResource(mResourceKey);

//            return o;
//        }
//    }
//}
