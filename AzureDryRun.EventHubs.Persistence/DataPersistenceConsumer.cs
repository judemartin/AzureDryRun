using AzureDryRun.EventHubs.Core;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDryRun.EventHubs.Persistence
{
    internal class DataPersistenceConsumer : IEventProcessor
    {
        private Stopwatch _checkpointStopWatch;

        async Task IEventProcessor.CloseAsync(PartitionContext context, CloseReason reason)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Releasing lease on Partition {0}. Reason: {1}.", context.Lease.PartitionId, reason);
            if (reason == CloseReason.Shutdown)
                await context.CheckpointAsync();
            Console.ReadLine();
        }

        Task IEventProcessor.OpenAsync(PartitionContext context)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("I am listening to Partition {0}, at position {1}.",
                context.Lease.PartitionId, context.Lease.Offset);
            _checkpointStopWatch = new Stopwatch();
            _checkpointStopWatch.Start();
            return Task.FromResult<object>(null);
        }

        async Task IEventProcessor.ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            foreach (var eventData in messages)
            {
                var data = Encoding.UTF8.GetString(eventData.GetBytes());
                var deviceTelemetry = JsonConvert.DeserializeObject<DeviceTelemetry>(data);

                const string connectionstring = "";

                using (var connection = new SqlConnection(connectionstring))
                {
                    connection.Open();

                    const string sqlCommandText =
                        "insert into DeviceTelemetry(IPAddress,Time,DeviceType,IsOn) values (@IPAddress,@Time,@DeviceType,@IsOn);";
                    using (var command = new SqlCommand(sqlCommandText, connection))
                    {
                        command.Parameters.AddWithValue("@IPAddress", deviceTelemetry.IpAddress);
                        command.Parameters.AddWithValue("@Time", deviceTelemetry.Time);
                        command.Parameters.AddWithValue("@DeviceType", deviceTelemetry.DeviceType);
                        command.Parameters.AddWithValue("@IsOn", deviceTelemetry.IsOn);

                        Console.WriteLine($"Added Device IP {deviceTelemetry.IpAddress} to the database.");
                        command.ExecuteNonQuery();
                    }
                }
            }
            if (_checkpointStopWatch.Elapsed > TimeSpan.FromMinutes(5))
            {
                await context.CheckpointAsync();
                _checkpointStopWatch.Restart();
            }
        }
    }
}
