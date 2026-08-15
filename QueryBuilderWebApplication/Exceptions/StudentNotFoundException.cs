namespace QueryBuilderWebApplication.Exceptions;

public class StudentNotFoundException : Exception
{
    public StudentNotFoundException(int studentNumber)
        : base($"Student {studentNumber} not found.")
    {
    }
}