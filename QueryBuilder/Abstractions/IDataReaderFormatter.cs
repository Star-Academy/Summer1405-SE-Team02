using System.Collections.Generic;
using System.Data;

namespace QueryBuilder.Abstractions
{
    public interface IDataReaderFormatter
    {
        List<Dictionary<string, string>> Format(IDataReader reader);
    }
}