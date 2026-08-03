using System;
using System.Collections.Generic;

namespace QueryBuilder.Presentation
{
    internal static class ConsoleResultPrinter
    {
        public static void PrintResults(List<Dictionary<string, string>> results)
        {
            foreach (var row in results)
            {
                foreach (var column in row)
                {
                    Console.Write($"{column.Key}: {column.Value} | ");
                }
                Console.WriteLine();
            }
        }
    }
}