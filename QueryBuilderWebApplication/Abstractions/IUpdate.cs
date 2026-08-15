using QueryBuilderWebApplication.Models;

namespace QueryBuilderWebApplication.Abstractions;

public interface IUpdate
{
    Task<bool> UpdateAsync(Student student);
}