using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoCoreDbRepository.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using ServiceStack;


namespace MongoCoreDbRepository.Repository
{
    public abstract class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly IMongoContext _context;
        protected IMongoCollection<TEntity> DbSet;

        protected BaseRepository(IMongoContext context)
        {
            _context = context;
            DbSet = _context.GetCollection<TEntity>(typeof(TEntity).Name);
        }

        private void ConfigDbSet()
        {
            DbSet = _context.GetCollection<TEntity>(typeof(TEntity).Name);
        }
        
        public virtual void Add(TEntity obj)
        {
            ConfigDbSet();
            _context.AddCommand(() => DbSet.InsertOneAsync(obj));
        }    
        
        public virtual async Task<TEntity> GetById(Guid id)
        {
            ConfigDbSet();
            // var data = await DbSet.FindAsync(Builders<TEntity>.Filter.Eq("_id", id));
            var data = await DbSet.Find(new BsonDocument {{"_id", id}})
                .FirstOrDefaultAsync();
            return data;
        }
        
        public virtual async Task<IEnumerable<TEntity>> GetByFilter(FilterDefinition<TEntity> filter, int skip, int limit, string sortBy)
        {
            ConfigDbSet();
            var all = await DbSet
                .Find(filter)
                .Sort(Builders<TEntity>.Sort.Descending(sortBy))
                .Skip(skip)
                .Limit(limit)
                .ToListAsync();
            return all;
        }
        
        public virtual async Task<IEnumerable<TEntity>> GetByFilter(string field, string search, int skip, int limit, string sortBy)
        {
            ConfigDbSet();
            var all = await DbSet
                .Find(new BsonDocument {{field, search}})
                .Sort(Builders<TEntity>.Sort.Descending(sortBy))
                .Skip(skip)
                .Limit(limit)
                .ToListAsync();
            return all;
        }
        
        public virtual async Task<IEnumerable<TEntity>> GetByFilterLike(string field, string search, int skip, int limit, string sortBy)
        {
            ConfigDbSet();
            var all = await DbSet
                .Find(new BsonDocument {{ field, new BsonDocument { { "$regex", ".*"+search+".*" }}}})
                .Sort(Builders<TEntity>.Sort.Descending(sortBy))
                .Skip(skip)
                .Limit(limit)
                .ToListAsync();
            return all;
        }
        
        public virtual async Task<IEnumerable<TEntity>> GetAll(int skip, int limit, string sortBy)
        {
            ConfigDbSet();
            var all = await DbSet.Find(_ => true)
                .Sort(Builders<TEntity>.Sort.Descending(sortBy))
                .Skip(skip)
                .Limit(limit)
                .ToListAsync();
            return all;
        }
        
        
        public virtual void Update(TEntity obj)
        {
            ConfigDbSet();
            _context.AddCommand(async () => await DbSet.ReplaceOneAsync(Builders<TEntity>.Filter.Eq("_id", obj.GetId()), obj));
        }
        
        public virtual void Remove(Guid id)
        {
            _context.AddCommand(async () => await DbSet.DeleteOneAsync(Builders<TEntity>.Filter.Eq("_id", id))); 
        }
        
        public async Task<int> CountByFilter(FilterDefinition<TEntity> filter)
        {
            var count = await DbSet
                .CountDocumentsAsync(filter);

            return Convert.ToInt32(count);
        }

        public async Task<int> CountByFilter(string field, string search)
        {
            var count = await DbSet
                .CountDocumentsAsync(new BsonDocument {{field, search}});

            return Convert.ToInt32(count);
        }
        
        public async Task<int> CountByFilterLike(string field, string search)
        {
            var count = await DbSet
                .CountDocumentsAsync(new BsonDocument {{ field, new BsonDocument { { "$regex", ".*"+search+".*" }}}});
    
            return Convert.ToInt32(count);
        }
        
        public async Task<long> Count()
        {
            var countAll = await DbSet.Find(_ => true).CountDocumentsAsync();

            return countAll;
        }
        
        
        public void Dispose()
        {
            _context?.Dispose();
        }

    }
}