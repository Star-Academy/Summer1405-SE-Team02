using System.Collections.Generic;
using QueryBuilder.Compilers;
using QueryBuilder.Models;
using Xunit;

namespace QueryBuilder.Tests.Compilers
{
    public class SqlClauseCompiler_SqlServerDBTests
    {
        private readonly SqlClauseCompiler_SqlServerDB _compiler = new();

        [Fact]
        public void CompileSelect_ShouldReturnBracketedColumns_WhenColumnsAreSelected()
        {
            // Arrange
            var query = new Query().From("student").Select("firstname", "lastname");

            // Act
            var result = _compiler.CompileSelect(query);

            // Assert
            Assert.Equal("SELECT [firstname], [lastname]", result);
        }

        [Fact]
        public void CompileSelect_ShouldReturnStar_WhenNoColumnIsSelected()
        {
            // Arrange
            var query = new Query().From("student");

            // Act
            var result = _compiler.CompileSelect(query);

            // Assert
            Assert.Equal("SELECT *", result);
        }

        [Fact]
        public void CompileFrom_ShouldReturnBracketedTableName_WhenCalled()
        {
            // Arrange
            var query = new Query().From("student");

            // Act
            var result = _compiler.CompileFrom(query);

            // Assert
            Assert.Equal("FROM [student]", result);
        }

        [Fact]
        public void CompileWhere_ShouldReturnEmptyAndKeepBindingsEmpty_WhenNoConditionsExist()
        {
            // Arrange
            var query = new Query().From("student");
            var bindings = new Dictionary<string, string>();

            // Act
            var result = _compiler.CompileWhere(query, bindings);

            // Assert
            Assert.Equal(string.Empty, result);
            Assert.Empty(bindings);
        }

        [Fact]
        public void CompileWhere_ShouldReturnBracketedClauseAndAddBinding_WhenSingleConditionExists()
        {
            // Arrange
            var query = new Query().From("student").Where("age", "20");
            var bindings = new Dictionary<string, string>();

            // Act
            var result = _compiler.CompileWhere(query, bindings);

            // Assert
            Assert.Equal("WHERE [age] = @p0", result);
            Assert.Equal("20", bindings["@p0"]);
        }

        [Theory]
        [InlineData("true", "1")]
        [InlineData("True", "1")]
        [InlineData("false", "0")]
        [InlineData("False", "0")]
        public void CompileWhere_ShouldConvertBooleanToOneOrZero_WhenValueIsBoolean(string rawValue, string expectedValue)
        {
            // Arrange
            var query = new Query().From("student").Where("ismale", rawValue);
            var bindings = new Dictionary<string, string>();

            // Act
            _compiler.CompileWhere(query, bindings);

            // Assert
            Assert.Equal(expectedValue, bindings["@p0"]);
        }

        [Fact]
        public void CompileWhere_ShouldKeepRawValue_WhenValueIsNotBoolean()
        {
            // Arrange
            var query = new Query().From("student").Where("city", "Tehran");
            var bindings = new Dictionary<string, string>();

            // Act
            _compiler.CompileWhere(query, bindings);

            // Assert
            Assert.Equal("Tehran", bindings["@p0"]);
        }

        [Fact]
        public void CompileWhere_ShouldJoinWithAnd_WhenMultipleConditionsExist()
        {
            // Arrange
            var query = new Query().From("student")
                .Where("age", "20")
                .Where("ismale", "true");
            var bindings = new Dictionary<string, string>();

            // Act
            var result = _compiler.CompileWhere(query, bindings);

            // Assert
            Assert.Equal("WHERE [age] = @p0 AND [ismale] = @p1", result);
            Assert.Equal("20", bindings["@p0"]);
            Assert.Equal("1", bindings["@p1"]);
        }
    }
}