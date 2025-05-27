using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Provider.Data;
using Provider.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PackagesDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var serviceBusConnection = builder.Configuration["ServiceBusConnection"];
builder.Services.AddSingleton(x => new ServiceBusClient(serviceBusConnection));
builder.Services.AddSingleton(x =>
    x.GetRequiredService<ServiceBusClient>().CreateSender("event-bus"));
builder.Services.AddHostedService<PackageBusListener>();


builder.Services.AddScoped<IPackageService, PackageService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseCors(policy =>
    policy.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader());

app.MapOpenApi();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
