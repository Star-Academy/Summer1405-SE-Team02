using System.Collections.Generic;
using QueryBuilder.Abstractions;
using QueryBuilder.Models;

namespace QueryBuilder.Compilers
{
    public class SqlClauseCompiler_SqlServerDB : ISqlClauseCompiler
    {
        public string CompileSelect(Query query)
        {
            var columnsClause = query.SelectedColumns.Count > 0
                ? string.Join(", ", query.SelectedColumns.ConvertAll(column => $"[{column}]"))
                : "*";

            return $"SELECT {columnsClause}";
        }

        public string CompileFrom(Query query)
        {
            return $"FROM [{query.TableName}]";
        }

        public string CompileWhere(Query query, Dictionary<string, string> bindings)
        {
            if (query.Conditions.Count == 0) return string.Empty;

            var whereClauses = new List<string>();
            for (var index = 0; index < query.Conditions.Count; index++)
            {
                var condition = query.Conditions[index];
                var parameterName = $"@p{index}";
                var formattedValue = FormatValueForSqlServer(condition.Value);

                whereClauses.Add($"[{condition.Column}] = {parameterName}");
                bindings.Add(parameterName, formattedValue);
            }

            return "WHERE " + string.Join(" AND ", whereClauses);
        }

        private string FormatValueForSqlServer(string rawValue)
        {
            if (bool.TryParse(rawValue, out var boolValue)) return boolValue ? "1" : "0";
            return rawValue;
        }
    }
}