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



    public class HierarchicalRollup
    {
        public static string FileExtension = ".jsx";

        public static void Rollup(string path, Func<object, object> selector, List<IStatistic> stats = null)
        {

            // TODESIGN - where is the cache directory for a vos tree?

            foreach (var dir in Directory.GetDirectories(path))
            {
                Console.WriteLine(dir);
                Rollup(dir, selector, stats);
            }

            foreach (var file in Directory.GetFiles(path, "*" + FileExtension))
            {
                using (var sr = new StreamReader(new FileStream(file, FileMode.Open)))
                {
                    Console.WriteLine(file);
                    //Console.WriteLine(sr.ReadToEnd());
                }
            }
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
