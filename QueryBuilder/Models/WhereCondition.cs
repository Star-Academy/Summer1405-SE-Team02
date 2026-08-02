namespace QueryBuilder.Models
{
    public class WhereCondition
    {
        public string Column { get; set; }
        public string Value { get; set; }

        public WhereCondition(string column, string value)
        {
            Column = column;
            Value = value;
        }
    }
}