using FluentAssertions;
using Npgsql;
using QueryBuilder.Compilers;
using QueryBuilder.IntegrationTests.Runners.Postgres;
using QueryBuilder.Models;
using QueryBuilder.Runners;
using Xunit;

namespace QueryBuilder.IntegrationTests.Runners;

public sealed class PostgresRunnerIntegrationTests :
    IClassFixture<PostgresDatabaseFixture>,
    IAsyncLifetime
{
    private readonly PostgresDatabaseFixture _sqlServerDatabaseFixture;

    private NpgsqlConnection _connection = null!;
    private PostgresRunner sut = null!;

    public PostgresRunnerIntegrationTests(PostgresDatabaseFixture sqlServerDatabaseFixture)
    {
        _sqlServerDatabaseFixture = sqlServerDatabaseFixture ?? throw new ArgumentNullException(nameof(PostgresDatabaseFixture));
    }

    public async Task InitializeAsync()
    {
        _connection = new NpgsqlConnection(_sqlServerDatabaseFixture.ConnectionString);

        await _connection.OpenAsync();

        sut = new PostgresRunner(
            _sqlServerDatabaseFixture.ConnectionString,
            new ParameterBinder(),
            new DataReaderFormatter());
    }

    public async Task DisposeAsync()
    {
        if (_connection is not null)
        {
            await _connection.DisposeAsync();
        }
    }

    [Fact]
    public void Execute_ShouldReturnAllColumns_WhenSelectIsEmpty()
    {
        // Arrange
        var query = new Query()
            .From("Students");

        var compiledQuery = new QueryCompiler(
            new SqlClauseCompiler_PostgresDB())
            .Compile(query);

        // Act
        var queryResult = sut.Execute(compiledQuery);

        // Assert
        queryResult.Should().HaveCount(5);
        queryResult.First().Keys
            .Should()
            .BeEquivalentTo("Id", "Name", "Age", "IsMale");
    }

    [Fact]
    public void Execute_ShouldReturnAllRecords_WhenNoFilterIsApplied()
    {
        // Arrange
        var query = new Query()
            .Select("Name")
            .From("Students");

        var compiledQuery = new QueryCompiler(
            new SqlClauseCompiler_PostgresDB())
            .Compile(query);

        // Act
        var queryResult = sut.Execute(compiledQuery);

        // Assert
        queryResult.Should().HaveCount(5);
        queryResult.First().Keys
            .Should()
            .BeEquivalentTo("Name");
    }

    [Fact]
    public void Execute_ShouldReturnOnlySelectedColumns_WhenSelectIsCalled()
    {
        // Arrange
        var query = new Query()
            .From("Students")
            .Select("Name", "Age");

        var compiledQuery = new QueryCompiler(
            new SqlClauseCompiler_PostgresDB())
            .Compile(query);

        // Act
        var queryResult = sut.Execute(compiledQuery);

        // Assert
        queryResult.Should().HaveCount(5);
        queryResult.First().Keys
            .Should()
            .BeEquivalentTo("Name", "Age");
    }

    [Fact]
    public void Execute_ShouldReturnOnlyMatchingRows_WhenMultipleWhereConditionsAreApplied()
    {
        // Arrange
        var query = new Query()
            .From("Students")
            .Where("Age", "20")
            .Where("IsMale", "false");

        var compiledQuery = new QueryCompiler(
            new SqlClauseCompiler_PostgresDB())
            .Compile(query);

        // Act
        var queryResult = sut.Execute(compiledQuery);

        // Assert
        queryResult.Should().HaveCount(1);
        queryResult.First()["Name"].Should().Be("Sara");
    }

    [Fact]
    public void Execute_ShouldReturnEmptyList_WhenNoRecordsMatchTheCondition()
    {
        // Arrange
        var query = new Query()
            .From("Students")
            .Where("Age", "100");

        var compiledQuery = new QueryCompiler(
            new SqlClauseCompiler_PostgresDB())
            .Compile(query);

        // Act
        var queryResult = sut.Execute(compiledQuery);

        // Assert
        queryResult.Should().BeEmpty();
    }

    [Fact]
    public void Execute_ShouldReturnEmptyStringForNullColumn_WhenDatabaseContainsNullValue()
    {
        // Arrange
        var query = new Query()
            .From("Students")
            .Where("Age", "22");

        var compiledQuery = new QueryCompiler(
            new SqlClauseCompiler_PostgresDB())
            .Compile(query);

        // Act
        var queryResult = sut.Execute(compiledQuery);

        // Assert
        queryResult.Should().HaveCount(1);
        queryResult.First()["Name"].Should().Be(string.Empty);
    }

    [Fact]
    public void Execute_ShouldFindRecordExactly_WhenWhereValueContainsSpecialCharacters()
    {
        // Arrange
        var query = new Query()
            .From("Users")
            .Where("Username", "' OR 1=1 --");

        var compiledQuery = new QueryCompiler(
            new SqlClauseCompiler_PostgresDB())
            .Compile(query);

        // Act
        var queryResult = sut.Execute(compiledQuery);

        // Assert
        queryResult.Should().HaveCount(1);
        queryResult.First()["Id"].Should().Be("1");
    }

    [Fact]
    public void Execute_ShouldReturnOnlyMatchingRows_WhenFilteringByNumericColumn()
    {
        // Arrange
        var query = new Query()
            .From("Students")
            .Where("Age", "20");

        var compiledQuery = new QueryCompiler(
            new SqlClauseCompiler_PostgresDB())
            .Compile(query);

        // Act
        var queryResult = sut.Execute(compiledQuery);

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
                        { "IsMale", "True" }
                    },
                    new Dictionary<string, string>
                    {
                        { "Id", "3" },
                        { "Name", "Sara" },
                        { "Age", "20" },
                        { "IsMale", "False" }
                    }
                });
    }

    [Fact]
    public void Execute_ShouldReturnOnlyMatchingRows_WhenFilteringByBooleanColumn()
    {
        // Arrange
        var query = new Query()
            .From("Employees")
            .Where("IsActive", "true");

        var compiledQuery = new QueryCompiler(
            new SqlClauseCompiler_PostgresDB())
            .Compile(query);

        // Act
        var queryResult = sut.Execute(compiledQuery);

        // Assert
        queryResult.Should().HaveCount(2);
    }
}