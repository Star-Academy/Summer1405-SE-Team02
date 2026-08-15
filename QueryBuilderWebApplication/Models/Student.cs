using System.ComponentModel.DataAnnotations;

namespace QueryBuilderWebApplication.Models;

public class Student
{
    [Required]
    public int StudentNumber { get; set; }

    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required]
    public int Grade { get; set; }

    [Required]
    public bool IsMale { get; set; }
}