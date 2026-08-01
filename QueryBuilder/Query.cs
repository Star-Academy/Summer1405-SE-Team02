using System.Collections.Generic;

public class WhereCondition
{
    public string Column { get; set; }
    public object Value { get; set; }

    public WhereCondition(string column, object value)
    {
        Column = column;
        Value = value;
    }
}

public class Query
{
    public string? TableName { get; private set; }
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

    public Query Where(string column, object value)
    {
        Conditions.Add(new WhereCondition(column, value));
        return this;
    }
}