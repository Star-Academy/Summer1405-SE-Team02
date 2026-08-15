using SqlKata.Execution;

namespace QueryBuilderWebApplication.Abstractions;

public interface IDbFactory
{
    QueryFactory Create(string databaseName);
}