using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using App.Business.Data;
using App.Models;
using MongoDB.Driver;

namespace App.Business.MongoDB
{
    public class MongoProductStore : IProductStore
    {
        private readonly IMongoDatabase _mongo;

        public MongoProductStore(IMongoDatabase mongo)
        {
            _mongo = mongo;
        }

        private IMongoCollection<Product> GetCollection()
        {
            return _mongo.GetCollection<Product>("products");
        }

        public async Task<IList<Product>> GetAllAsync(CancellationToken ct = default)
        {
            var cursor = await GetCollection().FindAsync(v => true, cancellationToken: ct);
            return await cursor.ToListAsync(ct);
        }

        public async Task<Product> GetByIdAsync(string id, CancellationToken ct = default)
        {
            var cursor = await GetCollection().FindAsync(v => v.Id == id, cancellationToken: ct);
            return await cursor.FirstOrDefaultAsync(ct);
        }

        public Task InsertAsync(Product product, CancellationToken ct = default)
        {
            return GetCollection().InsertOneAsync(product, cancellationToken: ct);
        }

        public Task<ReplaceOneResult> ReplaceAsync(Product product, CancellationToken ct = default)
        {
            return GetCollection().ReplaceOneAsync(v => v.Id == product.Id, product, cancellationToken: ct);
        }

        public Task<DeleteResult> DeleteAsync(string id, CancellationToken ct = default)
        {
            return GetCollection().DeleteOneAsync(v => v.Id == id, ct);
        }
    }
}
