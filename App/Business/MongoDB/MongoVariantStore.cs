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
    public class MongoVariantStore : IVariantStore
    {
        private readonly IMongoDatabase _mongo;

        public MongoVariantStore(IMongoDatabase mongo)
        {
            _mongo = mongo;
        }

        private IMongoCollection<Variant> GetCollection()
        {
            return _mongo.GetCollection<Variant>("variants");
        }

        public async Task<IList<Variant>> GetAllAsync(CancellationToken ct = default)
        {
            var cursor = await GetCollection().FindAsync(v => true, cancellationToken: ct);
            return await cursor.ToListAsync(ct);
        }

        public async Task<IList<Variant>> GetAllByProductIdAsync(string productId, CancellationToken ct = default)
        {
            var cursor = await GetCollection().FindAsync(v => v.ProductId == productId, cancellationToken: ct);
            return await cursor.ToListAsync(ct);
        }

        public async Task<Variant> GetByIdAsync(string id, CancellationToken ct = default)
        {
            var cursor = await GetCollection().FindAsync(v => v.Id == id, cancellationToken: ct);
            return await cursor.FirstOrDefaultAsync(ct);
        }

        public Task InsertAsync(Variant variant, CancellationToken ct = default)
        {
            return GetCollection().InsertOneAsync(variant, cancellationToken: ct);
        }

        public Task<ReplaceOneResult> ReplaceAsync(Variant variant, CancellationToken ct = default)
        {
            return GetCollection().ReplaceOneAsync(v => v.Id == variant.Id, variant, cancellationToken: ct);
        }

        public Task<DeleteResult> DeleteAsync(string id, CancellationToken ct = default)
        {
            return GetCollection().DeleteOneAsync(v => v.Id == id, ct);
        }
    }
}
