using System;
using FluentAssertions;
using NSubstitute;
using QueryBuilder.Abstractions;
using QueryBuilder.Runners;
using Xunit;


public class RunnerConstructorTests
{
    [Fact]
    public void PostgresRunner_ShouldThrow_WhenConnectionStringIsNull()
    {
        // Arrange
        // Act
        Action act = () => new PostgresRunner(
            null!,
            Substitute.For<IParameterBinder>(),
            Substitute.For<IDataReaderFormatter>());

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("connectionString");
    }

    [Fact]
    public void PostgresRunner_ShouldThrow_WhenBinderIsNull()
    {
        // Arrange 
        // Act
        Action act = () => new PostgresRunner(
            "cs",
            null!,
            Substitute.For<IDataReaderFormatter>());

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("parameterBinder");
    }

    [Fact]
    public void PostgresRunner_ShouldThrow_WhenFormatterIsNull()
    {
        // Arrange 
        // Act
        Action act = () => new PostgresRunner(
            "cs",
            Substitute.For<IParameterBinder>(),
            null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("formatter");
    }

    [Fact]
    public void SqlServerRunner_ShouldThrow_WhenConnectionStringIsNull()
    {
        // Arrange 
        // Act
        Action act = () => new SqlServerRunner(
            null!,
            Substitute.For<IParameterBinder>(),
            Substitute.For<IDataReaderFormatter>());

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("connectionString");
    }

    [Fact]
    public void SqlServerRunner_ShouldThrow_WhenBinderIsNull()
    {
        // Arrange 
        // Act
        Action act = () => new SqlServerRunner(
            "cs",
            null!,
            Substitute.For<IDataReaderFormatter>());

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("parameterBinder");
    }

    [Fact]
    public void SqlServerRunner_ShouldThrow_WhenFormatterIsNull()
    {
        // Arrange 
        // Act
        Action act = () => new SqlServerRunner(
            "cs",
            Substitute.For<IParameterBinder>(),
            null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("formatter");
    }
}