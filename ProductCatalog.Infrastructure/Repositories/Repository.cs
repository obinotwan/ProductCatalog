using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductCatalog.Domain.Interfaces;

namespace ProductCatalog.Infrastructure.Repositories
{

    public abstract class Repository<T> : IRepository<T> where T : class
    {
        protected readonly List<T> _store = new();
        protected int _currentId = 1;

        public virtual Task<T?> GetByIdAsync(int id)
        {
            var entity = _store.FirstOrDefault(e => GetId(e) == id);
            return Task.FromResult(entity);
        }

        public virtual Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<T>>(_store.ToList());
        }

        public virtual Task<T> AddAsync(T entity)
        {
            SetId(entity, _currentId++);
            _store.Add(entity);
            return Task.FromResult(entity);
        }

        public virtual Task UpdateAsync(T entity)
        {
            var id = GetId(entity);
            var index = _store.FindIndex(e => GetId(e) == id);
            if (index >= 0)
                _store[index] = entity;
            return Task.CompletedTask;
        }

        public virtual Task DeleteAsync(int id)
        {
            var entity = _store.FirstOrDefault(e => GetId(e) == id);
            if (entity != null)
                _store.Remove(entity);
            return Task.CompletedTask;
        }

        public virtual Task<bool> ExistsAsync(int id)
        {
            var exists = _store.Any(e => GetId(e) == id);
            return Task.FromResult(exists);
        }

        // Abstract methods that child classes must implement
        protected abstract int GetId(T entity);
        protected abstract void SetId(T entity, int id);
    }
}