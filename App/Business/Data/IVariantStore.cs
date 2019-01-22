using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using App.Models;
using MongoDB.Driver;

namespace App.Business.Data
{
    public interface IVariantStore
    {
        Task<IList<Variant>> GetAllAsync(CancellationToken ct = default);
        Task<IList<Variant>> GetAllByProductIdAsync(string productId, CancellationToken ct = default);
        Task<Variant> GetByIdAsync(string id, CancellationToken ct = default);
        Task InsertAsync(Variant variant, CancellationToken ct = default);
        Task<ReplaceOneResult> ReplaceAsync(Variant variant, CancellationToken ct = default);
        Task<DeleteResult> DeleteAsync(string id, CancellationToken ct = default);
    }
}
