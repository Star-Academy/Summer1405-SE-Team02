using Microsoft.AspNetCore.Mvc;
using QueryBuilderWebApplication.Abstractions;
using QueryBuilderWebApplication.Exceptions;
using QueryBuilderWebApplication.Models;
using QueryBuilderWebApplication.Services;

namespace QueryBuilderWebApplication.Controllers;

[ApiController]
[Route("[controller]")]
[ServiceFilter(typeof(Filters.ExceptionFilter))]
public class StudentsController : ControllerBase
{
    private readonly IDatabaseResolver _databaseResolver;

    public StudentsController(IDatabaseResolver databaseResolver)
    {
        _databaseResolver = databaseResolver;
    }

    [HttpGet]
    public IActionResult GetAll([FromQuery] string? db)
    {
        var result = _databaseResolver.Resolve(db);
        var repository = new StudentRepository(result.Factory);
        return Ok(repository.GetAll());
    }

    [HttpGet("{studentNumber:int}")]
    public IActionResult GetOne(int studentNumber, [FromQuery] string? db)
    {
        var result = _databaseResolver.Resolve(db);
        var repository = new StudentRepository(result.Factory);
        var student = repository.Get(studentNumber);

        if (student is null)
        {
            throw new StudentNotFoundException(studentNumber);
        }

        return Ok(student);
    }

    [HttpPost]
    public IActionResult Create([FromQuery] string? db, [FromBody] Student student)
    {
        var result = _databaseResolver.Resolve(db);
        var repository = new StudentRepository(result.Factory);

        var existing = repository.Get(student.StudentNumber);
        if (existing is not null)
        {
            throw new StudentAlreadyExistsException(student.StudentNumber);
        }

        repository.Add(student);
        return CreatedAtAction(nameof(GetOne), new { studentNumber = student.StudentNumber, db }, student);
    }

    [HttpPut("{studentNumber:int}")]
    public IActionResult Update(int studentNumber, [FromQuery] string? db, [FromBody] Student student)
    {
        var result = _databaseResolver.Resolve(db);
        var repository = new StudentRepository(result.Factory);
        var updated = repository.Update(studentNumber, student);

        if (!updated)
        {
            throw new StudentNotFoundException(studentNumber);
        }

        return NoContent();
    }

    [HttpDelete("{studentNumber:int}")]
    public IActionResult Delete(int studentNumber, [FromQuery] string? db)
    {
        var result = _databaseResolver.Resolve(db);
        var repository = new StudentRepository(result.Factory);
        var deleted = repository.Delete(studentNumber);

        if (!deleted)
        {
            throw new StudentNotFoundException(studentNumber);
        }

        return NoContent();
    }
}