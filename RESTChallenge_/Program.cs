using System;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;
using McMaster.Extensions.CommandLineUtils;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RESTChallenge_
{
    [HelpOption("--hlp")]
    [Subcommand(
        typeof(ShowAll),
        typeof(AddAnItem),
        typeof(UpdateItem),
        typeof(DeleteAnItem),
        typeof(ClearAll),
        typeof(SetTodoItemDone),
        typeof(UndoneItem)

        )]

    class Program
    {
        static Task<int> Main(string[] args)
        {
            return CommandLineApplication.ExecuteAsync<Program>(args);
        }


        

        
        



    }

    [Command(Description = "show all todo list", Name = "list")]
    class ShowAll
    {
        [Argument(0)]
        public string List { get; set; }
        public async Task OnExecuteAsync()
        {
            var client = new HttpClient();
            var result = await client.GetStringAsync("http://localhost:3000/todos");
            var content = JsonConvert.DeserializeObject<List<TodoList>>(result);

            foreach (var actv in content)
            {
                string Status = null;
                if (actv.status)
                {
                    Status = "(DONE)";
                }
                Console.WriteLine($" {actv.id}. {actv.activity} {Status}");
            }
            
        }
    }

    [Command(Description = "add an item", Name = "add")]
    class AddAnItem
    {
        [Argument(0)]
        public string Add { get; set; }
        public async Task OnExecuteAsync()
        {
            var client = new HttpClient();
            var request1 = new Requests() {activity = Add, status = false};
            var content = new StringContent(JsonConvert.SerializeObject(request1), Encoding.UTF8, "application/json");
            var result = await client.PostAsync("http://localhost:3000/todos", content);

            Console.WriteLine(result);
        }
    }

    [Command(Description = "update an item", Name = "update")]
    class UpdateItem
    {
        [Argument(0)]
        public int number { get; set; }

        [Argument(1)]
        public string Add { get; set; }
        public async Task OnExecuteAsync()
        {
            var client = new HttpClient();
            var request1 = new { id = Convert.ToInt32(number), activity = Add };
            var content = new StringContent(JsonConvert.SerializeObject(request1), Encoding.UTF8, "application/json");
            var result = await client.PatchAsync($"http://localhost:3000/todos/{number}", content);

            Console.WriteLine(result);
        }
    }

    [Command(Description = "delete an item", Name = "delete")]
    class DeleteAnItem
    {
        [Argument(0)]
        public int number { get; set; }

        public async Task OnExecuteAsync()
        {
            var client = new HttpClient();
            var request1 = new { id = Convert.ToInt32(number) };
            var content = new StringContent(JsonConvert.SerializeObject(request1), Encoding.UTF8, "application/json");
            var result = await client.DeleteAsync($"http://localhost:3000/todos/{number}");

            Console.WriteLine(result);
        }

    }

    [Command(Description = "clear all todo list", Name = "clear")]
    class ClearAll
    {
        public async Task OnExecuteAsync()
        {

            var prompt = Prompt.GetYesNo("You are about to clear all lists. Are you sure?", false, ConsoleColor.Red);

            var client = new HttpClient();

            if (prompt)
            {
                var result = await client.GetStringAsync("http://localhost:3000/todos");
                var content = JsonConvert.DeserializeObject<List<TodoList>>(result);
                var listID = new List<int>();

                foreach (var actv in content)
                {
                    listID.Add(actv.id);
                }
                foreach (var actv in listID)
                {
                    var fix = await client.DeleteAsync($"http://localhost:3000/todos/{actv}");
                }
            }
        }
    }

    [Command(Description = "set an item(s)'s status to be completed", Name = "done")]
    class SetTodoItemDone
    {
        [Argument(0)]
        public int number { get; set; }

        public async Task OnExecuteAsync()
        {
            var client = new HttpClient();
            var request1 = new { id = Convert.ToInt32(number), activity = true};
            var content = new StringContent(JsonConvert.SerializeObject(request1), Encoding.UTF8, "application/json");
            var result = await client.PatchAsync($"http://localhost:3000/todos/{number}", content);

            Console.WriteLine(result);
        }
    }

    [Command(Description = "set an item(s)'s to not complete", Name = "undone")]
    class UndoneItem
    {
        [Argument(0)]
        public int number { get; set; }
        public async Task OnExecuteAsync()
        {
            var client = new HttpClient();
            var request1 = new { id = Convert.ToInt32(number), activity = false };
            var content = new StringContent(JsonConvert.SerializeObject(request1), Encoding.UTF8, "application/json");
            var result = await client.PatchAsync($"http://localhost:3000/todos/{number}", content);

            Console.WriteLine(result);
        }

    }

    public class TodoList : Requests
    {
        public int id { get; set; }
 
    }

    public class Todos
    {
        public List<TodoList> todos { get; set; }
    }

    public class Requests
    {
        public string activity { get; set; }
        public bool status { get; set; } = false;

    }
}
