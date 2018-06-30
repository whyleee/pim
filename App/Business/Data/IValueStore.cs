using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using App.Models;
using MongoDB.Driver;

namespace App.Business.Data
{
    public interface IValueStore
    {
        Task<IList<Value>> GetAllAsync(CancellationToken ct = default(CancellationToken));
        Task<Value> GetByIdAsync(string id, CancellationToken ct = default(CancellationToken));
        Task InsertAsync(Value value, CancellationToken ct = default(CancellationToken));
        Task<ReplaceOneResult> ReplaceAsync(Value value, CancellationToken ct = default(CancellationToken));
        Task<DeleteResult> DeleteAsync(string id, CancellationToken ct = default(CancellationToken));
    }
}
