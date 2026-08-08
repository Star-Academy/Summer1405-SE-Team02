using System.Data;
using NSubstitute;
using QueryBuilder.Runners;
using Xunit;

public class ResultSetMapperTests
{
    [Fact]
    public void Map_ShouldReturnEmptyList_WhenNoRows()
    {
        var reader = Substitute.For<IDataReader>();
        reader.Read().Returns(false);

        var result = ResultSetMapper.Map(reader);

        Assert.Empty(result);
    }

    [Fact]
    public void Map_ShouldConvertRowsToDictionaries_WhenCalled()
    {
        var reader = Substitute.For<IDataReader>();

        reader.Read().Returns(true, true, false);
        reader.FieldCount.Returns(2);
        reader.GetName(0).Returns("id");
        reader.GetName(1).Returns("name");
        reader.GetValue(0).Returns(7);
        reader.GetValue(1).Returns("Ali");

        var result = ResultSetMapper.Map(reader);

        Assert.Equal(2, result.Count);
        Assert.Equal("7", result[0]["id"]);
        Assert.Equal("Ali", result[0]["name"]);
    }

    [Fact]
    public void Map_ShouldUseEmptyString_When_NullValuesReceived()
    {
        var reader = Substitute.For<IDataReader>();

        reader.Read().Returns(true, false);
        reader.FieldCount.Returns(1);
        reader.GetName(0).Returns("col");
        reader.GetValue(0).Returns(null!);

        var result = ResultSetMapper.Map(reader);

        Assert.Equal(string.Empty, result[0]["col"]);
    }
}