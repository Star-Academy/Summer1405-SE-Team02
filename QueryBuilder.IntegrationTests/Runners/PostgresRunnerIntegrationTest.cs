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

    private async Task SetupStudentsTable(NpgsqlConnection connection)
    {
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
            INSERT INTO "Students" VALUES (4, 'Maryam', 30, false);
            INSERT INTO "Students" VALUES (5, NULL, 22, true);
            """,
            connection
        );
        await setupCommand.ExecuteNonQueryAsync();
    }

    [Fact]
    public async Task Execute_ShouldReturnAllRecords_WhenNoFilterIsApplied()
    {
        // Arrange
        await using var connection = new NpgsqlConnection(_fixture.ConnectionString);
        await connection.OpenAsync();
        await SetupStudentsTable(connection);

        var query = new Query().From("Students");
        var compiledQuery = new QueryCompiler(new SqlClauseCompiler_PostgresDB()).Compile(query);
        var postgresRunner = new PostgresRunner(_fixture.ConnectionString, new ParameterBinder(), new DataReaderFormatter());

        // Act
        var queryResult = postgresRunner.Execute(compiledQuery);

        // Assert
        queryResult.Should().HaveCount(5);
        queryResult.First().Keys.Should().BeEquivalentTo("Id", "Name", "Age", "IsMale");
    }

    [Fact]
    public async Task Execute_ShouldReturnOnlySelectedColumns_WhenSelectIsCalled()
    {
        // Arrange
        await using var connection = new NpgsqlConnection(_fixture.ConnectionString);
        await connection.OpenAsync();
        await SetupStudentsTable(connection);

        var query = new Query().From("Students").Select("Name", "Age");
        var compiledQuery = new QueryCompiler(new SqlClauseCompiler_PostgresDB()).Compile(query);
        var postgresRunner = new PostgresRunner(_fixture.ConnectionString, new ParameterBinder(), new DataReaderFormatter());

        // Act
        var queryResult = postgresRunner.Execute(compiledQuery);

        // Assert
        queryResult.Should().HaveCount(5);
        queryResult.First().Keys.Should().BeEquivalentTo("Name", "Age");
    }

    [Fact]
    public async Task Execute_ShouldReturnOnlyMatchingRows_WhenMultipleWhereConditionsAreApplied()
    {
        // Arrange
        await using var connection = new NpgsqlConnection(_fixture.ConnectionString);
        await connection.OpenAsync();
        await SetupStudentsTable(connection);

        var query = new Query().From("Students").Where("Age", "20").Where("IsMale", "false");
        var compiledQuery = new QueryCompiler(new SqlClauseCompiler_PostgresDB()).Compile(query);
        var postgresRunner = new PostgresRunner(_fixture.ConnectionString, new ParameterBinder(), new DataReaderFormatter());

        // Act
        var queryResult = postgresRunner.Execute(compiledQuery);

        // Assert
        queryResult.Should().HaveCount(1);
        queryResult.First()["Name"].Should().Be("Sara");
    }

    [Fact]
    public async Task Execute_ShouldReturnEmptyList_WhenNoRecordsMatchTheCondition()
    {
        // Arrange
        await using var connection = new NpgsqlConnection(_fixture.ConnectionString);
        await connection.OpenAsync();
        await SetupStudentsTable(connection);

        var query = new Query().From("Students").Where("Age", "100");
        var compiledQuery = new QueryCompiler(new SqlClauseCompiler_PostgresDB()).Compile(query);
        var postgresRunner = new PostgresRunner(_fixture.ConnectionString, new ParameterBinder(), new DataReaderFormatter());

        // Act
        var queryResult = postgresRunner.Execute(compiledQuery);

        // Assert
        queryResult.Should().BeEmpty();
    }

    [Fact]
    public async Task Execute_ShouldReturnEmptyStringForNullColumn_WhenDatabaseContainsNullValue()
    {
        // Arrange
        await using var connection = new NpgsqlConnection(_fixture.ConnectionString);
        await connection.OpenAsync();
        await SetupStudentsTable(connection);

        var query = new Query().From("Students").Where("Age", "22");
        var compiledQuery = new QueryCompiler(new SqlClauseCompiler_PostgresDB()).Compile(query);
        var postgresRunner = new PostgresRunner(_fixture.ConnectionString, new ParameterBinder(), new DataReaderFormatter());

        // Act
        var queryResult = postgresRunner.Execute(compiledQuery);

        // Assert
        queryResult.Should().HaveCount(1);
        queryResult.First()["Name"].Should().Be(string.Empty);
    }

    [Fact]
    public async Task Execute_ShouldFindRecordExactly_WhenWhereValueContainsSpecialCharacters()
    {
        // Arrange
        await using var connection = new NpgsqlConnection(_fixture.ConnectionString);
        await connection.OpenAsync();

        await using var setupCommand = new NpgsqlCommand(
            """
            CREATE TABLE "Users" (
                "Id" INT,
                "Username" TEXT
            );
            INSERT INTO "Users" VALUES (1, ''' OR 1=1 --');
            INSERT INTO "Users" VALUES (2, 'NormalUser');
            """,
            connection
        );
        await setupCommand.ExecuteNonQueryAsync();

        var query = new Query().From("Users").Where("Username", "' OR 1=1 --");
        var compiledQuery = new QueryCompiler(new SqlClauseCompiler_PostgresDB()).Compile(query);
        var postgresRunner = new PostgresRunner(_fixture.ConnectionString, new ParameterBinder(), new DataReaderFormatter());

        // Act
        var queryResult = postgresRunner.Execute(compiledQuery);

        // Assert
        queryResult.Should().HaveCount(1);
        queryResult.First()["Id"].Should().Be("1");
    }

    [Fact]
    public async Task Execute_ShouldReturnOnlyMatchingRows_WhenFilteringByNumericColumn()
    {
        // Arrange
        await using var connection = new NpgsqlConnection(_fixture.ConnectionString);
        await connection.OpenAsync();
        await SetupStudentsTable(connection);

        var query = new Query().From("Students").Where("Age", "20");
        var compiledQuery = new QueryCompiler(new SqlClauseCompiler_PostgresDB()).Compile(query);
        var postgresRunner = new PostgresRunner(_fixture.ConnectionString, new ParameterBinder(), new DataReaderFormatter());

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
                        {"Id", "1"}, {"Name", "Ali"}, {"Age", "20"}, {"IsMale", "True"},
                    },
                    new Dictionary<string, string>
                    {
                        {"Id", "3"}, {"Name", "Sara"}, {"Age", "20"}, {"IsMale", "False"},
                    },
                }
            );
    }

    [Fact]
    public async Task Execute_ShouldReturnOnlyMatchingRows_WhenFilteringByBooleanColumn()
    {
        // Arrange
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
        var postgresRunner = new PostgresRunner(_fixture.ConnectionString, new ParameterBinder(), new DataReaderFormatter());

        // Act
        var queryResult = postgresRunner.Execute(compiledQuery);

        // Assert
        queryResult.Should().HaveCount(2);
    }
}