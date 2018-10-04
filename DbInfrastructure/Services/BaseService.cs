using EFGenericRepository;
using DbInfrastructure.Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace DbInfrastructure.Services.IServices
{
    public abstract class BaseService<T> : IDisposable, IBaseService<T> where T : class, IEntity<int>
    {
        private IBaseRepository<T> baseRepository { get; set; }
        private IMemoryCache _cache;

        public BaseService(IBaseRepository<T> baseRepository, IMemoryCache cache)
        {
            this._cache = cache;
            this.baseRepository = baseRepository;
        }

        public virtual List<T> LoadEntites(Expression<Func<T, bool>> whereLambda)
        {
            return baseRepository.FindBy(whereLambda).ToList();
        }

        public virtual List<T> GetAll()
        {
            return baseRepository.GetAll().ToList();
        }

        public virtual T GetSingle(int id)
        {
            return baseRepository.GetSingle(id);
        }

        public virtual T SaveOrUpdate(T entity, object key)
        {
            var tmp = baseRepository.SaveOrUpdate(entity, key);
            return entity;
        }

        public virtual void DeleteEntity(T entity)
        {
            baseRepository.Delete(entity);
        }

        public virtual async Task<List<T>> LoadEntitesAsync(Expression<Func<T, bool>> whereLambda)
        {
            return await baseRepository.FindByAsync(whereLambda);
        }

        public virtual async Task<T> SaveOrUpdateAsync(T entity, object key)
        {
            return await baseRepository.SaveOrUpdateAsync(entity, key);
        }

        public virtual async Task<T> GetSingleAsync(int id)
        {
            return await baseRepository.GetAsync(id);
        }

        public virtual async Task<List<T>> GetAllAsync()
        {
            return await baseRepository.GetAllAsync();
        }

        public virtual async Task<int> DeleteEntityAsync(T entity)
        {
            return await baseRepository.DeleteAsync(entity);
        }
        public void Dispose()
        {
            baseRepository.Dispose();
        }
    }
}