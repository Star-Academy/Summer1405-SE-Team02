using System.Collections.Generic;
using FluentAssertions;
using QueryBuilder.Compilers;
using QueryBuilder.Models;
using Xunit;

namespace QueryBuilder.Tests.Compilers
{
    public class SqlClauseCompiler_PostgresDBTests
    {
        private readonly SqlClauseCompiler_PostgresDB _sut = new();

        [Fact]
        public void CompileSelect_ShouldReturnQuotedColumns_WhenColumnsAreSelected()
        {
            // Arrange
            var query = new Query().From("student").Select("firstname", "lastname");

            // Act
            var result = _sut.CompileSelect(query);

            // Assert
            result.Should().Be("SELECT \"firstname\", \"lastname\"");
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
        public void CompileFrom_ShouldReturnQuotedTableName_Whenever()
        {
            // Arrange
            var query = new Query().From("student");

            // Act
            var result = _sut.CompileFrom(query);

            // Assert
            result.Should().Be("FROM \"student\"");
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
        public void CompileWhere_ShouldReturnQuotedClauseAndAddBinding_WhenSingleConditionExists()
        {
            // Arrange
            var query = new Query().From("student").Where("age", "20");
            var bindings = new Dictionary<string, string>();

            // Act
            var result = _sut.CompileWhere(query, bindings);

            // Assert
            result.Should().Be("WHERE \"age\" = @p0");
            bindings.Should().HaveCount(1);
            bindings["@p0"].Should().Be("20");
        }

        [Fact]
        public void CompileWhere_ShouldJoinWithAndOrderParameters_WhenMultipleConditionsExist()
        {
            // Arrange
            var query = new Query().From("student")
                .Where("age", "20")
                .Where("city", "Tehran");
            var bindings = new Dictionary<string, string>();

            // Act
            var result = _sut.CompileWhere(query, bindings);

            // Assert
            result.Should().Be("WHERE \"age\" = @p0 AND \"city\" = @p1");
            bindings.Should().HaveCount(2);
            bindings["@p0"].Should().Be("20");
            bindings["@p1"].Should().Be("Tehran");
        }
    }
}