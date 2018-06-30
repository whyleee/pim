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
    public class MongoValueStore : IValueStore
    {
        private readonly IMongoDatabase _mongo;

        public MongoValueStore(IMongoDatabase mongo)
        {
            _mongo = mongo;
        }

        private IMongoCollection<Value> GetCollection()
        {
            return _mongo.GetCollection<Value>("values");
        }

        public async Task<IList<Value>> GetAllAsync(CancellationToken ct = default(CancellationToken))
        {
            var cursor = await GetCollection().FindAsync(v => true, cancellationToken: ct);
            return await cursor.ToListAsync(ct);
        }

        public async Task<Value> GetByIdAsync(string id, CancellationToken ct = default(CancellationToken))
        {
            var cursor = await GetCollection().FindAsync(v => v.Id == id, cancellationToken: ct);
            return await cursor.FirstOrDefaultAsync(ct);
        }

        public Task InsertAsync(Value value, CancellationToken ct = default(CancellationToken))
        {
            return GetCollection().InsertOneAsync(value, cancellationToken: ct);
        }

        public Task<ReplaceOneResult> ReplaceAsync(Value value, CancellationToken ct = default(CancellationToken))
        {
            return GetCollection().ReplaceOneAsync(v => v.Id == value.Id, value, cancellationToken: ct);
        }

        public Task<DeleteResult> DeleteAsync(string id, CancellationToken ct = default(CancellationToken))
        {
            return GetCollection().DeleteOneAsync(v => v.Id == id, ct);
        }
    }
}
