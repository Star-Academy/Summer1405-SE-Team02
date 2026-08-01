using System.Text;

public interface ICompiler
{
    SqlResult Compile(Query query);
}

public abstract class AbstractCompiler : ICompiler
{
    public SqlResult Compile(Query query)
    {
        var result = new SqlResult();
        var sql = new StringBuilder();

        sql.Append(CompileSelect(query)).Append(" ");
        sql.Append(CompileFrom(query)).Append(" ");

        string whereClause = CompileWhere(query, result.Bindings);
        if (!string.IsNullOrEmpty(whereClause))
        {
            sql.Append(whereClause);
        }

        result.Sql = sql.ToString().Trim();
        return result;
    }

    protected abstract string CompileSelect(Query query);
    protected abstract string CompileFrom(Query query);
    protected abstract string CompileWhere(Query query, Dictionary<string, object> bindings);
}