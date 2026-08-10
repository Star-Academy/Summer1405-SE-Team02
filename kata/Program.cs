using Microsoft.Data.SqlClient;
using SqlKata;
using SqlKata.Compilers;
using SqlKata.Execution;

var compiler = new SqlServerCompiler();

var query = new Query("students")
    .Join("employee", "students.id", "employee")
    .Select("firstname", "lastname", "grade")
    .Where("ismale", 15)
    .OrderByDesc("students.firstname")
    .Limit(53)
    .Offset(15);

var result = compiler.Compile(query);

Console.WriteLine(result.Sql);
foreach (var binding in result.Bindings)
{
    Console.WriteLine(binding);
}
var connection = new SqlConnection(
    "Data Source=localhost,5000;Initial Catalog=StarAcademy;User Id=sa;Password=Your_strong_Password123;Encrypt=True;TrustServerCertificate=True"
);
var db = new QueryFactory(connection, compiler);

var user = db.Query("Student").Where("Firstname", "AAA").Where("IsMale", 1).Get();
foreach (var s in user)
{
    Console.WriteLine(
        $"{s.StudentNumber} | {s.FirstName} {s.LastName} | Grade: {s.Grade} | IsMale: {s.IsMale}"
    );
}
