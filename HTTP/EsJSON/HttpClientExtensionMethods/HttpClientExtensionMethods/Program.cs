using System.Net.Http.Json;
using HttpProxyControl;

namespace HttpClientExtensionMethods
{
    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
    }

    public class Program
    {
        public static async Task Main()
        {
            HttpClient client = HttpProxyHelper.CreateHttpClient(setProxy: true);
            client.BaseAddress = new Uri("https://jsonplaceholder.typicode.com");

            for (int i = 1; i <= 10; i++)
            {

                // Get the user information.
                User? user = await client.GetFromJsonAsync<User>($"users/{i}");
                Console.WriteLine($"Id: {user?.Id}");
                Console.WriteLine($"Name: {user?.Name}");
                Console.WriteLine($"Username: {user?.Username}");
                Console.WriteLine($"Email: {user?.Email}");

                // Post a new user.
                HttpResponseMessage response = await client.PostAsJsonAsync("users", user);
                Console.WriteLine(
                    $"{(response.IsSuccessStatusCode ? "Success" : "Error")} - {response.StatusCode}");
            }
        }
    }
}
