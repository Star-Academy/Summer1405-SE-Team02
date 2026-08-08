using FluentAssertions;
using QueryBuilder.Models;
using Xunit;

namespace QueryBuilder.Tests.Models
{
    public class QueryTests
    {
        [Fact]
        public void From_ShouldSetTableName_Whenever()
        {
            // Arrange
            var sut = new Query();

            // Act
            sut.From("student");

            // Assert
            sut.TableName.Should().Be("student");
        }

        [Fact]
        public void Select_ShouldAddColumnsToList_WhenColumnsPassed()
        {
            // Arrange
            var sut = new Query().From("student");

            // Act
            sut.Select("firstname", "lastname");

            // Assert
            sut.SelectedColumns.Should().BeEquivalentTo(new[] { "firstname", "lastname" });
        }

        [Fact]
        public void Select_ShouldKeepColumnsEmpty_WhenNoColumnsPassed()
        {
            // Arrange
            var sut = new Query().From("student");

            // Act
            sut.Select();

            // Assert
            sut.SelectedColumns.Should().BeEmpty();
        }

        [Fact]
        public void Where_ShouldAddConditionToList_Whenever()
        {
            // Arrange
            var sut = new Query().From("student");

            // Act
            sut.Where("age", "20");

            // Assert
            sut.Conditions.Should().HaveCount(1);
            sut.Conditions[0].Column.Should().Be("age");
            sut.Conditions[0].Value.Should().Be("20");
        }

        [Fact]
        public void Where_ShouldPreserveConditionsOrder_WheneverMultipleTimes()
        {
            // Arrange
            var sut = new Query().From("student");

            // Act
            sut.Where("age", "20")
                .Where("city", "Tehran");

            // Assert
            sut.Conditions.Should().HaveCount(2);
            sut.Conditions[0].Column.Should().Be("age");
            sut.Conditions[1].Column.Should().Be("city");
        }

        [Fact]
        public void FluentApi_ShouldReturnSameInstance_WhenMethodsChained()
        {
            // Arrange
            var sut = new Query();

            // Act
            var afterFrom = sut.From("student");
            var afterSelect = afterFrom.Select("id");
            var afterWhere = afterSelect.Where("age", "20");

            // Assert
            afterFrom.Should().BeSameAs(sut);
            afterSelect.Should().BeSameAs(sut);
            afterWhere.Should().BeSameAs(sut);
        }
    }
}