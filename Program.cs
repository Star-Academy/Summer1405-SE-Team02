using System;

namespace SearchHistoryApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var manager = new SearchHistoryManager();

            while (true)
            {
                Console.Write(Messages.Prompt);
                var input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input)) continue;

                var command_argument = input.Split(' ', 2);
                var command = command_argument[0].ToUpper();
                var argument = command_argument.Length > 1 ? command_argument[1] : string.Empty;

                switch (command)
                {
                    case "EXIT":
                        return;
                    case "SEARCH":
                        manager.Search(argument);
                        break;
                    case "CURRENT":
                        manager.PrintCurrent();
                        break;
                    case "BACK":
                        manager.GoBack();
                        break;
                    case "FORWARD":
                        manager.GoForward();
                        break;
                    case "STATS":
                        manager.PrintStats();
                        break;
                    case "UNIQUE":
                        manager.PrintUnique();
                        break;
                    default:
                        Console.WriteLine(string.Format(Messages.CommandNotImplemented, command));
                        break;
                }
            }
        }
    }
}