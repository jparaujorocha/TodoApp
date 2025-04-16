using Moq;
using TodoApp.Application.DTOs;
using TodoApp.Application.Interfaces;

namespace TodoApp.Tests.Mocks
{
    public static class MockTodoListService
    {
        public static Mock<ITodoListService> GetMock()
        {
            var mock = new Mock<ITodoListService>();
            var items = new List<TodoItemResponse>();
            int nextId = 1;

            var categories = new List<string> { "Work", "Personal", "Shopping", "Health", "Education" };

            mock.Setup(service => service.GetNextId()).Returns(() => nextId++);
            mock.Setup(service => service.GetAllCategories()).Returns(categories);
            mock.Setup(service => service.GetAllItems()).Returns(items);

            mock.Setup(service => service.GetItemById(It.IsAny<int>()))
                .Returns((int id) => items.FirstOrDefault(i => i.Id == id));

            mock.Setup(service => service.AddItem(It.IsAny<TodoItemRequest>()))
                .Returns(() => nextId++);

            mock.Setup(service => service.AddItemAndReturn(It.IsAny<TodoItemRequest>()))
                .Returns((TodoItemRequest request) =>
                {
                    var id = nextId++;
                    var newItem = new TodoItemResponse
                    {
                        Id = id,
                        Title = request.Title,
                        Description = request.Description,
                        Category = request.Category,
                        IsCompleted = false,
                        Progressions = new List<ProgressionResponse>()
                    };
                    items.Add(newItem);
                    return newItem;
                });

            mock.Setup(service => service.UpdateItem(It.IsAny<int>(), It.IsAny<TodoItemRequest>()))
                .Callback((int id, TodoItemRequest request) =>
                {
                    var existingItem = items.FirstOrDefault(i => i.Id == id);
                    if (existingItem != null)
                    {
                        existingItem.Description = request.Description;
                    }
                });

            mock.Setup(service => service.UpdateItemAndReturn(It.IsAny<int>(), It.IsAny<TodoItemRequest>()))
                .Returns((int id, TodoItemRequest request) =>
                {
                    var existingItem = items.FirstOrDefault(i => i.Id == id);
                    if (existingItem != null)
                    {
                        existingItem.Description = request.Description;
                        return existingItem;
                    }
                    return null!;
                });

            mock.Setup(service => service.RemoveItem(It.IsAny<int>()))
                .Callback((int id) =>
                {
                    var item = items.FirstOrDefault(i => i.Id == id);
                    if (item != null)
                    {
                        items.Remove(item);
                    }
                });

            mock.Setup(service => service.RegisterProgression(It.IsAny<int>(), It.IsAny<ProgressionRequest>()))
                .Callback((int id, ProgressionRequest request) =>
                {
                    var existingItem = items.FirstOrDefault(i => i.Id == id);
                    if (existingItem != null)
                    {
                        decimal accumulatedPercent = existingItem.Progressions.Sum(p => p.Percent) + request.Percent;

                        existingItem.Progressions.Add(new ProgressionResponse
                        {
                            Date = request.Date,
                            Percent = request.Percent,
                            AccumulatedPercent = accumulatedPercent
                        });

                        existingItem.IsCompleted = accumulatedPercent >= 100m;
                    }
                });

            mock.Setup(service => service.RegisterProgressionAndReturn(It.IsAny<int>(), It.IsAny<ProgressionRequest>()))
                .Returns((int id, ProgressionRequest request) =>
                {
                    var existingItem = items.FirstOrDefault(i => i.Id == id);
                    if (existingItem != null)
                    {
                        decimal accumulatedPercent = existingItem.Progressions.Sum(p => p.Percent) + request.Percent;

                        existingItem.Progressions.Add(new ProgressionResponse
                        {
                            Date = request.Date,
                            Percent = request.Percent,
                            AccumulatedPercent = accumulatedPercent
                        });

                        existingItem.IsCompleted = accumulatedPercent >= 100m;
                        return existingItem;
                    }
                    return null!;
                });

            mock.Setup(service => service.GetPrintOutput())
                .Returns(() =>
                {
                    return string.Join("\n", items.Select(i =>
                        $"{i.Id}) {i.Title} - {i.Description} ({i.Category}) Completed:{i.IsCompleted}"
                    ));
                });

            return mock;
        }

