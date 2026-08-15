namespace QueryBuilderWebApplication.Exceptions;

public class StudentAlreadyExistsException : Exception
{
    public StudentAlreadyExistsException(int studentNumber)
        : base($"Student with number {studentNumber} already exists.")
    {
    }
}