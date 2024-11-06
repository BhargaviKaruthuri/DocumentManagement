

namespace DocumentManagement.Data
{
    public interface IDbConnectionFactory
    {
        Task<int> ExecuteAsync(string query, object param = null);
        Task<IEnumerable<T>> QueryAsync<T>(string query, object param = null);
        Task<int> ExecuteScalarAsync<T>(string query, object param = null);

    }
}
