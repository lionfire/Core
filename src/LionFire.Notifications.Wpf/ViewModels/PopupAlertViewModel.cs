using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace LionFire.Notifications
{

    public interface IAlertViewModel
    {
        int Slot { get; set; }
        Key HotKey { get; set; }
    }

    public class SlottedAlert
    {
        public double Precedence { get; set; }
    }


    public class PopupAlertViewModel : Screen, IAlertViewModel
    {
        public int Slot { get; set; } = 1; // STUB

        public Key HotKey { get; set; } = Key.F1; // STUB


        public string Title { get; set; } = "title";
        public string Message { get; set; } = "msg";
        public string Detail { get; set; } = "detail";
        public bool HasDetail => !string.IsNullOrWhiteSpace(Detail);

        #region Properties

        public Dictionary<string,object> Properties
        {
            get { if (properties == null) { properties = new Dictionary<string, object>(); } return properties; }
            set { properties = value; }
        }
        private Dictionary<string,object> properties;

        #endregion

        protected override void OnActivate()
        {

            base.OnActivate();
        }

    }
}
