using QueryBuilderWebApplication.Models;

namespace QueryBuilderWebApplication.Abstractions;

public interface IGetOne
{
    Task<Student?> GetOneAsync(int studentNumber);
}