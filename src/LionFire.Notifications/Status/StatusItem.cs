using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using LionFire.Structures;

namespace LionFire.Status
{
    public class StatusItem : INotifyPropertyChanged, IKeyed<string>
    {

        public string Key { get; protected set; }


        #region StatusText

        public string StatusText
        {
            get { return statusText; }
            set
            {
                if (statusText == value) return;
                statusText = value;
                OnPropertyChanged(nameof(StatusText));
            }
        }
        private string statusText;

        #endregion


        #region Description

        public string Description
        {
            get { return description; }
            set
            {
                if (description == value) return;
                description = value;
                OnPropertyChanged(nameof(Description));
            }
        }
        private string description;

        #endregion


        #region Importance

        public Importance Importance
        {
            get { return importance; }
            set
            {
                if (importance == value) return;
                importance = value;
                OnPropertyChanged(nameof(Importance));
            }
        }
        private Importance importance;

        #endregion


        #region Urgency

        public Urgency Urgency
        {
            get { return urgency; }
            set
            {
                if (urgency == value) return;
                urgency = value;
                OnPropertyChanged(nameof(Urgency));
            }
        }
        private Urgency urgency;

        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}
