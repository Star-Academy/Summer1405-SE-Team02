namespace QueryBuilderWebApplication.Exceptions;

public class InvalidDatabaseException : Exception
{
    public InvalidDatabaseException(string databaseName)
        : base($"Unsupported database '{databaseName}'. Use 'postgres' or 'sqlserver'.")
    {
    }
}