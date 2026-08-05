using System;
using System.Collections.Generic;
using System.Data;
using NSubstitute;
using QueryBuilder.Runners;
using Xunit;

namespace QueryBuilder.Tests.Runners
{
    public class ParameterBinderTests
    {
        private readonly IDbCommand _command;
        private readonly IDbDataParameter _dbParameter;
        private readonly IDataParameterCollection _parameterCollection;
        private readonly ParameterBinder _binder;

        public ParameterBinderTests()
        {
            _command = Substitute.For<IDbCommand>();
            _dbParameter = Substitute.For<IDbDataParameter>();
            _parameterCollection = Substitute.For<IDataParameterCollection>();

            _command.CreateParameter().Returns(_dbParameter);
            _command.Parameters.Returns(_parameterCollection);

            _binder = new ParameterBinder();
        }

        [Theory]
        [InlineData("true", true)]
        [InlineData("False", false)]
        public void BindParameters_ShouldBindTypedBool_WhenValueIsBooleanString(string rawValue, object expectedValue)
        {
            // Arrange
            var bindings = new Dictionary<string, string> { ["@p0"] = rawValue };

            // Act
            _binder.BindParameters(_command, bindings);

            // Assert
            _dbParameter.Received().ParameterName = "@p0";
            _dbParameter.Received().Value = expectedValue;
            _parameterCollection.Received().Add(_dbParameter);
        }

        [Theory]
        [InlineData("42", 42)]
        [InlineData("1", 1)]
        public void BindParameters_ShouldBindTypedInt_WhenValueIsIntegerString(string rawValue, object expectedValue)
        {
            // Arrange
            var bindings = new Dictionary<string, string> { ["@p0"] = rawValue };

            // Act
            _binder.BindParameters(_command, bindings);

            // Assert
            _dbParameter.Received().Value = expectedValue;
        }

        [Theory]
        [InlineData("Ali", "Ali")]
        [InlineData("12.5", "12.5")]
        public void BindParameters_ShouldBindRawString_WhenValueIsNotBoolOrInt(string rawValue, object expectedValue)
        {
            // Arrange
            var bindings = new Dictionary<string, string> { ["@p0"] = rawValue };

            // Act
            _binder.BindParameters(_command, bindings);

            // Assert
            _dbParameter.Received().Value = expectedValue;
        }

        [Fact]
        public void BindParameters_ShouldNotCreateParameters_WhenBindingsIsEmpty()
        {
            // Arrange
            var bindings = new Dictionary<string, string>();

            // Act
            _binder.BindParameters(_command, bindings);

            // Assert
            _command.DidNotReceive().CreateParameter();
            _parameterCollection.DidNotReceive().Add(Arg.Any<object>());
        }

        [Fact]
        public void BindParameters_ShouldCreateParameterPerBinding_WhenMultipleBindingsExist()
        {
            // Arrange
            var bindings = new Dictionary<string, string>
            {
                ["@p0"] = "1",
                ["@p1"] = "true"
            };

            // Act
            _binder.BindParameters(_command, bindings);

            // Assert
            _command.Received(2).CreateParameter();
            _parameterCollection.Received(2).Add(_dbParameter);
        }

        [Fact]
        public void BindParameters_ShouldThrowArgumentNullException_WhenCommandIsNull()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(
                () => _binder.BindParameters(null!, new Dictionary<string, string>()));
            Assert.Equal("command", exception.ParamName);
        }

        [Fact]
        public void BindParameters_ShouldThrowArgumentNullException_WhenBindingsIsNull()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(
                () => _binder.BindParameters(_command, null!));
            Assert.Equal("bindings", exception.ParamName);
        }
    }
}