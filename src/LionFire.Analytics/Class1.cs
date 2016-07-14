using LionFire.Analytics.Statistics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace LionFire.Analytics
{

    

    public class StatsRollup
    {
        public void Rollup(string path, Func<object, object> selector, List<IStatistic> stats = null)
        {
            //if (stats == null) stats = new List<IStatistic> { new Sum() };

            //foreach(var 

        }

        public void GenStatsForDir(IStatistic stat, string sourceDir, string statDir)
        {
            //foreach (var Directory.GetFiles(sourceDir))
            //{
            //    //JObject.ReadFrom(

            //}
        }
    }
}
