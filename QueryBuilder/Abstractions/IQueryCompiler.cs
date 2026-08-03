using QueryBuilder.Models;

namespace QueryBuilder.Abstractions
{
    public interface IQueryCompiler
    {
        CompiledQuery Compile(Query query);
    }
}