using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using HttpProxyControl;
public class Todo
{
    public int UserId { get; set; }
    public int Id { get; set; }
    public string Titolo { get; set; }
    public bool Completato { get; set; }
}

class Program
{
    static async Task Main(string[] args)
    {
        string url = "https://jsonplaceholder.typicode.com/todos";
        await TodoList(url);
    }

    static async Task TodoList(string apriurl)
    {
        HttpClient client = HttpProxyHelper.CreateHttpClient(setProxy: true);
        client.BaseAddress = new Uri(apriurl);
        List<Todo> todoList = await client.GetFromJsonAsync<List<Todo>>("");
        foreach (var todo in todoList)
        {
            Console.WriteLine($"Id: {todo.Id}");
            Console.WriteLine($"UserID: {todo.UserId}");
            Console.WriteLine($"Titolo: {todo.Titolo}");
            Console.WriteLine($"Completato: {todo.Completato}");
            Console.WriteLine();
        }
    }
}
