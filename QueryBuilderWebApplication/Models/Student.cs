namespace QueryBuilderWebApplication.Models;

public class Student
{
    public int StudentNumber { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int Grade { get; set; }
    public bool IsMale { get; set; }
}