using System.Collections.Generic;
using System.Data;
using QueryBuilder.Abstractions;

namespace QueryBuilder.Runners
{
    internal sealed class ParameterBinder : IParameterBinder
    {
        public void BindParameters(IDbCommand command, Dictionary<string, string> bindings)
        {
            if (command is null) throw new ArgumentNullException(nameof(command));
            if (bindings is null) throw new ArgumentNullException(nameof(bindings));
            foreach (var parameter in bindings)
            {
                var dbParameter = command.CreateParameter();
                dbParameter.ParameterName = parameter.Key;

                if (bool.TryParse(parameter.Value, out var boolValue))
                {
                    dbParameter.Value = boolValue;
                }
                else if (int.TryParse(parameter.Value, out var intValue))
                {
                    dbParameter.Value = intValue;
                }
                else
                {
                    dbParameter.Value = parameter.Value;
                }

                command.Parameters.Add(dbParameter);
            }
        }
    }
}