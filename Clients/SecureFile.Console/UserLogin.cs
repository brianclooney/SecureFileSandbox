using System.Text;
using Newtonsoft.Json;

public class UserLogin
{
    public static async Task Handle(string userName)
    {
        var password = InputHelper.PromptForPassword("Password");

        using (var httpClient = new HttpClient())
        {
            string json = JsonConvert.SerializeObject(new
            {
                UserName = userName,
                Password = password
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("http://localhost:5222/users/login", content);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("User logged in successfully");
            }
            else
            {
                Console.WriteLine($"Failed to login. Status code: {response.StatusCode}");
            }
        }
    }
}
