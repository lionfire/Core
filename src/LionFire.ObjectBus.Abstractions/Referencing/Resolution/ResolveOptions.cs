namespace LionFire.Referencing
{
    public class ResolveOptions
    {
        /// <remarks>
        /// May not be needed once NetStandard2.1 and IAsyncEnumerable arrives.
        /// </remarks>
        public int MaxResults { get; set; }

        public bool InheritedTypes { get; set; }

        //public PersistenceDirection Direction { get; set; }
        //protected PersistenceDirection direction { get; set; }

        /// <summary>
        /// If true, this implies VerifyExists = true.
        /// </summary>
        public bool VerifyDeserializable { get; set; }

        public bool VerifyExists { get; set; }
    }

}
