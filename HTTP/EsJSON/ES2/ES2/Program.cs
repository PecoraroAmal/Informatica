using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
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

        try
        {
            string fileName = "todos.json";
            using (StreamWriter sw = File.CreateText(fileName))
            {
                await JsonSerializer.SerializeAsync(sw.BaseStream, todoList);
            }
            Console.WriteLine("Lista salvata su file todos.json.");
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
        }
    }
}
