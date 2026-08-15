using Microsoft.AspNetCore.Mvc;
using QueryBuilderWebApplication.Abstractions;
using QueryBuilderWebApplication.Models;
using SqlKata.Execution;

namespace QueryBuilderWebApplication.Controllers;

[ApiController]
[Route("[controller]")]
public class StudentsController : ControllerBase
{
    private const string Table = "Students";

    private readonly IDbFactory _dbFactory;

    public StudentsController(IDbFactory dbFactory)
    {
        _dbFactory = dbFactory;
    }

    [HttpGet]
    public IActionResult GetAll([FromQuery] string? db)
    {
        var (factory, error) = ResolveDb(db);
        if (error is not null) return error;
        return Ok(factory!.Query(Table).Get<Student>());
    }

    [HttpGet("{studentNumber:int}")]
    public IActionResult GetOne(int studentNumber, [FromQuery] string? db)
    {
        var (factory, error) = ResolveDb(db);
        if (error is not null) return error;

        var student = factory!.Query(Table)
            .Where(nameof(Student.StudentNumber), studentNumber)
            .FirstOrDefault<Student>();

        return student is null
            ? NotFound(new { error = $"Student {studentNumber} not found." })
            : Ok(student);
    }

    [HttpPost]
    public IActionResult Create([FromQuery] string? db, [FromBody] Student student)
    {
        var (factory, error) = ResolveDb(db);
        if (error is not null) return error;

        factory!.Query(Table).Insert(student);
        return CreatedAtAction(nameof(GetOne),
            new { studentNumber = student.StudentNumber, db }, student);
    }

    [HttpPut("{studentNumber:int}")]
    public IActionResult Update(int studentNumber, [FromQuery] string? db, [FromBody] Student student)
    {
        var (factory, error) = ResolveDb(db);
        if (error is not null) return error;

        var affected = factory!.Query(Table)
            .Where(nameof(Student.StudentNumber), studentNumber)
            .Update(new { student.FirstName, student.LastName, student.Grade, student.IsMale });

        return affected == 0
            ? NotFound(new { error = $"Student {studentNumber} not found." })
            : NoContent();
    }

    [HttpDelete("{studentNumber:int}")]
    public IActionResult Delete(int studentNumber, [FromQuery] string? db)
    {
        var (factory, error) = ResolveDb(db);
        if (error is not null) return error;

        var affected = factory!.Query(Table)
            .Where(nameof(Student.StudentNumber), studentNumber)
            .Delete();

        return affected == 0
            ? NotFound(new { error = $"Student {studentNumber} not found." })
            : NoContent();
    }

    private (QueryFactory? Factory, IActionResult? Error) ResolveDb(string? db)
    {
        try { return (_dbFactory.Create(db ?? string.Empty), null); }
        catch (InvalidDatabaseException ex) { return (null, BadRequest(new { error = ex.Message })); }
    }
}