using LionFire.Structures;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;

namespace LionFire.DependencyMachines
{
    public class DependenyMachineDefinition : IFreezable, INotifyPropertyChanged
    {

        #region IFreezable

        public bool IsFrozen { get; private set; }
        public void Freeze() => IsFrozen = true;

        #endregion

        public IEnumerable<IParticipant> Participants => participants.Values;
        internal ConcurrentDictionary<string, IParticipant> participants { get; } = new ConcurrentDictionary<string, IParticipant>();
        public DependencyMachineConfig? Config { get; set; }

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
