using System.Collections.Generic;

namespace QueryBuilder.Constants
{
    public static class Messages
    {
        public static readonly Dictionary<string, string> Dictionary = new Dictionary<string, string>
        {
            { "EnvError", "Error: Connection strings are not properly set in the .env file." },
            { "PostgresHeader", "--- Executing on PostgreSQL ---" },
            { "SqlServerHeader", "--- Executing on SQL Server ---" },
            { "GeneratedSql", "Generated SQL: {0}" }
        };
    }
}