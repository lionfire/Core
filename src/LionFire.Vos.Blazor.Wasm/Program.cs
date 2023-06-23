//using Microsoft.AspNetCore.Blazor.Hosting;

//namespace LionFire.Vos.Blazor.Wasm
//{
//    public class Program
//    {
//        public static void Main(string[] args)
//        {
//            CreateHostBuilder(args).Build().Run();
//        }

//        public static IWebAssemblyHostBuilder CreateHostBuilder(string[] args) =>
//            BlazorWebAssemblyHost.CreateDefaultBuilder()
//                .UseBlazorStartup<Startup>();
//    }
//}

#error TODO: seems broken, maybe rebase off of  dotnet new blazorwasm --hosted -o MyProject

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MyProject.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
