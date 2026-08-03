using System.Collections.Generic;
using System.Data;

namespace QueryBuilder.Abstractions
{
    public interface IParameterBinder
    {
        void BindParameters(IDbCommand command, Dictionary<string, string> bindings);
    }
}