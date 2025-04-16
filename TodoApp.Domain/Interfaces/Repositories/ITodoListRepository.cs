using System.Collections.Generic;
using TodoApp.Domain.Entities;

namespace TodoApp.Domain.Interfaces.Repositories
{
    public interface ITodoListRepository : IRepository<TodoItem>
    {
        int GetNextId();
        List<string> GetAllCategories();
        bool CategoryExists(string category);
    }
}