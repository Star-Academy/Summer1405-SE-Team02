using System.Collections.Generic;

namespace QueryBuilder.Models
{
    public class CompiledQuery
    {
        public string RawSql { get; set; } = string.Empty;
        public Dictionary<string, string> Bindings { get; set; } = [];
    }
}