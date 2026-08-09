using FluentAssertions;
using Npgsql;
using QueryBuilder.Compilers;
using QueryBuilder.IntegrationTests.Fixtures;
using QueryBuilder.Models;
using QueryBuilder.Runners;
using Xunit;

namespace QueryBuilder.IntegrationTests.Runners;

public sealed class PostgresRunnerIntegrationTests : IClassFixture<PostgresDatabaseFixture>
{
    private readonly PostgresDatabaseFixture _fixture;

    public PostgresRunnerIntegrationTests(PostgresDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Execute_WithAgeFilter_ReturnsOnlyMatchingRows()
    {
        // Arrange
        await using var connection = new NpgsqlConnection(_fixture.ConnectionString);
        await connection.OpenAsync();

        await using var setupCommand = new NpgsqlCommand(
            """
            CREATE TABLE "Students" (
                "Id" INT,
                "Name" TEXT,
                "Age" INT,
                "IsMale" BOOLEAN
            );
            INSERT INTO "Students" VALUES (1, 'Ali', 20, true);
            INSERT INTO "Students" VALUES (2, 'Reza', 25, true);
            INSERT INTO "Students" VALUES (3, 'Sara', 20, false);
            """,
            connection
        );
        await setupCommand.ExecuteNonQueryAsync();

        var query = new Query().From("Students").Where("Age", "20");
        var compiledQuery = new QueryCompiler(new SqlClauseCompiler_PostgresDB()).Compile(query);
        var postgresRunner = new PostgresRunner(
            _fixture.ConnectionString,
            new ParameterBinder(),
            new DataReaderFormatter()
        );

        // Act
        var queryResult = postgresRunner.Execute(compiledQuery);

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

    [Fact]
    public async Task Execute_WithBooleanFilter_ReturnsOnlyMatchingRows()
    {
        await using var connection = new NpgsqlConnection(_fixture.ConnectionString);
        await connection.OpenAsync();

        await using var setupCommand = new NpgsqlCommand(
            """
            CREATE TABLE "Employees" (
                "Id" INT,
                "Name" TEXT,
                "IsActive" BOOLEAN
            );
            INSERT INTO "Employees" VALUES (1, 'Ali', true);
            INSERT INTO "Employees" VALUES (2, 'Reza', false);
            INSERT INTO "Employees" VALUES (3, 'Sara', true);
            """,
            connection
        );
        await setupCommand.ExecuteNonQueryAsync();

        var query = new Query().From("Employees").Where("IsActive", "true");
        var compiledQuery = new QueryCompiler(new SqlClauseCompiler_PostgresDB()).Compile(query);
        var postgresRunner = new PostgresRunner(
            _fixture.ConnectionString,
            new ParameterBinder(),
            new DataReaderFormatter()
        );

        // Act
        var queryResult = postgresRunner.Execute(compiledQuery);

        // Assert
        queryResult.Should().HaveCount(2);
    }
}
