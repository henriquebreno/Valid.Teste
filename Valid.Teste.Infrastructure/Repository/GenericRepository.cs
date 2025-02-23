using Dapper;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using Valid.Teste.Domain.Interfaces;
using Valid.Teste.Infrastructure;

namespace Valid.Teste.Infrastructure.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly DapperContext _context;
        private readonly string _tableName;

        public GenericRepository(DapperContext context)
        {
            _context = context;
            _tableName = typeof(T).Name + "s";
        }

        private IEnumerable<System.Reflection.PropertyInfo> GetColumnProperties()
        {
            return typeof(T).GetProperties()
                .Where(p => p.Name != "Id" && p.GetCustomAttributes(typeof(ColumnAttribute), false).Any());
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            var query = $"SELECT * FROM {_tableName}";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<T>(query);
        }

        public async Task<T?> GetById(int id)
        {
            var query = $"SELECT * FROM {_tableName} WHERE Id = @Id";
            using var connection = _context.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<T>(query, new { Id = id });
        }

        public async Task<int> Add(T entity)
        {
            var properties = GetColumnProperties().ToList();
            var columns = string.Join(", ", properties.Select(p => p.Name));
            var values = string.Join(", ", properties.Select(p => "@" + p.Name));

            var query = $"INSERT INTO {_tableName} ({columns}) VALUES ({values})";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(query, entity);
        }

        public async Task<int> Update(T entity)
        {
            var properties = GetColumnProperties().ToList();
            var setClause = string.Join(", ", properties.Select(p => $"{p.Name} = @{p.Name}"));

            var query = $"UPDATE {_tableName} SET {setClause} WHERE Id = @Id";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(query, entity);
        }

        public async Task<int> Delete(int id)
        {
            var query = $"DELETE FROM {_tableName} WHERE Id = @Id";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(query, new { Id = id });
        }
    }

}