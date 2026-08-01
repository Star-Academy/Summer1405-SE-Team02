using System.Collections.Generic;
public class SqlResult
{
    public string? Sql { get; set; }
    public Dictionary<string, object> Bindings { get; set; } = new Dictionary<string, object>();
}