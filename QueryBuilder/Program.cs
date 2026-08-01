using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.SqlClient;
using Npgsql;

var myQuery = new Query()
    .From("student")
    .Select("firstname", "lastname", "grade")
    .Where("ismale", true);

ICompiler pgCompiler = new PostgresCompiler();
var pgResult = pgCompiler.Compile(myQuery);

Console.WriteLine("--- Executing on PostgreSQL ---");
Console.WriteLine("Generated SQL: " + pgResult.Sql);

string pgConnString = "Host=localhost;Port=5432;Database=mohaymen;Username=postgres;Password=postgres";
using (var pgConn = new NpgsqlConnection(pgConnString))
{
    pgConn.Open();
    using (var cmd = new NpgsqlCommand(pgResult.Sql, pgConn))
    {
        foreach (var param in pgResult.Bindings)
        {
            cmd.Parameters.Add(new NpgsqlParameter { Value = param.Value });
        }

        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    Console.Write($"{reader.GetName(i)}: {reader.GetValue(i)} | ");
                }
                Console.WriteLine();
            }
        }
    }
}

Console.WriteLine();

ICompiler sqlCompiler = new SqlServerCompiler();
var sqlResult = sqlCompiler.Compile(myQuery);

Console.WriteLine("--- Executing on SQL Server ---");
Console.WriteLine("Generated SQL: " + sqlResult.Sql);

string sqlConnString = "Server=localhost,1433;Database=master;User Id=sa;Password=Your_strong_Password123;TrustServerCertificate=True;";
using (var sqlConn = new SqlConnection(sqlConnString))
{
    sqlConn.Open();
    using (var cmd = new SqlCommand(sqlResult.Sql, sqlConn))
    {
        foreach (var param in sqlResult.Bindings)
        {
            cmd.Parameters.AddWithValue(param.Key, param.Value);
        }

        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    Console.Write($"{reader.GetName(i)}: {reader.GetValue(i)} | ");
                }
                Console.WriteLine();
            }
        }
    }
}