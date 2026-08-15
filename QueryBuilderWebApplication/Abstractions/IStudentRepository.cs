using QueryBuilderWebApplication.Models;

namespace QueryBuilderWebApplication.Abstractions;

public interface IStudentRepository
{
    IReadOnlyList<Student> GetAll();

    Student? Get(int studentNumber);

    void Add(Student student);

    bool Update(int studentNumber, Student student);

    bool Delete(int studentNumber);
}