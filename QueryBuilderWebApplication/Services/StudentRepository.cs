using QueryBuilderWebApplication.Abstractions;
using QueryBuilderWebApplication.Models;
using SqlKata.Execution;

namespace QueryBuilderWebApplication.Services;

public class StudentRepository : IStudentRepository
{
    private const string TableName = "Students";

    private readonly QueryFactory _db;

    public StudentRepository(QueryFactory db)
    {
        _db = db;
    }

    public IReadOnlyList<Student> GetAll()
    {
        return _db.Query(TableName).Get<Student>().ToList();
    }

    public Student? Get(int studentNumber)
    {
        return _db.Query(TableName)
            .Where(nameof(Student.StudentNumber), studentNumber)
            .FirstOrDefault<Student>();
    }

    public void Add(Student student)
    {
        _db.Query(TableName).Insert(student);
    }

    public bool Update(int studentNumber, Student student)
    {
        var affectedRows = _db.Query(TableName)
            .Where(nameof(Student.StudentNumber), studentNumber)
            .Update(new
            {
                student.FirstName,
                student.LastName,
                student.Grade,
                student.IsMale
            });

        return affectedRows > 0;
    }

    public bool Delete(int studentNumber)
    {
        var affectedRows = _db.Query(TableName)
            .Where(nameof(Student.StudentNumber), studentNumber)
            .Delete();

        return affectedRows > 0;
    }
}