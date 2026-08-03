using System;
using System.Collections.Generic;
using System.IO;
using QueryBuilder.Models;
using QueryBuilder.Compilers;
using QueryBuilder.Runners;
using QueryBuilder.Constants;
using QueryBuilder.Infrastructure;
using QueryBuilder.Presentation;

namespace QueryBuilder
{
    public class Program
    {
        public static void Main()
        {
            EnvironmentLoader.LoadEnvironmentVariables();

            var postgresConn = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING");
            var sqlServerConn = Environment.GetEnvironmentVariable("SQLSERVER_CONNECTION_STRING");

            if (string.IsNullOrWhiteSpace(postgresConn) || string.IsNullOrWhiteSpace(sqlServerConn))
            {
                Console.WriteLine(Messages.Dictionary["EnvError"]);
                return;
            }

            var myQuery = new Query()
                .From("student")
                .Select("firstname", "lastname", "grade")
                .Where("ismale", "true");

            var parameterBinder = new DefaultParameterBinder();

            var postgresClauseCompiler = new SqlClauseCompiler_PostgresDB();
            var postgresQueryCompiler = new QueryCompiler(postgresClauseCompiler);
            var postgresResult = postgresQueryCompiler.Compile(myQuery);
            var postgresRunner = new PostgresRunner(postgresConn, parameterBinder);

            Console.WriteLine(Messages.Dictionary["PostgresHeader"]);
            Console.WriteLine(string.Format(Messages.Dictionary["GeneratedSql"], postgresResult.RawSql));

            var postgresData = postgresRunner.Execute(postgresResult);
            ConsoleResultPrinter.PrintResults(postgresData);

            Console.WriteLine();

            var sqlServerClauseCompiler = new SqlClauseCompiler_SqlServerDB();
            var sqlServerQueryCompiler = new QueryCompiler(sqlServerClauseCompiler);
            var sqlServerResult = sqlServerQueryCompiler.Compile(myQuery);
            var sqlServerRunner = new SqlServerRunner(sqlServerConn, parameterBinder);

            Console.WriteLine(Messages.Dictionary["SqlServerHeader"]);
            Console.WriteLine(string.Format(Messages.Dictionary["GeneratedSql"], sqlServerResult.RawSql));

            var sqlServerData = sqlServerRunner.Execute(sqlServerResult);
            ConsoleResultPrinter.PrintResults(sqlServerData);
        }
    }
}