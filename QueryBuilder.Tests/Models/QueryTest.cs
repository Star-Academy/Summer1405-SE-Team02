using QueryBuilder.Models;
using Xunit;

namespace QueryBuilder.Tests.Models
{
    public class QueryTests
    {
        [Fact]
        public void From_ShouldSetTableName_WhenCalled()
        {
            // Arrange & Act
            var query = new Query().From("student");

            // Assert
            Assert.Equal("student", query.TableName);
        }

        [Fact]
        public void Select_ShouldAddColumnsToList_WhenColumnsPassed()
        {
            // Arrange & Act
            var query = new Query().From("student").Select("firstname", "lastname");

            // Assert
            Assert.Equal(new[] { "firstname", "lastname" }, query.SelectedColumns);
        }

        [Fact]
        public void Select_ShouldKeepColumnsEmpty_WhenNoColumnsPassed()
        {
            // Arrange & Act
            var query = new Query().From("student").Select();

            // Assert
            Assert.Empty(query.SelectedColumns);
        }

        [Fact]
        public void Where_ShouldAddConditionToList_WhenCalled()
        {
            // Arrange & Act
            var query = new Query().From("student").Where("age", "20");

            // Assert
            Assert.Single(query.Conditions);
            Assert.Equal("age", query.Conditions[0].Column);
            Assert.Equal("20", query.Conditions[0].Value);
        }

        [Fact]
        public void Where_ShouldPreserveConditionsOrder_WhenCalledMultipleTimes()
        {
            // Arrange & Act
            var query = new Query().From("student")
                .Where("age", "20")
                .Where("city", "Tehran");

            // Assert
            Assert.Equal(2, query.Conditions.Count);
            Assert.Equal("age", query.Conditions[0].Column);
            Assert.Equal("city", query.Conditions[1].Column);
        }

        [Fact]
        public void FluentApi_ShouldReturnSameInstance_WhenMethodsChained()
        {
            // Arrange
            var query = new Query();

            // Act
            var afterFrom = query.From("student");
            var afterSelect = afterFrom.Select("id");
            var afterWhere = afterSelect.Where("age", "20");

            // Assert
            Assert.Same(query, afterFrom);
            Assert.Same(query, afterSelect);
            Assert.Same(query, afterWhere);
        }
    }
}