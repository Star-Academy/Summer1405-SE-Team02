using System;
using System.IO;
using System.Diagnostics.CodeAnalysis;


namespace QueryBuilder.Infrastructure
{
    [ExcludeFromCodeCoverage]
    internal static class EnvironmentLoader
    {
        public static void LoadEnvironmentVariables()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
            if (!File.Exists(filePath))
                return;

            foreach (var line in File.ReadAllLines(filePath))
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    continue;

                var parts = line.Split('=', 2, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2)
                {
                    Environment.SetEnvironmentVariable(parts[0].Trim(), parts[1].Trim());
                }
            }
        }
    }
}