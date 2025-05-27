
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Provider.Models;

namespace Provider.Services;

public class PackageBusListener : BackgroundService
{
    private readonly ServiceBusClient _client;
    private readonly ServiceBusProcessor _processor;
    private readonly ServiceBusSender _eventBusSender;
    private readonly IServiceScopeFactory _scopeFactory;

    public PackageBusListener(ServiceBusClient client,IServiceScopeFactory scopeFactory, ServiceBusSender eventBusSender)
    {
        _client = client;
        _eventBusSender = eventBusSender;
        _scopeFactory = scopeFactory;
        _processor = _client.CreateProcessor("packages-bus", new ServiceBusProcessorOptions());
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _processor.ProcessMessageAsync += ProcessMessageHandler;
        _processor.ProcessErrorAsync += ProcessErrorHandler;
        await _processor.StartProcessingAsync(stoppingToken);
    }

    private async Task ProcessMessageHandler(ProcessMessageEventArgs args)
    {
        using var scope = _scopeFactory.CreateScope();
        var packageService = scope.ServiceProvider.GetRequiredService<IPackageService>();

        var message = args.Message;
        var body = message.Body.ToString();
        var eventType = message.ApplicationProperties["EventType"].ToString();
        string responseMessage;
        ServiceBusMessage response;
        // Handle the message based on the event type
        switch (eventType)
        {
            case "Add":
                var packageToAdd = JsonSerializer.Deserialize<Package>(body);
                if (packageToAdd != null)
                {
                    var addResponse = await packageService.AddPackageAsync(packageToAdd);
                    if (!addResponse.Success)
                    {
                        throw new Exception(addResponse.Error);
                    }
                    responseMessage = JsonSerializer.Serialize(addResponse);
                }
                else
                {
                    responseMessage = JsonSerializer.Serialize(new ServiceResponse<bool>
                    {
                        Success = false,
                        Error = "Package to add is null."
                    });

                }
                response = new ServiceBusMessage(responseMessage)
                {
                    ApplicationProperties =
                    {
                        ["EventType"] = "AddResponse",
                        ["CorrelationId"] = message.ApplicationProperties["CorrelationId"]
                    }
                };
                await _eventBusSender.SendMessageAsync(response);
                break;


            case "GetPackageForEvent":
                var eventId = message.Body.ToString();
                if (!string.IsNullOrEmpty(eventId))
                {
                    var getResponse = await packageService.GetPackagesForEventAsync(eventId);
                    if (!getResponse.Success)
                    {
                        throw new Exception(getResponse.Error);
                    }
                    responseMessage = JsonSerializer.Serialize(getResponse.Data);
                    response = new ServiceBusMessage(responseMessage)
                    {
                        CorrelationId = message.CorrelationId,
                        ApplicationProperties =
                        {
                            ["EventType"] = "PackageResponse"
                        }
                    };
                    await _eventBusSender.SendMessageAsync(response);
                }
                break;


            case "Update":
                var packageToUpdate = JsonSerializer.Deserialize<Package>(body);
                if (packageToUpdate != null)
                {
                    var updateResponse = await packageService.UpdatePackage(packageToUpdate);
                    if (!updateResponse.Success)
                    {
                        throw new Exception(updateResponse.Error);
                    }
                }

                break;


            case "Delete":
                // Handle package updated event
                break;


            default:
                throw new ArgumentException($"Unknown event type: {eventType}");
        }
        await args.CompleteMessageAsync(message);
    }

    private Task ProcessErrorHandler(ProcessErrorEventArgs args)
    {
        // Handle the error
        Console.WriteLine($"Error processing message: {args.Exception}");
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _processor.StopProcessingAsync(cancellationToken);
        await _processor.DisposeAsync();
        await base.StopAsync(cancellationToken);
    }
}
