using System;
using System.Threading.Tasks;
using Func.TodoApi.Models.Todo;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Func.TodoApi.Functions
{
    public static class TodoQueueTriggerFunctions
    {
        [FunctionName("TodoQueueTrigger")]
        public static async Task Run([QueueTrigger("todos", Connection = "AzureWebJobsStorage")]Todo todo,
            [Blob("todos", Connection = "AzureWebJobsStorage")] CloudBlobContainer container,
            ILogger log)
        {
            await container.CreateIfNotExistsAsync();

            if (todo.TaskDescription.Contains("!Delete"))
            {
                return;
            }
            var blob = container.GetBlockBlobReference($"{todo.TaskDescription}");
            await blob.UploadTextAsync($"Created a new task: {todo.TaskDescription}");
            log.LogInformation($"C# Queue trigger Fx processed: {todo.TaskDescription}");
        }
    }
}
