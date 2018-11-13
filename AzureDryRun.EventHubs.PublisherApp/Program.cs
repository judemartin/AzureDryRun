using AzureDryRun.EventHubs.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDryRun.EventHubs.PublisherApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var publisher = new Publisher();

            publisher.Init(
               "");


            var random = new Random(Environment.TickCount);
            const int numEvents = 1000;
            Console.ForegroundColor = ConsoleColor.Green;

            for (var i = 0; i < numEvents; i++)
            {
                var deviceTelemetry = DeviceTelemetry.GenerateRandom(random);
                publisher.Publish(deviceTelemetry);
                Console.Clear();
                Console.WriteLine($"Published {i + 1} events...");
            }
            Console.ReadLine();
        }
    }
}
