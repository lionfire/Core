using LionFire.Text.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using LionFire.Structures;
using LionFire.Execution.Configuration;
#if !NET462
using System.Runtime.Loader;
#endif
using System.Threading.Tasks;

namespace LionFire.Execution.Initialization
{
    public class ExecutionConfigResolver
    {

        public List<IExecutionConfigResolver> DefaultSourceUriResolvers;

        public IExecutionConfigResolver ScriptResolver {
            get; private set;
        } = new ScriptFileResolver();

        public ExecutionConfigResolver()
        {
            DefaultSourceUriResolvers = new List<IExecutionConfigResolver>{
            ScriptResolver,
            new LocalAssemblyFromUriResolver(),
            new AssemblyFromNugetResolver(),
        };
        }



        public async Task<bool> ResolveObject(ExecutionConfig c)
        {
            bool result = await ResolveType(c);
            if (result)
            {
                // REFACTOR - move this to some other location that can inject via DI
                c.Object = Activator.CreateInstance(c.Type);
            }
            return c.Object != null;
        }

        public async Task<bool> ResolveSourceContent(ExecutionConfig c)
        {
            if (c.SourceContent != null) return true;

            bool result = false;
            //if (c.SourceUriScheme != null)
            {
                switch (c.SourceUriScheme)
                {
                    case "http":
                    case "https":
                        var hc = new HttpClient();
                        var script = await hc.GetStringAsync(c.SourceUriScheme + ":" + c.SourceUriBody);
                        c.SourceContent = script;
                        c.ExecutionType = ExecutionKind.Script;
                        return true;
                    case "gist":
                        result |= await ScriptResolver.Resolve(c);
                        break;
                    case "github":
                        throw new NotImplementedException("github support coming soon");
                    case "nuget":
                        throw new NotImplementedException("nuget support coming soon");
                    case "assembly":
                        result = await Singleton<LocalAssemblyFromUriResolver>.Instance.Resolve(c);
                        break;
                    case null:
                    default:

                        foreach (var resolver in DefaultSourceUriResolvers)
                        {
                            result |= await resolver.Resolve(c);
                            if (result) break;
                        }
                        break;
                }
            }

            return result;
        }

        public async Task<bool> ResolveType(ExecutionConfig c, bool reresolve = false)
        {
            if (!reresolve)
            {
                if (c.Type != null) { return true; }
                if (c.Object != null)
                {
                    c.Type = c.Object.GetType();
                    return true;
                }
            }

            c.Type = null;
            c.Object = null;

            c.Package = null;
            c.PackageSchema = null;
            c.AssemblyName = null;
            c.AssemblyVersion = null;

            c.ExecutionType = ExecutionKind.Unspecified;

            //var arg = c.SourceUriBody;




            //string typeName = arg;


            /*
                        if (c.Package != null && c.Package.Contains(":"))
                        {
                            var split = arg.Split(new char[] { ':' }, 2);
                            c.PackageSchema = split[0].TrimStart('[');
                            c.Package = split[1];
                        }
            */
            /*
                        char AssemblyTypeSeparator = '/';

                        if (typeName.Contains(AssemblyTypeSeparator.ToString()))
                        {
                            var split = arg.Split(new char[] { AssemblyTypeSeparator }, 2);
                            c.AssemblyName = split[0];
                            typeName = split[1];
                        }

                        if (c.AssemblyName.Contains("`"))
                        {
                            var split = arg.Split(new char[] { '`' }, 2);
                            c.AssemblyName = split[0];
                            c.AssemblyVersion = split[1];
                        }

                        Assembly assembly = null;
                        if (c.AssemblyName != null)
                        {
                            await ResolveAssembly(c);

                            if (assembly == null)
                            {
                                throw new Exception("Failed to load assembly: " + c.AssemblyName + " " + c.AssemblyVersion);
                            }
                        }
            */
            try
            {
                if (c.Assembly != null)
                {
                    Type type = c.Assembly.GetType(c.TypeName);
                    if (type == null)
                    {
                        type = c.Assembly.GetType(c.AssemblyName + "." + c.TypeName);
                    }
                    c.Type = type;
                }
                else
                {
                    if (c.TypeName != null)
                    {
                        Type type = Type.GetType(c.TypeName);
                        c.Type = type;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            //c.TypeName = typeName;

            return c.Type != null;
        }

        /*      public async Task<bool> ResolveAssembly(ExecutionConfig c)
              {
                  c.Assembly = await ResolveAssemblyFromName(c.AssemblyName, c.AssemblyVersion);
                  return c.Assembly != null;
              }*/


    }

    public static class ExecutionTypeResolverExtensions
    {
        public static async Task<bool> ResolveObject(this ExecutionConfig config)
        {
            return await Singleton<ExecutionConfigResolver>.Instance.ResolveObject(config);
        }
        public static async Task<bool> ResolveType(this ExecutionConfig config)
        {
            return await Singleton<ExecutionConfigResolver>.Instance.ResolveType(config);
        }
        /*    public static async Task<bool> ResolveAssembly(this ExecutionConfig config)
            {
                return await Singleton<ExecutionTypeResolver>.Instance.ResolveAssembly(config);
            }
    */
    }
}
