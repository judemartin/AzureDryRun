

using Func.TodoApi.Models.Todo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace Func.TodoApi.Functions
{
    public static class TodoQueueApiFunction
    {
        [FunctionName("CreateTodosQueues")]
        public static async Task<IActionResult> CreateTodosQueues(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todo-queues")]HttpRequest req,
           [Table("todos", Connection = "AzureWebJobsStorage")] IAsyncCollector<TodoTableEntity> todoTable,
           [Queue("todos", Connection = "AzureWebJobsStorage")] IAsyncCollector<Todo> todoQueue,
           ILogger log)
        {
            log.LogInformation("Creating a new Todo Item");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var model = JsonConvert.DeserializeObject<TodoCreateModel>(requestBody);
            var todo = new Todo() { TaskDescription = model.TaskDescription };

            await todoTable.AddAsync(todo.ToTableEntity()); // Adds Item to Table 
            await todoQueue.AddAsync(todo); //Adds Item to Queue 

            return new OkObjectResult(todo);
        }
    }
}
