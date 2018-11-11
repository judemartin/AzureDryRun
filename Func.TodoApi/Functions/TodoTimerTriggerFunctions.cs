using System;
using System.Threading.Tasks;
using Func.TodoApi.Models.Todo;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;

namespace Func.TodoApi.Functions
{
    public static class TodoTimerTriggerFunctions
    {
        [FunctionName("TodoTimerTriggerFunctions")]
        public static async Task Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer,
            [Table("todos", Connection = "AzureWebJobsStorage")]CloudTable todoTable,
            ILogger log)
        {

            var query = new TableQuery<TodoTableEntity>();
            var segment = await todoTable.ExecuteQuerySegmentedAsync(query, null);
            var deletedCount = 0;
            foreach (var todo in segment)
            {
                if (todo.IsCompleted)
                {
                    await todoTable.ExecuteAsync(TableOperation.Delete(todo));
                    deletedCount++;
                }
                log.LogInformation($"Deleted {deletedCount} items at {DateTime.UtcNow}");
            }
        }
    }
}
