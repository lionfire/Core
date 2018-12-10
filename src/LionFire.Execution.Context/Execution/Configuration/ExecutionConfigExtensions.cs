using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using LionFire.ExtensionMethods;
using LionFire.ExtensionMethods.Copying;

namespace LionFire.Execution.Configuration
{
    public static class ExecutionConfigExtensions
    {
        public static ExecutionConfig ImportFromFile(this ExecutionConfig ec, string path)
        {
            using (var sr = new StreamReader(new FileStream(path, FileMode.Open)))
            {
                var result = JsonConvert.DeserializeObject<ExecutionConfig>(sr.ReadToEnd());
                ec.AssignPropertiesFrom(result);
            }
            return ec;
        }
    }
}
