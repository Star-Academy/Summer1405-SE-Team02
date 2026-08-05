using System.Collections.Generic;
using QueryBuilder.Compilers;
using QueryBuilder.Models;
using Xunit;

namespace QueryBuilder.Tests.Compilers
{
    public class SqlClauseCompiler_PostgresDBTests
    {
        private readonly SqlClauseCompiler_PostgresDB _compiler = new();

        [Fact]
        public void CompileSelect_ShouldReturnQuotedColumns_WhenColumnsAreSelected()
        {
            // Arrange
            var query = new Query().From("student").Select("firstname", "lastname");

            // Act
            var result = _compiler.CompileSelect(query);

            // Assert
            Assert.Equal("SELECT \"firstname\", \"lastname\"", result);
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
        public void CompileFrom_ShouldReturnQuotedTableName_WhenCalled()
        {
            // Arrange
            var query = new Query().From("student");

            // Act
            var result = _compiler.CompileFrom(query);

            // Assert
            Assert.Equal("FROM \"student\"", result);
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
        public void CompileWhere_ShouldReturnQuotedClauseAndAddBinding_WhenSingleConditionExists()
        {
            // Arrange
            var query = new Query().From("student").Where("age", "20");
            var bindings = new Dictionary<string, string>();

            // Act
            var result = _compiler.CompileWhere(query, bindings);

            // Assert
            Assert.Equal("WHERE \"age\" = @p0", result);
            Assert.Single(bindings);
            Assert.Equal("20", bindings["@p0"]);
        }

        [Fact]
        public void CompileWhere_ShouldJoinWithAndAndOrderParameters_WhenMultipleConditionsExist()
        {
            // Arrange
            var query = new Query().From("student")
                .Where("age", "20")
                .Where("city", "Tehran");
            var bindings = new Dictionary<string, string>();

            // Act
            var result = _compiler.CompileWhere(query, bindings);

            // Assert
            Assert.Equal("WHERE \"age\" = @p0 AND \"city\" = @p1", result);
            Assert.Equal(2, bindings.Count);
            Assert.Equal("20", bindings["@p0"]);
            Assert.Equal("Tehran", bindings["@p1"]);
        }
    }
}