using QueryBuilder.Presentation;
using Xunit;
using System.IO;
using System;


namespace QueryBuilder.Presentation
{
    public class ConsoleResultPrinterTest
    {
        [Fact]
        public void PrintResults_ShouldPrintKeyAndValue_WhenCalled()
        {
            // Arrange
            var result = new List<Dictionary<string, string>>
            {
                new() { ["age"] = "10" },
                new() { ["grade"] = "20" }
            };

            var originalOut = Console.Out;

            using var stringWriter = new StringWriter();

            // Act
            try
            {
                Console.SetOut(stringWriter);
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
            Assert.Equal(expected, actual);
        }
    }
}