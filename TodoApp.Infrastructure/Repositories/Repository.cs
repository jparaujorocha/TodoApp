using System.Collections.Generic;
using System.Linq;
using TodoApp.Domain.Interfaces.Repositories;

namespace TodoApp.Infrastructure.Repositories
{
    public abstract class Repository<T> : IRepository<T>
    {
        protected readonly List<T> _entities;

        protected Repository()
        {
            _entities = new List<T>();
        }

        public virtual T GetById(int id)
        {
            return _entities.FirstOrDefault(e => GetEntityId(e) == id)!;
        }

        public virtual List<T> GetAll()
        {
            return new List<T>(_entities);
        }

        public virtual void Add(T entity)
        {
            _entities.Add(entity);
        }

        public virtual void Update(T entity)
        {
            int index = _entities.FindIndex(e => GetEntityId(e) == GetEntityId(entity));

            if (index >= 0)
            {
                _entities[index] = entity;
            }
        }

        public virtual void Delete(int id)
        {
            T entity = GetById(id);

            if (entity != null)
            {
                _entities.Remove(entity);
            }
        }

        protected abstract int GetEntityId(T entity);
    }
}