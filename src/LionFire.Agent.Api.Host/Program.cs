using Oakton;
using Oakton.Resources;
using Wolverine;
using LionFire.Agent.Api.Host;
using LionFire.Hosting;
using Wolverine.Http;
using Wolverine.RabbitMQ;
using Marten;
using Wolverine.Marten;
using Marten.NodaTimePlugin;
using LionFire.Agent.Api.Host.Chat;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.SignalR.Protocol;
using LionFire.Chat;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.LionFire();

builder.Services.AddMarten(_ =>
    {
        var connectionString = builder.Configuration.GetConnectionString("Marten");
        _.Connection(connectionString);
        _.DatabaseSchemaName = "chat";
        _.UseNodaTime();
    })
    .IntegrateWithWolverine();

builder.Services.AddResourceSetupOnStartup();

builder.Host.UseWolverine(o =>
{
    o.PublishMessage<ChatMessage>().ToRabbitQueue("chat", o =>
    {
        o.IsDurable = true;
    })
    .UseDurableOutbox();
    //o.PublishAllMessages().ToRabbitQueue("all", o =>
    //{
    //    o.IsDurable = true;
    //});
    o.UseRabbitMq(rabbit =>
    {
        rabbit.HostName = "localhost";
    })
    .AutoProvision();
});
builder.Host.ApplyOaktonExtensions();


#region aspnet

builder.Services.AddAuthorization();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#endregion

// https://www.camiloterevinto.com/post/oauth-pkce-flow-for-asp-net-core-with-swagger
#region Authentication: Bearer: JWT
builder.Services
    .AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = builder.Configuration.GetSection("Auth:Authority").Get<string>();
    });
#endregion

#region Auth: Swagger

builder.Services.AddSwaggerGen(options =>
{
    var scheme = new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri(configuration.GetSection("Auth:Swagger:AuthorizationUrl").Get<string>()),
                TokenUrl = new Uri(configuration.GetSection("Auth:Swagger:TokenUrl").Get<string>()),
                //RefreshUrl = new Uri(configuration.GetSection("Auth:Swagger:RefreshUrl").Get<string>()) // TODO ?
            }
        },
        Type = SecuritySchemeType.OAuth2
    };

    options.AddSecurityDefinition("OAuth", scheme);

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Id = "OAuth", Type = ReferenceType.SecurityScheme }
            },
            new List<string> { }
        }
    });
});

#endregion

var app = builder.Build();

#region Dev
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.OAuthClientId("ca.lionfire.chat");
        //options.OAuthConfigObject = new Swashbuckle.AspNetCore.SwaggerUI.OAuthConfigObject()
        //{
            
        //};
        options.OAuthScopes("profile", "openid", "chat");
        options.OAuthUsePkce();
        options.EnablePersistAuthorization();
        //options.InjectStylesheet("/content/swagger-extras.css");
    });

    app.MapGet("/", () => Results.Redirect("/swagger"));
}
#endregion

#region aspnet

app.UseHttpsRedirection();

app.UseAuthorization();

#endregion

#region TEMP sample - WeatherForecast

var summaries = new[]
{
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

app.MapGet("/weatherforecast", (HttpContext httpContext) =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = summaries[Random.Shared.Next(summaries.Length)]
        })
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

#endregion

//app.MapPost("/event/create", (CreateEvent body, IMessageBus bus) => bus.InvokeAsync(body));
//app.MapPostToWolverine<CreateEvent, EventCreated>("/event/create");

app.MapPostToWolverine<SendChatMessage, ChatMessageAccepted>("/chat/send");
app.MapPostToWolverine<CreateChatSession, ChatSessionCreated>("/chat/create");
app.MapWolverineEndpoints(o =>
{

});

//app.Run();
return await app.RunOaktonCommands(args);


#if Experiments
public record TestResponse(string Message);
public record OnTest(string Message);

public static class TestEndpoint
{
    [WolverinePost("/test")]
    public static (TestResponse, OnTest) Post(TestResponse command)
    {
        
        // Just telling Marten that there's a new entity to persist,
        // but I'm assuming that the transactional middleware in Wolverine is
        // handling the asynchronous persistence outside of this handler
        //session.Store(todo);

        // By Wolverine.Http conventions, the first "return value" is always
        // assumed to be the Http response, and any subsequent values are
        // handled independently
        return (
            new TestResponse(command.Message+" response"),
            new OnTest(command.Message + " on")
        );
    }
}

public static class Handler
{
    public static void Handle( OnTest onTest)
    {
        Console.WriteLine($"OnTest: {onTest.Message}");
    }
}

public class HelloEndpoint
{
    [WolverineGet("/hello")]
    public string Get() => "Hello.";
}
#endif