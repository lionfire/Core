using LionFire.Referencing;
using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Persistence
{
    public class ListingWrapper
    {
        public IListing<IReference> Listing { get; set; }
        public IReadHandle<object> ReadHandle { get; set; }
    }
}
