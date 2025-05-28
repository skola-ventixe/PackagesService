using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Provider.Data;
using Provider.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});


var serviceBusConnection = builder.Configuration["ServiceBusConnection"];
builder.Services.AddSingleton(x => new ServiceBusClient(serviceBusConnection));
builder.Services.AddSingleton(x =>
    x.GetRequiredService<ServiceBusClient>().CreateSender("event-bus"));
builder.Services.AddSingleton<PackageBusListener>();
builder.Services.AddHostedService<PackageBusListener>();

builder.Services.AddScoped<IPackageService, PackageService>();

builder.Services.AddDbContext<PackagesDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



var app = builder.Build();

app.MapOpenApi();
app.UseHttpsRedirection();
app.UseCors("AllowAllOrigins");

app.UseAuthorization();

app.MapControllers();

app.Run();
