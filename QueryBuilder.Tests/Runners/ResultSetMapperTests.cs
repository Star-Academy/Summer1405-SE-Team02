using System.Data;
using Moq;
using QueryBuilder.Runners;
using Xunit;

public class ResultSetMapperTests
{
    [Fact]
    public void Map_Should_Return_Empty_List_When_No_Rows()
    {
        var reader = new Mock<IDataReader>();
        reader.Setup(r => r.Read()).Returns(false);

        var result = ResultSetMapper.Map(reader.Object);

        Assert.Empty(result);
    }

    [Fact]
    public void Map_Should_Convert_Rows_To_Dictionaries()
    {
        var reader = new Mock<IDataReader>();
        var readCount = 0;

        reader.Setup(r => r.Read()).Returns(() => ++readCount <= 2);
        reader.Setup(r => r.FieldCount).Returns(2);
        reader.Setup(r => r.GetName(0)).Returns("id");
        reader.Setup(r => r.GetName(1)).Returns("name");
        reader.Setup(r => r.GetValue(0)).Returns(7);
        reader.Setup(r => r.GetValue(1)).Returns("Ali");

        var result = ResultSetMapper.Map(reader.Object);

        Assert.Equal(2, result.Count);
        Assert.Equal("7", result[0]["id"]);
        Assert.Equal("Ali", result[0]["name"]);
    }

    [Fact]
    public void Map_Should_Use_Empty_String_For_Null_Values()
    {
        var reader = new Mock<IDataReader>();
        var readCount = 0;

        reader.Setup(r => r.Read()).Returns(() => ++readCount <= 1);
        reader.Setup(r => r.FieldCount).Returns(1);
        reader.Setup(r => r.GetName(0)).Returns("col");
        reader.Setup(r => r.GetValue(0)).Returns((object?)null);

        var result = ResultSetMapper.Map(reader.Object);

        Assert.Equal(string.Empty, result[0]["col"]);
    }
}