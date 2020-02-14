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
        typeof(DeleteAnItem)

        )]
    class Program
    {
        static Task<int> Main(string[] args)
        {
            return CommandLineApplication.ExecuteAsync<Program>(args);
        }


        

        [Option(Description = "clear all todo list", ValueName = "clear")]
        public string Clear { get; set; }

        [Option(Description = "set an item(s)'s status to be completed", ValueName = "done")]
        public string Done { get; set; }

        [Option(Description = "set an item(s)'s to not complete", ValueName = "undone")]
        public string Undone { get; set; }



    }

    [Command(Description = "show all todo list", Name = "list")]
    class ShowAll
    {
        [Argument(0)]
        public string List { get; set; }
        public async Task OnExecuteAsync()
        {
            var client = new HttpClient();
            var result = await client.GetStringAsync("http://localhost:3000/todo");
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
            var result = await client.PostAsync("http://localhost:3000/todo", content);

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
            var result = await client.PatchAsync($"http://localhost:3000/todo/{number}", content);

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
            var result = await client.DeleteAsync($"http://localhost:3000/todo/{number}");

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
