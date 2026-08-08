using System.Collections.Generic;
using FluentAssertions;
using QueryBuilder.Compilers;
using QueryBuilder.Models;
using Xunit;

namespace QueryBuilder.Tests.Compilers
{
    public class SqlClauseCompiler_SqlServerDBTests
    {
        private readonly SqlClauseCompiler_SqlServerDB _sut = new();

        [Fact]
        public void CompileSelect_ShouldReturnBracketedColumns_WhenColumnsAreSelected()
        {
            // Arrange
            var query = new Query().From("student").Select("firstname", "lastname");

            // Act
            var result = _sut.CompileSelect(query);

            // Assert
            result.Should().Be("SELECT [firstname], [lastname]");
        }

        [Fact]
        public void CompileSelect_ShouldReturnStar_WhenNoColumnIsSelected()
        {
            // Arrange
            var query = new Query().From("student");

            // Act
            var result = _sut.CompileSelect(query);

            // Assert
            result.Should().Be("SELECT *");
        }

        [Fact]
        public void CompileFrom_ShouldReturnBracketedTableName_Whenever()
        {
            // Arrange
            var query = new Query().From("student");

            // Act
            var result = _sut.CompileFrom(query);

            // Assert
            result.Should().Be("FROM [student]");
        }

        [Fact]
        public void CompileWhere_ShouldReturnEmptyAndKeepBindingsEmpty_WhenNoConditionsExist()
        {
            // Arrange
            var query = new Query().From("student");
            var bindings = new Dictionary<string, string>();

            // Act
            var result = _sut.CompileWhere(query, bindings);

            // Assert
            result.Should().BeEmpty();
            bindings.Should().BeEmpty();
        }

        [Fact]
        public void CompileWhere_ShouldReturnBracketedClauseAndAddBinding_WhenSingleConditionExists()
        {
            // Arrange
            var query = new Query().From("student").Where("age", "20");
            var bindings = new Dictionary<string, string>();

            // Act
            var result = _sut.CompileWhere(query, bindings);

            // Assert
            result.Should().Be("WHERE [age] = @p0");
            bindings["@p0"].Should().Be("20");
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
            _sut.CompileWhere(query, bindings);

            // Assert
            bindings["@p0"].Should().Be(expectedValue);
        }

        [Fact]
        public void CompileWhere_ShouldKeepRawValue_WhenValueIsNotBoolean()
        {
            // Arrange
            var query = new Query().From("student").Where("city", "Tehran");
            var bindings = new Dictionary<string, string>();

            // Act
            _sut.CompileWhere(query, bindings);

            // Assert
            bindings["@p0"].Should().Be("Tehran");
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
            var result = _sut.CompileWhere(query, bindings);

            // Assert
            result.Should().Be("WHERE [age] = @p0 AND [ismale] = @p1");
            bindings["@p0"].Should().Be("20");
            bindings["@p1"].Should().Be("1");
        }
    }
}