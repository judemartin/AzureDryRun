using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDryRun.EventHubs.Persistence
{
    internal class Program
    {
        private static void Main(string[] args)
        {

            const string eventHubConnectionString = "";
            const string eventHubName = "";


            const string storageAccountName = "";
            const string storageAccountKey = "";
            var storageConnectionString = $"DefaultEndpointsProtocol=https;AccountName={storageAccountName};AccountKey={storageAccountKey}";

            var eventProcessorHostName = Guid.NewGuid().ToString();
            var eventProcessorHost = new EventProcessorHost(
                eventProcessorHostName,
                eventHubName,
                EventHubConsumerGroup.DefaultGroupName,
                eventHubConnectionString,
                storageConnectionString);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Registering with Event Hubs...");
            var options = new EventProcessorOptions();
            options.ExceptionReceived += (sender, e) => { Console.WriteLine(e.Exception); };
            eventProcessorHost.RegisterEventProcessorAsync<DataPersistenceConsumer>(options).Wait();

            Console.WriteLine("Receiving. Press enter key to stop worker...");
            Console.ReadLine();
            eventProcessorHost.UnregisterEventProcessorAsync().Wait();

        }
    }
}
