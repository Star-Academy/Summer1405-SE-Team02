using System;
using System.Collections.Generic;
using System.Linq;

var history = new List<string>();
var currentIndex = -1;

while (true)
{
    Console.Write("> ");
    var input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input)) continue;

    var parts = input.Split(' ', 2);
    var command = parts[0].ToUpper();
    var argument = parts.Length > 1 ? parts[1] : "";

    switch (command)
    {
        case "EXIT":
            return;

        case "SEARCH":
            if (string.IsNullOrWhiteSpace(argument))
            {
                Console.WriteLine("Please enter a word to search.");
                break;
            }

            if (currentIndex < history.Count - 1)
            {
                var itemsToRemove = history.Count - 1 - currentIndex;
                history.RemoveRange(currentIndex + 1, itemsToRemove);
            }

            history.Add(argument);
            currentIndex++;

            Console.WriteLine($"current: {history[currentIndex]}");
            break;

        case "CURRENT":
            if (currentIndex >= 0 && currentIndex < history.Count)
                Console.WriteLine($"current: {history[currentIndex]}");
            else
                Console.WriteLine("History is empty.");
            break;

        case "BACK":
            if (currentIndex > 0)
            {
                currentIndex--;
                Console.WriteLine($"current: {history[currentIndex]}");
            }
            else
            {
                Console.WriteLine("There is no back");
            }
            break;

        case "FORWARD":
            if (currentIndex < history.Count - 1)
            {
                currentIndex++;
                Console.WriteLine($"current: {history[currentIndex]}");
            }
            else
            {
                Console.WriteLine("There is no forward");
            }
            break;

        case "STATS":
            if (history.Count == 0)
            {
                Console.WriteLine("History is empty.");
                break;
            }

            var topSearches = history
                .GroupBy(word => word)
                .Select(group => new { Word = group.Key, Count = group.Count() })
                .OrderByDescending(item => item.Count)
                .Take(3);

            foreach (var item in topSearches)
            {
                Console.WriteLine($"{item.Word}: {item.Count}");
            }
            break;

        case "UNIQUE":
            var uniqueCount = history.Distinct().Count();
            Console.WriteLine(uniqueCount);
            break;

        default:
            Console.WriteLine($"Command '{command}' not implemented yet!");
            break;
    }
}