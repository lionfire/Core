using FileContextCore;
using LionFire.MultiSolution.Host.Components;
using LionFire.MultiSolution.Host.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.FluentUI.AspNetCore.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddFluentUIComponents();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<MultiSolutionContext>(options => options.UseFileContextDatabase());
builder.Services.AddSingleton<DocumentService>();
builder.Services.Configure<DocumentsOptions>(builder.Configuration.GetSection("Documents"));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

public class MultiSolutionContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //optionsBuilder.UseFileContextDatabase();
    }
}