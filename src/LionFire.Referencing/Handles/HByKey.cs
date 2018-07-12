namespace LionFire.Referencing
{
    // OPTIMIZATION: Store a key, and generate the IReference on demand if really needed
    public abstract class HByKey<ObjectType> : HBase<ObjectType>, IH<ObjectType>
        where ObjectType : class
    {

        protected abstract IReference GetReferenceFromKey(string key);
        protected abstract string SetKeyFromReference(IReference reference);

        #region Key

        public override string Key
        {
            get { return key; }
            set
            {
                if (key == value) return;
                if (key != default(string)) throw new AlreadySetException();
                key = value;
            }
        }
        private string key;

        #endregion


        #region Reference

        public override IReference Reference
        {
            get { return GetReferenceFromKey(key); }
            set { this.Key = SetKeyFromReference(value); }
        }

        #endregion
    }
}
