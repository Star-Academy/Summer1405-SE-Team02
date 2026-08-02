using System;
using QueryBuilder.Models;
using QueryBuilder.Compilers;
using QueryBuilder.Runners;

namespace QueryBuilder
{
    public class Program
    {
        public static void Main()
        {
            Environment.SetEnvironmentVariable("POSTGRES_CONNECTION_STRING", "Host=localhost;Port=5432;Database=mohaymen;Username=postgres;Password=postgres");
            Environment.SetEnvironmentVariable("SQLSERVER_CONNECTION_STRING", "Server=localhost,1433;Database=master;User Id=sa;Password=Your_strong_Password123;TrustServerCertificate=True;");

            var myQuery = new Query()
                .From("student")
                .Select("firstname", "lastname", "grade")
                .Where("ismale", "true");

            var postgresClauseCompiler = new PostgresCompiler();
            var postgresQueryCompiler = new BaseQueryCompiler(postgresClauseCompiler);

            var postgresResult = postgresQueryCompiler.Compile(myQuery);
            var postgresRunner = new PostgresRunner();
            postgresRunner.Execute(postgresResult);

            Console.WriteLine();

            var sqlServerClauseCompiler = new SqlServerCompiler();
            var sqlServerQueryCompiler = new BaseQueryCompiler(sqlServerClauseCompiler);

            var sqlServerResult = sqlServerQueryCompiler.Compile(myQuery);
            var sqlServerRunner = new SqlServerRunner();
            sqlServerRunner.Execute(sqlServerResult);
        }
    }
}