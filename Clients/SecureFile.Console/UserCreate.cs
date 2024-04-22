
using System.Text;
using Newtonsoft.Json;

public class UserCreate
{
    public class User
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
    
    public static async Task Handle()
    {
        var firstName = InputHelper.PromptForString("First Name");
        var lastName = InputHelper.PromptForString("Last Name");
        var userName = InputHelper.PromptForString("Username");
        var email = InputHelper.PromptForString("Email");
        var password = InputHelper.PromptForPassword("Password");

        var user = new User
        {
            FirstName = firstName,
            LastName = lastName,
            UserName = userName,
            Email = email,
            Password = password
        };

        using (var httpClient = new HttpClient())
        {
            string json = JsonConvert.SerializeObject(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("http://localhost:5222/users", content);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("User was created successfully");
            }
            else
            {
                Console.WriteLine($"Failed to create user. Status code: {response.StatusCode}");
            }
        }
    }
}
