using LionFire.Structures;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;

namespace LionFire.DependencyMachine
{
    public class DependenyMachineDefinition : IFreezable, INotifyPropertyChanged
    {

        #region IFreezable

        public bool IsFrozen { get; private set; }
        public void Freeze() => IsFrozen = true;

        #endregion

        public IEnumerable<IDependencyMachineParticipant> Participants => participants.Values;
        internal ConcurrentDictionary<string, IDependencyMachineParticipant> participants { get; } = new ConcurrentDictionary<string, IDependencyMachineParticipant>();

        public Dictionary<object, object> Dependencies = new Dictionary<object, object>();
        public void AddDependency(object dependant, object dependency)
        {
            if (IsFrozen) throw new ObjectFrozenException();
            Dependencies.Add(dependant, dependency);
            OnPropertyChanged(nameof(Dependencies));
        }
        public void AddDependency<TDependency>(object dependant) => AddDependency(dependant, typeof(TDependency));


        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion

    }
}
