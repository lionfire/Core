// DEPRECATED - use Contributes instead
//using LionFire.Collections.Concurrent;
//using LionFire.Data.Async.Gets;
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace LionFire.DependencyMachines
//{
//    public class Contributor : Participant<Contributor>
//    {
//        public Contributor(string contributesTo, string key, params string[] dependsOn)
//        {
//            Contributes = new List<object> { contributesTo };

//            this.Key = $"{key}"; //  key ?? (contributesTo + $" ({Guid.NewGuid()})");
//            if (dependsOn.Any())
//            {
//                Dependencies = new List<object>(dependsOn);
//            } 
//        }
//    }
//}
