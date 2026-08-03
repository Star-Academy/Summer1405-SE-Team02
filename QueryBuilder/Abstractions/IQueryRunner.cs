using System.Collections.Generic;
using QueryBuilder.Models;

namespace QueryBuilder.Abstractions
{
    public interface IQueryRunner
    {
        List<Dictionary<string, string>> Execute(CompiledQuery compiledQuery);
    }
}