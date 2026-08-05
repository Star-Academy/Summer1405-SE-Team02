using System.Collections.Generic;
using QueryBuilder.Models;

namespace QueryBuilder.Abstractions
{
    public interface ISqlClauseCompiler
    {
        string CompileSelect(Query query);
        string CompileFrom(Query query);
        string CompileWhere(Query query, Dictionary<string, string> bindings);
    }
}