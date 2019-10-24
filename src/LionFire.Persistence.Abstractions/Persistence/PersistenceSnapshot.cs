namespace LionFire.Persistence
{
    public struct PersistenceSnapshot
    {
        public PersistenceState State { get; set; }
        public bool HasValue { get; set; }
        public object Value { get; set; }
    }

    public struct PersistenceSnapshot<TValue> : IPersistenceSnapshot<TValue>
    {
        public PersistenceSnapshot(PersistenceState state, TValue value, bool? hasValue)
        {
            State = state;
            Value = value;
            HasValue = hasValue ?? value != default;
        }

        public PersistenceState State { get; set; }
        public bool HasValue { get; set; }
        public TValue Value { get; set; }
    }
}
