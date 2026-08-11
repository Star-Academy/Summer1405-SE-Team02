using QueryBuilderWebApplication.Models;

namespace QueryBuilderWebApplication.Abstractions;

public interface IListMaker
{
    Task<IReadOnlyList<Student>> ListMakerAsync();
}