using System;
using System.Text;
using QueryBuilder.Abstractions;
using QueryBuilder.Models;

namespace QueryBuilder.Compilers
{
    public class QueryCompiler : IQueryCompiler
    {
        private readonly ISqlClauseCompiler _sqlClauseCompiler;

        public QueryCompiler(ISqlClauseCompiler sqlClauseCompiler)
        {
            _sqlClauseCompiler = sqlClauseCompiler ?? throw new ArgumentNullException(nameof(sqlClauseCompiler));
        }

        public CompiledQuery Compile(Query query)
        {
            var result = new CompiledQuery();
            var sqlBuilder = new StringBuilder();

            sqlBuilder.Append(_sqlClauseCompiler.CompileSelect(query)).Append(" ");
            sqlBuilder.Append(_sqlClauseCompiler.CompileFrom(query)).Append(" ");

            var whereClause = _sqlClauseCompiler.CompileWhere(query, result.Bindings);
            if (!string.IsNullOrWhiteSpace(whereClause))
            {
                sqlBuilder.Append(whereClause);
            }

            result.RawSql = sqlBuilder.ToString().Trim();
            return result;
        }
    }
}