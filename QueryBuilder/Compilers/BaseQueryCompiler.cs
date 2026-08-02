using System.Text;
using QueryBuilder.Abstractions;
using QueryBuilder.Models;

namespace QueryBuilder.Compilers
{
    public class BaseQueryCompiler : IQueryCompiler
    {
        private readonly ISqlClauseCompiler _clauseCompiler;

        public BaseQueryCompiler(ISqlClauseCompiler clauseCompiler)
        {
            _clauseCompiler = clauseCompiler;
        }

        public SqlResult Compile(Query query)
        {
            var result = new SqlResult();
            var sqlBuilder = new StringBuilder();

            sqlBuilder.Append(_clauseCompiler.CompileSelect(query)).Append(" ");
            sqlBuilder.Append(_clauseCompiler.CompileFrom(query)).Append(" ");

            var whereClause = _clauseCompiler.CompileWhere(query, result.Bindings);
            if (!string.IsNullOrEmpty(whereClause))
            {
                sqlBuilder.Append(whereClause);
            }

            result.Sql = sqlBuilder.ToString().Trim();
            return result;
        }
    }
}