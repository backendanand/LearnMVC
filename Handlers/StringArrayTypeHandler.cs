using Dapper;
using System.Data;

namespace LearnMVC.Handlers
{
    public class StringArrayTypeHandler : SqlMapper.TypeHandler<string[]>
    {
        public override void SetValue(IDbDataParameter parameter, string[] value)
        {
            parameter.Value = value;
        }

        public override string[] Parse(object value)
        {
            return (string[])value;
        }
    }
}
