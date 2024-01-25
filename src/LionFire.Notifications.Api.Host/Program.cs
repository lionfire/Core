using LionFire.Notifications;
using LionFire.Hosting;

//new HostApplicationBuilder(args)
//    .Build().Run();
var builder = WebApplication.CreateBuilder(args);

builder
    .LionFire(lf => lf
        .NotificationDispatcher()
    );


builder.Services.AddSingleton<NatsAlertDispatcher>();
builder.Services.AddSingleton<IAlertDispatcher>(sp => sp.GetRequiredService<NatsAlertDispatcher>());
builder.Services.AddHostedService(sp => sp.GetRequiredService<NatsAlertDispatcher>());

//builder.Services.AddSingleton<RedisAlertDispatcher>();
//builder.Services.AddSingleton<IAlertDispatcher,RedisAlertDispatcher>();
//builder.Services.AddHostedService(sp => sp.GetRequiredService<RedisAlertDispatcher>());

builder.Services.AddControllers().AddApplicationPart(typeof(NewAlertController).Assembly);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
