using AzureDryRun.EventHubs.Core;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDryrun.EventHubs.ConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            const string eventHubConnectionString = "";
            const string eventHubName = "";

            // create a new publisher access key with send permissions in the Azure EventHub - Shared access policies
            const string publisherConnectionString = "";
            const string storageAccountName = "";
            const string storageAccountKey = "";
            var storageConnectionString = $"DefaultEndpointsProtocol=https;AccountName={storageAccountName};AccountKey={storageAccountKey}";

            var publisher = new Publisher();
            publisher.Init(publisherConnectionString);

            var strings = new List<string> { "Hello", "World" };
            publisher.Publish(strings);

            var eventProcessorHostName = Guid.NewGuid().ToString();
            var eventProcessorHost = new EventProcessorHost(eventProcessorHostName, eventHubName,
                EventHubConsumerGroup.DefaultGroupName, eventHubConnectionString, storageConnectionString);
            Console.WriteLine("Registering EventProcessor...");
            var options = new EventProcessorOptions();
            options.ExceptionReceived += (sender, e) => { Console.WriteLine(e.Exception); };
            eventProcessorHost.RegisterEventProcessorAsync<Consumer>(options).Wait();

            Console.WriteLine("Receiving. Press enter key to stop worker...");
            Console.ReadLine();
            eventProcessorHost.UnregisterEventProcessorAsync().Wait();
        }
    }
}
