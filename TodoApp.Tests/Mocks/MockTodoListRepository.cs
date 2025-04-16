using Moq;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Interfaces.Repositories;

namespace TodoApp.Tests.Mocks
{
    public static class MockTodoListRepository
    {
        public static Mock<ITodoListRepository> GetMock()
        {
            var mock = new Mock<ITodoListRepository>();
            var items = new List<TodoItem>();
            int nextId = 1;

            var categories = new List<string> { "Work", "Personal", "Shopping", "Health", "Education" };

            mock.Setup(repo => repo.GetNextId()).Returns(() => nextId++);
            mock.Setup(repo => repo.GetAllCategories()).Returns(categories);
            mock.Setup(repo => repo.CategoryExists(It.IsAny<string>()))
                .Returns((string category) => categories.Contains(category));
            mock.Setup(repo => repo.GetById(It.IsAny<int>()))
                .Returns((int id) => items.FirstOrDefault(i => i.Id == id));
            mock.Setup(repo => repo.GetAll()).Returns(items);
            mock.Setup(repo => repo.Add(It.IsAny<TodoItem>()))
                .Callback((TodoItem item) =>
                {
                    items.Add(item);
                });
            mock.Setup(repo => repo.Update(It.IsAny<TodoItem>()))
                .Callback((TodoItem item) =>
                {
                    var existingItem = items.FirstOrDefault(i => i.Id == item.Id);
                    if (existingItem != null)
                    {
                        items.Remove(existingItem);
                        items.Add(item);
                    }
                });
            mock.Setup(repo => repo.Delete(It.IsAny<int>()))
                .Callback((int id) =>
                {
                    var item = items.FirstOrDefault(i => i.Id == id);
                    if (item != null)
                    {
                        items.Remove(item);
                    }
                });

            return mock;
        }

        public static Mock<ITodoListRepository> GetMockWithSampleData()
        {
            var mock = GetMock();
            var items = new List<TodoItem>();

            var todoItem = new TodoItem(1, "Complete Project Report", "Finish the final report for the project", "Work");

            var date1 = new DateTime(2025, 3, 18);
            var date2 = new DateTime(2025, 3, 19);
            var date3 = new DateTime(2025, 3, 20);

            todoItem.AddProgression(date1, 30m);
            todoItem.AddProgression(date2, 50m);
            todoItem.AddProgression(date3, 20m);

            items.Add(todoItem);

            mock.Setup(repo => repo.GetAll()).Returns(items);
            mock.Setup(repo => repo.GetById(It.Is<int>(id => id == 1)))
                .Returns(todoItem);

            return mock;
        }
    }
}