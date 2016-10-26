using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LionFire.Execution.Configuration;
using System.Net;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json.Linq;
using LionFire.ExtensionMethods;

namespace LionFire.Execution.Initialization
{
    public class ScriptFileResolver : IExecutionConfigResolver
    {
        List<string> ScriptPaths = new List<string>
        {
            @"E:\src\tmp\LxScripts",
            @"E:\st\Projects\Dev\C# Scripts\Reference",
        };

        public string GetFullPathForFile(string fileName)
        {
            if (fileName.Contains(":")) return fileName; // Assume absolute path with drive letter 
            if (fileName.StartsWith("/") || fileName.StartsWith("\\")) return fileName; // Assume absolute path with drive letter

            foreach (var dir in ScriptPaths)
            {
                var path = Path.Combine(dir, fileName);
                if (File.Exists(path))
                {
                    return path;
                }
            }
            return null;
        }

        private async Task<bool> ResolveGistScript(ExecutionConfig config)
        {
            var scriptFiles = new List<string>(); // TODO: Put these in the config
            try
            {
                var hc = new HttpClient();
                //hc.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.71 Safari/537.36");
                hc.DefaultRequestHeaders.Add("User-Agent", ".NET");
                //hc.DefaultRequestHeaders.Add("Cache -Control", "max-age=0");
                

                var split = config.SourceUriBody.Split('/');
                //if (split.Length != 2) throw new ArgumentException("gist: URIs must be in the format gist:username/path/to/gist");

                string url;
                string gistId;
                if (split.Length == 1)
                {
                    gistId = split[0];
                }
                else if (split.Length == 2)
                {
                    gistId = split[1];
                }
                else throw new ArgumentException();

                url = $"https://api.github.com/gists/{gistId}";

                string content = null;
                {
                    var response = await hc.GetAsync(url);
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Failed to retrieve \"{url}\".  HTTP code {response.StatusCode.ToString()}.");
                    }
                    content = await response.Content.ReadAsStringAsync();
                }

                string version = null;
                var fileUrls = new Dictionary<string, string>();
                {
                    var jobj = JObject.Parse(content);
                    version = (jobj["history"][0]["version"] as JValue).Value as string;
                    var files = jobj["files"];

                    if (files.Children().Count() > 1)
                    {
                        // TOLOG - warning of more than one file
                    }

                    foreach (var file in files.Children().OfType<JProperty>())
                    {
                        var val = file.Value as JObject;
                        var raw = val["raw_url"] as JValue;
                        fileUrls.Add(file.Name, raw.Value as string);
                        break;
                    }
                }

                var gistCacheRoot = @"E:\src\tmp\Gist";
                var gistCacheDir = Path.Combine(gistCacheRoot, gistId);
                var gistCacheVersionDir = Path.Combine(gistCacheDir, version);
                var curVersionPath = Path.Combine(gistCacheDir, ".CurrentVersion");

                bool isUpToDate = Directory.Exists(gistCacheVersionDir);
                // ENH: Verify file presence and sizes

                if (!isUpToDate)
                {
                    foreach (var kvp in fileUrls)
                    {
                        var response = await hc.GetAsync(kvp.Value);
                        if (!response.IsSuccessStatusCode)
                        {
                            throw new Exception($"Failed to retrieve \"{kvp.Value}\".  HTTP code {response.StatusCode.ToString()}.");
                        }
                        content = await response.Content.ReadAsStringAsync();

                        gistCacheVersionDir.EnsureDirectoryExists();

                        var path = Path.Combine(gistCacheVersionDir, kvp.Key);
                        using (var sw = new StreamWriter(new FileStream(path, FileMode.Create)))
                        {
                            sw.Write(content);
                        }
                        scriptFiles.Add(path);
                    }
                    using (var sw = new StreamWriter(new FileStream(curVersionPath, FileMode.Create)))
                    {
                        sw.Write(version);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
            
        }

        public async Task<bool> Resolve(ExecutionConfig config)
        {
            string content = null;
            var scriptFiles = new List<string>();

            if (config.SourceUriScheme == "http" || config.SourceUriScheme == "https")
            {
                var uri = $"{config.SourceUriScheme}:{config.SourceUriBody}";

                var hc = new HttpClient();
                var response = await hc.GetAsync(uri);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to retrieve \"{uri}\".  HTTP code {response.StatusCode.ToString()}.");
                }

                content = await response.Content.ReadAsStringAsync();
                config.ResolvedSourceUri = uri;
            }
            else if (config.SourceUriScheme == "gist")
            {
                await ResolveGistScript(config);
                
            }
            else if (config.SourceUriScheme == "file" || string.IsNullOrWhiteSpace(config.SourceUriScheme))
            {
                var path = GetFullPathForFile(config.SourceUriBody);

                if (path != null)
                {
                    using (var sr = new System.IO.StreamReader(new System.IO.FileStream(path, FileMode.Open)))
                    {
                        content = sr.ReadToEnd();
                        config.ResolvedSourceUri = "file://" + (path.Contains(":") ? "/" + path : path);
                    }
                }
            }

            if (content != null && config.SourceContent == null)
            {
                config.SourceContent = content;
                return true;
            }
            return false;
        }
    }

}