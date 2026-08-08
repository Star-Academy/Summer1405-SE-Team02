using System.Collections.Generic;
using System.Data;
using QueryBuilder.Abstractions;

namespace QueryBuilder.Runners
{
    internal sealed class DataReaderFormatter : IDataReaderFormatter
    {
        public List<Dictionary<string, string>> Format(IDataReader reader)
        {
            var results = new List<Dictionary<string, string>>();

            while (reader.Read())
            {
                var row = new Dictionary<string, string>();

                for (var index = 0; index < reader.FieldCount; index++)
                {
                    var columnName = reader.GetName(index);
                    var columnValue = reader.GetValue(index)?.ToString() ?? string.Empty;
                    row.Add(columnName, columnValue);
                }

                results.Add(row);
            }

            return results;
        }
    }
}