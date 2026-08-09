using System.Data;
using FluentAssertions;
using NSubstitute;
using QueryBuilder.Runners;
using Xunit;

public class DataReaderFormatterTests
{
    [Fact]
    public void Format_ShouldReturnEmptyList_WhenNoRows()
    {
        // Arrange
        var sut = new DataReaderFormatter();
        var reader = Substitute.For<IDataReader>();
        reader.Read().Returns(false);

        // Act
        var result = sut.Format(reader);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void Format_ShouldConvertRowsToDictionaries_Whenever()
    {
        // Arrange
        var sut = new DataReaderFormatter();
        var reader = Substitute.For<IDataReader>();
        reader.Read().Returns(true, true, false);
        reader.FieldCount.Returns(2);
        reader.GetName(0).Returns("id");
        reader.GetName(1).Returns("name");
        reader.GetValue(0).Returns(7);
        reader.GetValue(1).Returns("Ali");

        // Act
        var result = sut.Format(reader);

        // Assert
        result.Should().HaveCount(2);
        result[0]["id"].Should().Be("7");
        result[0]["name"].Should().Be("Ali");
    }

    [Fact]
    public void Format_ShouldUseEmptyStringForNullValues_Whenever()
    {
        // Arrange
        var sut = new DataReaderFormatter();
        var reader = Substitute.For<IDataReader>();
        reader.Read().Returns(true, false);
        reader.FieldCount.Returns(1);
        reader.GetName(0).Returns("col");
        reader.GetValue(0).Returns(null!);

        // Act
        var result = sut.Format(reader);

        // Assert
        result[0]["col"].Should().BeEmpty();
    }
}