using System;
using System.Collections.Generic;
using System.Data;
using FluentAssertions;
using NSubstitute;
using QueryBuilder.Runners;
using Xunit;


public class ParameterBinderTests
{
    [Fact]
    public void BindParameters_ShouldThrow_WhenCommandIsNull()
    {
        // Arrange
        var sut = new ParameterBinder();

        // Act
        Action act = () => sut.BindParameters(null!, new Dictionary<string, string>());

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("command");
    }

    [Fact]
    public void BindParameters_ShouldThrow_WhenBindingsIsNull()
    {
        // Arrange
        var sut = new ParameterBinder();
        var command = Substitute.For<IDbCommand>();

        // Act
        Action act = () => sut.BindParameters(command, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("bindings");
    }

    [Fact]
    public void BindParameters_ShouldConvertIntValues_Whenever()
    {
        // Arrange
        var sut = new ParameterBinder();
        var (command, parameters, created) = CreateFixture();

        // Act
        sut.BindParameters(command, new Dictionary<string, string>
        {
            ["age"] = "30"
        });

        // Assert
        var parameter = created[0];
        parameter.ParameterName.Should().Be("age");
        parameter.Value.Should().BeOfType<int>();
        parameter.Value.Should().Be(30);
        parameters.Received(1).Add(parameter);
    }

    [Fact]
    public void BindParameters_ShouldConvertBoolValues_Whenever()
    {
        // Arrange
        var sut = new ParameterBinder();
        var (command, _, created) = CreateFixture();

        // Act
        sut.BindParameters(command, new Dictionary<string, string>
        {
            ["active"] = "true"
        });

        // Assert
        var parameter = created[0];
        parameter.Value.Should().BeOfType<bool>();
        parameter.Value.Should().Be(true);
    }

    [Fact]
    public void BindParameters_ShouldKeepStringValues_Whenever()
    {
        // Arrange
        var sut = new ParameterBinder();
        var (command, _, created) = CreateFixture();

        // Act
        sut.BindParameters(command, new Dictionary<string, string>
        {
            ["name"] = "Ali"
        });

        // Assert
        created[0].Value.Should().Be("Ali");
    }

    private static (IDbCommand Command, IDataParameterCollection Parameters, List<IDbDataParameter> Created) CreateFixture()
    {
        var command = Substitute.For<IDbCommand>();
        var parameters = Substitute.For<IDataParameterCollection>();
        var created = new List<IDbDataParameter>();

        command.Parameters.Returns(parameters);
        command.CreateParameter().Returns(_ =>
        {
            var parameter = Substitute.For<IDbDataParameter>();
            created.Add(parameter);
            return parameter;
        });

        return (command, parameters, created);
    }
}