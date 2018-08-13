using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using App.Models;
using MongoDB.Driver;

namespace App.Business.Data
{
    public interface IProductStore
    {
        Task<IList<Product>> GetAllAsync(CancellationToken ct = default);
        Task<Product> GetByIdAsync(string id, CancellationToken ct = default);
        Task InsertAsync(Product product, CancellationToken ct = default);
        Task<ReplaceOneResult> ReplaceAsync(Product product, CancellationToken ct = default);
        Task<DeleteResult> DeleteAsync(string id, CancellationToken ct = default);
    }
}