        public static Mock<ITodoListService> GetSampleDataMock()
        {
            var mock = new Mock<ITodoListService>();

            var todoItem = new TodoItemResponse
            {
                Id = 1,
                Title = "Complete Project Report",
                Description = "Finish the final report for the project",
                Category = "Work",
                IsCompleted = true,
                Progressions = new List<ProgressionResponse>
                {
                    new ProgressionResponse
                    {
                        Date = new DateTime(2025, 3, 18),
                        Percent = 30m,
                        AccumulatedPercent = 30m
                    },
                    new ProgressionResponse
                    {
                        Date = new DateTime(2025, 3, 19),
                        Percent = 50m,
                        AccumulatedPercent = 80m
                    },
                    new ProgressionResponse
                    {
                        Date = new DateTime(2025, 3, 20),
                        Percent = 20m,
                        AccumulatedPercent = 100m
                    }
                }
            };

            var items = new List<TodoItemResponse> { todoItem };
            var categories = new List<string> { "Work", "Personal", "Shopping", "Health", "Education" };

            mock.Setup(service => service.GetAllItems()).Returns(items);
            mock.Setup(service => service.GetItemById(1)).Returns(todoItem);
            mock.Setup(service => service.GetAllCategories()).Returns(categories);
            mock.Setup(service => service.GetNextId()).Returns(2);

            mock.Setup(service => service.GetPrintOutput())
                .Returns(
                    "1) Complete Project Report - Finish the final report for the project (Work) Completed:True\n" +
                    "3/18/2025 12:00:00 AM - 30% |OOOOOOOOOOOOOOO                                     |\n" +
                    "3/19/2025 12:00:00 AM - 80% |OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO            |\n" +
                    "3/20/2025 12:00:00 AM - 100% |OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO|"
                );

            return mock;
        }

        public static Mock<ITodoListService> GetEmptyMock()
        {
            var mock = new Mock<ITodoListService>();

            var items = new List<TodoItemResponse>();
            var categories = new List<string> { "Work", "Personal", "Shopping", "Health", "Education" };

            mock.Setup(service => service.GetAllItems()).Returns(items);
            mock.Setup(service => service.GetAllCategories()).Returns(categories);
            mock.Setup(service => service.GetNextId()).Returns(1);
            mock.Setup(service => service.GetPrintOutput()).Returns(string.Empty);

            mock.Setup(service => service.GetItemById(It.IsAny<int>()))
                .Throws<KeyNotFoundException>();

            return mock;
        }

        public static Mock<ITodoListService> GetErrorMock()
        {
            var mock = new Mock<ITodoListService>();

            mock.Setup(service => service.GetAllItems())
                .Throws(new Exception("Error retrieving items"));

            mock.Setup(service => service.GetItemById(It.IsAny<int>()))
                .Throws(new KeyNotFoundException("TodoItem not found"));

            mock.Setup(service => service.AddItem(It.IsAny<TodoItemRequest>()))
                .Throws(new ArgumentException("Invalid item data"));

            mock.Setup(service => service.UpdateItem(It.IsAny<int>(), It.IsAny<TodoItemRequest>()))
                .Throws(new InvalidOperationException("Cannot update item"));

            mock.Setup(service => service.RemoveItem(It.IsAny<int>()))
                .Throws(new InvalidOperationException("Cannot remove item"));

            mock.Setup(service => service.RegisterProgression(It.IsAny<int>(), It.IsAny<ProgressionRequest>()))
                .Throws(new ArgumentException("Invalid progression data"));

            return mock;
        }
    }
}