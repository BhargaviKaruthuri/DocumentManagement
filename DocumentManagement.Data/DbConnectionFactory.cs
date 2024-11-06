using Dapper;
using Microsoft.Data.SqlClient;


namespace DocumentManagement.Data
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connString;
        public DbConnectionFactory(string connString)
        {
            _connString = connString;
        }

        public async Task<int> ExecuteAsync(string query, object param = null)
        {
            using (var connection = new SqlConnection(_connString))
            {
                return await connection.ExecuteAsync(query, param);
            }
        }
        public async Task<IEnumerable<T>> QueryAsync<T>(string query, object param = null)
        {
            using (var connection = new SqlConnection(_connString))
            {
                return await connection.QueryAsync<T>(query, param);
            }
        }
        public async Task<int> ExecuteScalarAsync<T>(string query, object param = null)
        {
            using (var connection = new SqlConnection(_connString))
            {
                return await connection.ExecuteScalarAsync<int>(query, param);
            }
        }
    }
}
