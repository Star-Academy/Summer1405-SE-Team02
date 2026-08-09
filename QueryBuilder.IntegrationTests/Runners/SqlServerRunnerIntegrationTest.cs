using FluentAssertions;
using Microsoft.Data.SqlClient;
using QueryBuilder.Compilers;
using QueryBuilder.IntegrationTests.Fixtures;
using QueryBuilder.Models;
using QueryBuilder.Runners;
using Xunit;

namespace QueryBuilder.IntegrationTests.Runners;

public sealed class SqlServerRunnerIntegrationTests : IClassFixture<SqlServerDatabaseFixture>
{
    private readonly SqlServerDatabaseFixture _fixture;

    public SqlServerRunnerIntegrationTests(SqlServerDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Execute_WithAgeFilter_ReturnsOnlyMatchingRows()
    {
        // Arrange
        await using var connection = new SqlConnection(_fixture.ConnectionString);
        await connection.OpenAsync();

        await using var setupCommand = new SqlCommand(
            """
            CREATE TABLE Students (
                Id INT,
                Name NVARCHAR(100),
                Age INT,
                IsMale BIT
            );
            INSERT INTO Students VALUES (1, N'Ali', 20, 1);
            INSERT INTO Students VALUES (2, N'Reza', 25, 1);
            INSERT INTO Students VALUES (3, N'Sara', 20, 0);
            """,
            connection
        );
        await setupCommand.ExecuteNonQueryAsync();

        var query = new Query().From("Students").Where("Age", "20");
        var compiledQuery = new QueryCompiler(new SqlClauseCompiler_SqlServerDB()).Compile(query);
        var sqlServerRunner = new SqlServerRunner(
            _fixture.ConnectionString,
            new ParameterBinder(),
            new DataReaderFormatter()
        );

        // Act
        var queryResult = sqlServerRunner.Execute(compiledQuery);

        // Assert
        queryResult.Should().HaveCount(2);
        queryResult
            .Should()
            .BeEquivalentTo(
                new[]
                {
                    new Dictionary<string, string>
                    {
                        { "Id", "1" },
                        { "Name", "Ali" },
                        { "Age", "20" },
                        { "IsMale", "True" },
                    },
                    new Dictionary<string, string>
                    {
                        { "Id", "3" },
                        { "Name", "Sara" },
                        { "Age", "20" },
                        { "IsMale", "False" },
                    },
                }
            );
    }
}
