using System.Collections.Generic;

namespace QueryBuilder.Models
{
    public class Query
    {
        public string TableName { get; private set; } = string.Empty;
        public List<string> SelectedColumns { get; private set; } = new List<string>();
        public List<WhereCondition> Conditions { get; private set; } = new List<WhereCondition>();

        public Query From(string table)
        {
            TableName = table;
            return this;
        }

        public Query Select(params string[] columns)
        {
            SelectedColumns.AddRange(columns);
            return this;
        }

        public Query Where(string column, string value)
        {
            Conditions.Add(new WhereCondition(column, value));
            return this;
        }
    }
}