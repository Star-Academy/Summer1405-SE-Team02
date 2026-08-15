using QueryBuilderWebApplication.Models;

namespace QueryBuilderWebApplication.Abstractions;

public interface IDatabaseResolver
{
    DatabaseResolverResult Resolve(string? databaseName);
}