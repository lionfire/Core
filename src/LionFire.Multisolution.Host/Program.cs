using FileContextCore;
using LionFire.MultiSolution.Host.Components;
using LionFire.MultiSolution.Host.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.FluentUI.AspNetCore.Components;
using System.ComponentModel;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddFluentUIComponents();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<MultiSolutionContext>();
//builder.Services.AddDbContext<MultiSolutionContext>(options => options.UseFileContextDatabase());
builder.Services.AddSingleton<DocumentService>();
builder.Services.Configure<DocumentsOptions>(builder.Configuration.GetSection("Documents"));
builder.Services.AddHostedService<X>();

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

public class X : BackgroundService
{
    public X(DocumentService documentService)
    {
        DocumentService = documentService;
    }

    public DocumentService DocumentService { get; }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.CompletedTask;   
    }
}