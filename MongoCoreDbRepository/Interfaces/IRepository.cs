using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace MongoCoreDbRepository.Interfaces
{
    public interface IRepository<TEntity> : IDisposable where TEntity : class 
    {
        void Add(TEntity obj);
        Task<TEntity> GetById(Guid id);
        Task<IEnumerable<TEntity>> GetAll(int skip, int limit, string sortBy);
        Task<IEnumerable<TEntity>> GetByFilter(FilterDefinition<TEntity> filter, int skip, int limit, string sortBy );
        Task<IEnumerable<TEntity>> GetByFilter(string field, string search, int skip, int limit, string sortBy );
        Task<IEnumerable<TEntity>> GetByFilterLike(string field, string search, int skip, int limit, string sortBy );
        void Update(TEntity obj);
        void Remove(Guid id);
        Task<long> Count();
        Task<int> CountByFilter(FilterDefinition<TEntity> filter);
        Task<int> CountByFilter(string field, string search);
        Task<int> CountByFilterLike(string field, string search);
    }
}