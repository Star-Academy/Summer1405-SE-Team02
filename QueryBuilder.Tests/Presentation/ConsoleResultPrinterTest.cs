using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using QueryBuilder.Presentation;
using Xunit;

namespace QueryBuilder.Presentation
{
    public class ConsoleResultPrinterTest
    {
        [Fact]
        public void PrintResults_ShouldPrintKeyAndValue_Whenever()
        {
            // Arrange
            var result = new List<Dictionary<string, string>>
            {
                new() { ["age"] = "10" },
                new() { ["grade"] = "20" }
            };

            var originalOut = Console.Out;
            using var stringWriter = new StringWriter();

            try
            {
                Console.SetOut(stringWriter);

                // Act
                ConsoleResultPrinter.PrintResults(result);
            }
            finally
            {
                Console.SetOut(originalOut);
            }

            var actual = stringWriter.ToString()
                .Replace("\r\n", "\n")
                .Trim();

            var expected = "age: 10 | \ngrade: 20 |";

            // Assert
            actual.Should().Be(expected);
        }
    }
}