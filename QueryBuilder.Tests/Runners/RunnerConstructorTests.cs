using System;
using QueryBuilder.Runners;
using Xunit;

public class RunnerConstructorTests
{
    [Fact]
    public void PostgresRunner_ShouldThrow_WhenConnectionStringIsNull() =>
        Assert.Throws<ArgumentNullException>(() => new PostgresRunner(null!, new ParameterBinder()));

    [Fact]
    public void PostgresRunner_ShouldThrow_WhenBinderIsNull() =>
        Assert.Throws<ArgumentNullException>(() => new PostgresRunner("cs", null!));

    [Fact]
    public void SqlServerRunner_ShouldThrow_WhenConnectionStringIsNull() =>
        Assert.Throws<ArgumentNullException>(() => new SqlServerRunner(null!, new ParameterBinder()));

    [Fact]
    public void SqlServerRunner_ShouldThrow_WhenBinderIsNull() =>
        Assert.Throws<ArgumentNullException>(() => new SqlServerRunner("cs", null!));
}