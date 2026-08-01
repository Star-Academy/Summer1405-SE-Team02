using System.Collections.Generic;
using System.Text;

public class PostgresCompiler : AbstractCompiler
{
    protected override string CompileSelect(Query query)
    {
        string cols = query.SelectedColumns.Count > 0
            ? string.Join(", ", query.SelectedColumns.ConvertAll(c => $"\"{c}\""))
            : "*";
        return $"SELECT {cols}";
    }

    protected override string CompileFrom(Query query)
    {
        return $"FROM \"{query.TableName}\"";
    }

    protected override string CompileWhere(Query query, Dictionary<string, object> bindings)
    {
        if (query.Conditions.Count == 0) return string.Empty;

        var whereClauses = new List<string>();
        for (int i = 0; i < query.Conditions.Count; i++)
        {
            var condition = query.Conditions[i];
            string paramName = $"${i + 1}";

            whereClauses.Add($"\"{condition.Column}\" = {paramName}");
            bindings.Add(paramName, condition.Value);
        }
        return "WHERE " + string.Join(" AND ", whereClauses);
    }
}