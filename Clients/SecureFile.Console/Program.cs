class SecureFileConsole
{
    public static async Task<int> Main(string[] args)
    {
        if (args.Length == 0)
        {
            return 0;
        }

        switch (args[0])
        {
            case "create":
                await UserCreate.Handle();
                break;

            case "login":
                if (args.Length < 2)
                {
                    Console.WriteLine("Usage: SecureFile login <username>");
                    return 1;
                }

                await UserLogin.Handle(args[1]);
                break;

            default:
                Console.WriteLine("Invalid command");
                break;
        }

        return 0;
    }
}
