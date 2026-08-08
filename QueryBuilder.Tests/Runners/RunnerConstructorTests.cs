using System;
using QueryBuilder.Runners;
using Xunit;

public class RunnerConstructorTests
{
    [Fact]
    public void PostgresRunner_Should_Throw_When_ConnectionString_Is_Null() =>
        Assert.Throws<ArgumentNullException>(() => new PostgresRunner(null!, new ParameterBinder()));

    [Fact]
    public void PostgresRunner_Should_Throw_When_Binder_Is_Null() =>
        Assert.Throws<ArgumentNullException>(() => new PostgresRunner("cs", null!));

    [Fact]
    public void SqlServerRunner_Should_Throw_When_ConnectionString_Is_Null() =>
        Assert.Throws<ArgumentNullException>(() => new SqlServerRunner(null!, new ParameterBinder()));

    [Fact]
    public void SqlServerRunner_Should_Throw_When_Binder_Is_Null() =>
        Assert.Throws<ArgumentNullException>(() => new SqlServerRunner("cs", null!));
}