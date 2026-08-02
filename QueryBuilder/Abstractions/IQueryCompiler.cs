using QueryBuilder.Models;

namespace QueryBuilder.Abstractions
{
    public interface IQueryCompiler
    {
        SqlResult Compile(Query query);
    }
}