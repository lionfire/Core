//#define TRACE_PROGRESSIVETASK
using LionFire.Execution.Jobs;
using LionFire.Reactive;
using LionFire.Reactive.Subjects;
using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Execution
{



    public abstract class ProgressiveJob : JobBase, IHasRunTask, IHasDescription, IHasProgress, IHasProgressMessage, INotifyPropertyChanged
    {

        public override bool IsCompleted { get { return progress >= 1; } }
        public string Description { get; set; }

        public CancellationToken CancellationToken { get; set; }


        #region Progress

        public double Progress
        {
            get { return progress; }
            set
            {
                if (progress == value) return;
                progress = value;
                OnPropertyChanged(nameof(Progress));
            }
        }
        private double progress = double.NaN;

        #endregion

        #region ProgressMessage

        public string ProgressMessage
        {
            get { return progressMessage; }
            set
            {
                if (progressMessage == value) return;
                progressMessage = value;
                OnPropertyChanged(nameof(ProgressMessage));
            }
        }
        private string progressMessage;

        #endregion


   
        public void UpdateProgress(double progressFactor, string message = null)
        {
            // TODO: Log
#if TRACE_PROGRESSIVETASK
            Console.WriteLine(this.ToString() + $" {progressFactor*100.0}% {message}");
#endif
            Progress = progressFactor;
            if (message != null) { ProgressMessage = message; }
        }


        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion




    }
}
