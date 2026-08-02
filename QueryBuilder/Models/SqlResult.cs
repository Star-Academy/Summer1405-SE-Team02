using System.Collections.Generic;

namespace QueryBuilder.Models
{
    public class SqlResult
    {
        public string Sql { get; set; } = string.Empty;

        public Dictionary<string, string> Bindings { get; set; } = new Dictionary<string, string>();
    }
}