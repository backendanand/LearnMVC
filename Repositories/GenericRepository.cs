using Dapper;
using LearnMVC.Data;
using LearnMVC.Models.DTOs;
using System.Data;
using System.Reflection;
using System.Text;

namespace LearnMVC.Repositories
{
    public class GenericRepository : IGenericRepository
    {
        private readonly DapperContext _context;

        public GenericRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<T>> GetAsync<T>(QueryOptions options)
        {
            using var connection = _context.CreateConnection();

            var sql = new StringBuilder();
            var parameters = new DynamicParameters();

            // SELECT
            var columns = options.SelectColumns.Any()
                ? string.Join(", ", options.SelectColumns)
                : "*";

            sql.Append($"SELECT {columns} FROM {options.Table} {options.Alias}");

            // JOINS
            foreach (var join in options.Joins)
            {
                sql.Append($" {join.JoinType} JOIN {join.Table} {join.Alias} ON {join.OnCondition}");
            }

            // WHERE
            if (options.Filters.Any())
            {
                sql.Append(" WHERE ");
                var conditions = new List<string>();
                int i = 0;

                foreach (var filter in options.Filters)
                {
                    var paramName = $"@p{i}";
                    conditions.Add(BuildCondition(filter, paramName, parameters));
                    i++;
                }

                sql.Append(string.Join(" AND ", conditions));
            }

            // ORDER BY
            if (options.Sorts.Any())
            {
                var order = options.Sorts
                    .Select(s => $"{s.Column} {(s.Descending ? "DESC" : "ASC")}");
                sql.Append($" ORDER BY {string.Join(", ", order)}");
            }

            // LIMIT / OFFSET (PostgreSQL)
            if (options.Limit.HasValue)
                sql.Append($" LIMIT {options.Limit.Value}");

            if (options.Offset.HasValue)
                sql.Append($" OFFSET {options.Offset.Value}");

            return await connection.QueryAsync<T>(sql.ToString(), parameters);
        }

        private string BuildCondition(QueryFilter filter, string paramName, DynamicParameters parameters)
        {
            switch (filter.Operator)
            {
                case QueryOperator.Equal:
                    parameters.Add(paramName, filter.Value);
                    return $"{filter.Column} = {paramName}";

                case QueryOperator.NotEqual:
                    parameters.Add(paramName, filter.Value);
                    return $"{filter.Column} <> {paramName}";

                case QueryOperator.GreaterThan:
                    parameters.Add(paramName, filter.Value);
                    return $"{filter.Column} > {paramName}";

                case QueryOperator.LessThan:
                    parameters.Add(paramName, filter.Value);
                    return $"{filter.Column} < {paramName}";

                case QueryOperator.GreaterOrEqual:
                    parameters.Add(paramName, filter.Value);
                    return $"{filter.Column} >= {paramName}";

                case QueryOperator.LessOrEqual:
                    parameters.Add(paramName, filter.Value);
                    return $"{filter.Column} <= {paramName}";

                case QueryOperator.Like:
                    parameters.Add(paramName, filter.Value);
                    return $"{filter.Column} ILIKE {paramName}";

                case QueryOperator.In:
                    parameters.Add(paramName, filter.Value);
                    return $"{filter.Column} = ANY({paramName})";

                case QueryOperator.Between:
                    var p1 = paramName + "1";
                    var p2 = paramName + "2";
                    parameters.Add(p1, filter.Value);
                    parameters.Add(p2, filter.SecondValue);
                    return $"{filter.Column} BETWEEN {p1} AND {p2}";

                case QueryOperator.IsNull:
                    return $"{filter.Column} IS NULL";

                case QueryOperator.IsNotNull:
                    return $"{filter.Column} IS NOT NULL";

                default:
                    throw new NotSupportedException("Operator not supported");
            }
        }

        public async Task<T> GetByIdAsync<T>(string table, string keyColumn, object id)
        {
            using var connection = _context.CreateConnection();
            var sql = $"SELECT * FROM {table} WHERE {keyColumn} = @Id";
            return await connection.QueryFirstOrDefaultAsync<T>(sql, new { Id = id });
        }

        public async Task<long> InsertAsync<T>(string table, T entity)
        {
            using var connection = _context.CreateConnection();

            var props = typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.Name.ToLower() != "id");

            var columns = props.Select(p => p.Name);
            var values = props.Select(p => "@" + p.Name);

            var sql = $@"
            INSERT INTO {table} ({string.Join(",", columns)})
            VALUES ({string.Join(",", values)})
            RETURNING id;";

            var newId = await connection.ExecuteScalarAsync<long>(sql, entity);
            return newId;
        }

        public async Task<int> UpdateAsync<T>(string table, string keyColumn, object id, T entity)
        {
            using var connection = _context.CreateConnection();

            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var setClause = props
                .Where(p => p.Name != keyColumn)
                .Select(p => $"{p.Name} = @{p.Name}");

            var sql = $@"
            UPDATE {table}
            SET {string.Join(",", setClause)}
            WHERE {keyColumn} = @Id";

            var parameters = new DynamicParameters(entity);
            parameters.Add("Id", id);

            return await connection.ExecuteAsync(sql, parameters);
        }

        public async Task<int> DeleteAsync(string table, string keyColumn, object id)
        {
            using var connection = _context.CreateConnection();
            var sql = $"DELETE FROM {table} WHERE {keyColumn} = @Id";
            return await connection.ExecuteAsync(sql, new { Id = id });
        }
    }

}
