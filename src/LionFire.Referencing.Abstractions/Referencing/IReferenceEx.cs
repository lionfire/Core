namespace LionFire.Referencing
{
    public interface IReferenceEx : IReference
    //#if AOT
    // IROStringKeyed
    //#else
    //IKeyed<string>
    //#endif
    {

        string Uri { get; }

        string Name { get; }


        //string Dimension { get; set; } // What is this???  Package or something

        /// <summary>
        /// REVIEW: Consider making this bool? and returning null if host unspecified
        /// </summary>
        bool IsLocalhost
        {
            get;
        }

        ///// <summary>
        ///// For Reference types that are aliases to other References, this will return the target.
        ///// This is invoked by the HandleFactory.
        ///// </summary>
        ///// <returns></returns>
        //IReference Resolve();

//#if !AOT
//        IHandle<T> GetHandle<T>(T obj = null)
//            where T : class;//, new();
//#endif
    }
}
