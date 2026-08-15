using Microsoft.Data.SqlClient;
using Testcontainers.MsSql;
using Xunit;

namespace QueryBuilder.IntegrationTests.Runners.SqlServer;

public sealed class SqlServerDatabaseFixture : IAsyncLifetime
{
    public MsSqlContainer Container { get; private set; } = null!;

    public string ConnectionString => Container.GetConnectionString();

    public async Task InitializeAsync()
    {
        Container = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("YourStrong!Passw0rd")
            .Build();

        await Container.StartAsync();

        await using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();

        await using var command = new SqlCommand(
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
            INSERT INTO Students VALUES (4, N'Maryam', 30, 0);
            INSERT INTO Students VALUES (5, NULL, 22, 1);

            CREATE TABLE Users (
                Id INT,
                Username NVARCHAR(100)
            );

            INSERT INTO Users VALUES (1, N''' OR 1=1 --');
            INSERT INTO Users VALUES (2, N'NormalUser');

            CREATE TABLE Employees (
                Id INT,
                Name NVARCHAR(100),
                IsActive BIT
            );

            INSERT INTO Employees VALUES (1, N'Ali', 1);
            INSERT INTO Employees VALUES (2, N'Reza', 0);
            INSERT INTO Employees VALUES (3, N'Sara', 1);
            """,
            connection);

        await command.ExecuteNonQueryAsync();
    }

    public async Task DisposeAsync()
    {
        if (Container is not null)
        {
            await Container.DisposeAsync();
        }
    }
}