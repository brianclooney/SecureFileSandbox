using System.Text;

public class InputHelper
{
    public static string PromptForString(string message)
    {
        Console.Write($"Enter {message}: ");
        return Console.ReadLine() ?? "";
    }

    public static string PromptForPassword(string message)
    {
        var pass = new StringBuilder();

        Console.Write($"Enter {message}: ");

        while (true)
        {
            var key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Enter)
            {
                break;
            }
            else if (key.Key == ConsoleKey.Backspace)
            {
                if (pass.Length > 0)
                {
                    pass.Remove(pass.Length - 1, 1);
                    Console.Write("\b \b");
                }
            }
            else if (key.KeyChar != '\u0000')
            {
                pass.Append(key.KeyChar);
                Console.Write("*");
            }
        }

        Console.WriteLine("");

        return pass.ToString();
    }

}