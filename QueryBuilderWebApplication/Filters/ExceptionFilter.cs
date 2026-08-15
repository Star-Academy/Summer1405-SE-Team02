using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using QueryBuilderWebApplication.Exceptions;

namespace QueryBuilderWebApplication.Filters;

public class ExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        switch (context.Exception)
        {
            case InvalidDatabaseException ex:
                context.Result = new BadRequestObjectResult(new { error = ex.Message });
                context.ExceptionHandled = true;
                break;

            case StudentNotFoundException ex:
                context.Result = new NotFoundObjectResult(new { error = ex.Message });
                context.ExceptionHandled = true;
                break;

            case StudentAlreadyExistsException ex:
                context.Result = new ConflictObjectResult(new { error = ex.Message });
                context.ExceptionHandled = true;
                break;

            case Microsoft.Data.SqlClient.SqlException ex when ex.Number == 2627:
                context.Result = new ConflictObjectResult(new { error = "Student with this number already exists." });
                context.ExceptionHandled = true;
                break;

            case Npgsql.PostgresException ex when ex.SqlState == "23505":
                context.Result = new ConflictObjectResult(new { error = "Student with this number already exists." });
                context.ExceptionHandled = true;
                break;
        }
    }
}