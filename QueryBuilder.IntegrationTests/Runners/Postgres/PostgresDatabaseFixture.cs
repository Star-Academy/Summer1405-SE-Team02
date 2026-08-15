using Npgsql;
using Testcontainers.PostgreSql;
using Xunit;

namespace QueryBuilder.IntegrationTests.Runners.Postgres;

public sealed class PostgresDatabaseFixture : IAsyncLifetime
{
    public PostgreSqlContainer Container { get; private set; } = null!;

    public string ConnectionString => Container.GetConnectionString();

    public async Task InitializeAsync()
    {
        Container = new PostgreSqlBuilder("postgres:16-alpine")
            .WithDatabase("postgresDB")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        await Container.StartAsync();

        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(
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

            CREATE TABLE "Users" (
                "Id" INT,
                "Username" TEXT
            );

            INSERT INTO "Users" VALUES (1, ''' OR 1=1 --');
            INSERT INTO "Users" VALUES (2, 'NormalUser');

            CREATE TABLE "Employees" (
                "Id" INT,
                "Name" TEXT,
                "IsActive" BOOLEAN
            );

            INSERT INTO "Employees" VALUES (1, 'Ali', true);
            INSERT INTO "Employees" VALUES (2, 'Reza', false);
            INSERT INTO "Employees" VALUES (3, 'Sara', true);
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