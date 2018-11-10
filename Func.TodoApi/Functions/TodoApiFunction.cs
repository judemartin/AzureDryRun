using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using Func.TodoApi.Models.Todo;
using System.Linq;

namespace Func.TodoApi.Functions
{
    public static class TodoApiFunction
    {
        static List<Todo> items = new List<Todo>();

        [FunctionName("CreateTodo")]
        public static async Task<IActionResult> CreateTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todo")]HttpRequest req, ILogger log)
        {
            log.LogInformation("Creating a new Todo Item");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<TodoCreateModel>(requestBody);
            var todo = new Todo() { TaskDescription = input.TaskDescription };
            items.Add(todo);

            return new OkObjectResult(todo);
        }

        [FunctionName("GetTodos")]
        public static IActionResult GetTodos(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo")]HttpRequest req, ILogger log)
        {
            log.LogInformation("Getting todo list items");
            return new OkObjectResult(items);
        }

        [FunctionName("GetTodoById")]
        public static IActionResult GetTodoById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo/{id}")]HttpRequest req,
            ILogger log, string Id)
        {
            var todo = items.FirstOrDefault(t => t.Id == Id);
            if (todo == null)
                return new NotFoundResult();

            return new OkObjectResult(todo);
        }

        [FunctionName("UpdateTodo")]
        public static async Task<IActionResult> UpdateTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "todo/{id}")]HttpRequest req,
            string Id, ILogger log)
        {
            var todo = items.FirstOrDefault(t => t.Id == Id);
            if (todo == null)
                return new NotFoundResult();

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var model = JsonConvert.DeserializeObject<TodoUpdateModel>(requestBody);
            todo.IsCompleted = model.IsCompleted;

            if (!string.IsNullOrEmpty(model.TaskDescription))
            {
                todo.TaskDescription = model.TaskDescription;
            }

            return new OkObjectResult(todo);
        }

        [FunctionName("DeleteTodo")]
        public static IActionResult DeleteTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "todo/{id}")]HttpRequest req,
            ILogger log, string Id)
        {
            var todo = items.FirstOrDefault(t => t.Id == Id);
            if (todo == null)
                return new NotFoundResult();

            items.Remove(todo);
            return new OkResult();
        }
    }
}
