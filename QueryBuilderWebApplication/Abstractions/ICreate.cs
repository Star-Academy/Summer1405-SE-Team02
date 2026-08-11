using QueryBuilderWebApplication.Models;

namespace QueryBuilderWebApplication.Abstractions;

public interface ICreate
{
    Task<Student> CreateAsync(Student student);
}