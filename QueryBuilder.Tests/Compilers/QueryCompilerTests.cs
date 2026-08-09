using System;
using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using QueryBuilder.Abstractions;
using QueryBuilder.Compilers;
using QueryBuilder.Models;
using Xunit;

namespace QueryBuilder.Tests.Compilers
{
    public class QueryCompilerTests
    {
        private readonly ISqlClauseCompiler _clauseCompiler;
        private readonly QueryCompiler _sut;

        public QueryCompilerTests()
        {
            _clauseCompiler = Substitute.For<ISqlClauseCompiler>();
            _sut = new QueryCompiler(_clauseCompiler);
        }

        [Fact]
        public void Compile_ShouldCombineSelectFromAndWhere_WhenWhereClauseIsNotEmpty()
        {
            // Arrange
            var query = new Query().From("student").Where("ismale", "true");

            _clauseCompiler.CompileSelect(query).Returns("SELECT *");
            _clauseCompiler.CompileFrom(query).Returns("FROM \"student\"");
            _clauseCompiler
                .CompileWhere(query, Arg.Any<Dictionary<string, string>>())
                .Returns("WHERE \"ismale\" = @p0");

            // Act
            var result = _sut.Compile(query);

            // Assert
            result.RawSql.Should().Be("SELECT * FROM \"student\" WHERE \"ismale\" = @p0");
            _clauseCompiler.Received(1).CompileSelect(query);
            _clauseCompiler.Received(1).CompileFrom(query);
            _clauseCompiler.Received(1).CompileWhere(query, result.Bindings);
        }

        [Fact]
        public void Compile_ShouldExposeBindingsPopulatedByClauseCompiler_WhenWhereClauseExists()
        {
            // Arrange
            var query = new Query().From("student").Where("ismale", "true");

            _clauseCompiler.CompileSelect(query).Returns("SELECT *");
            _clauseCompiler.CompileFrom(query).Returns("FROM \"student\"");
            _clauseCompiler
                .CompileWhere(query, Arg.Any<Dictionary<string, string>>())
                .Returns(callInfo =>
                {
                    callInfo.ArgAt<Dictionary<string, string>>(1).Add("@p0", "true");
                    return "WHERE \"ismale\" = @p0";
                });

            // Act
            var result = _sut.Compile(query);

            // Assert
            result.Bindings.Should().HaveCount(1);
            result.Bindings["@p0"].Should().Be("true");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Compile_ShouldTrimTrailingWhitespace_WhenWhereClauseIsNullOrWhiteSpace(string? whereClause)
        {
            // Arrange
            var query = new Query().From("student");

            _clauseCompiler.CompileSelect(query).Returns("SELECT *");
            _clauseCompiler.CompileFrom(query).Returns("FROM \"student\"");
            _clauseCompiler
                .CompileWhere(query, Arg.Any<Dictionary<string, string>>())
                .Returns(whereClause);

            // Act
            var result = _sut.Compile(query);

            // Assert
            result.RawSql.Should().Be("SELECT * FROM \"student\"");
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenSqlClauseCompilerIsNull()
        {
            // Arrange
            //act
            Action act = () => new QueryCompiler(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("sqlClauseCompiler");
        }
    }
}