#nullable enable
using System;
using System.Reflection;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Templates.Themes;
using Serilog.Templates;
using Serilog.Sinks.File.GZip;
using Serilog.Formatting;
using LionFire.Hosting;

namespace LionFire.Logging.Serilog;

public class LionFireSerilogDefaults
{
    public static ExpressionTemplate? DebugTemplate { get; set; } = null;


    public static ExpressionTemplate ConsoleTemplate => LionFireEnvironment.IsUnitTest == true ? TestConsoleTemplate : new ExpressionTemplate(
            template: "[{@t:yyyy-MM-dd HH:mm:ss} {@l:u2} {ToString(Substring(SourceContext, LastIndexOf(SourceContext, '.')+1), '        '):u10}] {@m}\n{@x}",
             theme: TemplateTheme.Code);
    public static ExpressionTemplate LongConsoleTemplate => LionFireEnvironment.IsUnitTest == true ? TestConsoleTemplate : new ExpressionTemplate(
            template: "[{@t:yyyy-MM-dd HH:mm:ss} {@l:u2} {ToString(SourceContext, '        '):u10}] {@m}\n{@x}",
             theme: TemplateTheme.Code);

    public static ExpressionTemplate TestConsoleTemplate => new ExpressionTemplate(
            template: "[{@t:mm:ss} {@l:u2} {ToString(Substring(SourceContext, LastIndexOf(SourceContext, '.')+1), '        '):u10}] {@m}\n{@x}",
             theme: TemplateTheme.Code);
 
    public static ExpressionTemplate FileTemplate(string app) => new ExpressionTemplate(
                        "{ {@t, @m, @l: if @l = 'Verbose' then 'trace' else if @l = 'Debug' then 'dbug' else if @l = 'Trace' then 'trace' else if @l = 'Information' then 'info' else if @l = 'Warning' then 'warn' else if @l = 'Error' then 'error' else if @l = 'Critical' then 'crit' else @l, @x, @sc: SourceContext, @a: '" + (app ?? throw new ArgumentNullException(nameof(app))) + "', @p: Rest() } }\n");

    /// <summary>
    /// Including leading dot
    /// </summary>
    public static string DefaultLogFileExtension { get; set; } = ".log";
    public static string DefaultLogFileNameWithoutExtension
            => DefaultAppName;
    public static string DefaultAppName
            => Assembly.GetEntryAssembly()?.GetName().Name ?? Guid.NewGuid().ToString();
    
}
