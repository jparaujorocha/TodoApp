using System;
using System.Collections.Generic;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Interfaces.Repositories;

namespace TodoApp.Infrastructure.Repositories
{
    public class TodoListRepository : Repository<TodoItem>, ITodoListRepository
    {
        private readonly List<string> _categories;
        private int _lastId;

        public TodoListRepository()
        {
            _categories = new List<string> { "Work", "Personal", "Shopping", "Health", "Education" };
            _lastId = 0;
        }

        public int GetNextId()
        {
            return ++_lastId;
        }

        public List<string> GetAllCategories()
        {
            return new List<string>(_categories);
        }

        public bool CategoryExists(string category)
        {
            return _categories.Contains(category);
        }

        public override void Add(TodoItem entity)
        {
            base.Add(entity);
            _lastId = Math.Max(_lastId, entity.Id);
        }

        protected override int GetEntityId(TodoItem entity)
        {
            return entity.Id;
        }
    }
}