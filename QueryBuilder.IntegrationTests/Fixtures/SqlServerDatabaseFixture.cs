using Testcontainers.MsSql;
using Xunit;

namespace QueryBuilder.IntegrationTests.Fixtures;

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
    }

    public async Task DisposeAsync()
    {
        if (Container is not null)
        {
            await Container.DisposeAsync();
        }
    }
}
