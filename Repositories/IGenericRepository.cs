using LearnMVC.Models.DTOs;

namespace LearnMVC.Repositories
{
    public interface IGenericRepository
    {
        Task<IEnumerable<T>> GetAsync<T>(QueryOptions options);
        Task<T> GetByIdAsync<T>(string table, string keyColumn, object id);
        Task<long> InsertAsync<T>(string table, T entity);
        Task<int> UpdateAsync<T>(string table, string keyColumn, object id, T entity);
        Task<int> DeleteAsync(string table, string keyColumn, object id);
    }

}
