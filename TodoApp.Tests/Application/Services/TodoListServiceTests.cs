using Bogus;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using TodoApp.Application.DTOs;
using TodoApp.Application.Services;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Interfaces.Repositories;
using TodoApp.Tests.Mocks;

namespace TodoApp.Tests.Application.Services
{
    [TestFixture]
    public class TodoListServiceTests
    {
        private Mock<ITodoListRepository> _mockRepository;
        private Mock<ILogger<TodoListService>> _mockLogger;
        private TodoListService _service;
        private Faker _faker;

        [SetUp]
        public void Setup()
        {
            _mockRepository = MockTodoListRepository.GetMock();
            _mockLogger = new Mock<ILogger<TodoListService>>();
            _service = new TodoListService(_mockRepository.Object, _mockLogger.Object);
            _faker = new Faker();
        }

        [Test]
        public void AddItem_WithValidData_AddsItemToRepository()
        {
            // Arrange
            int id = 1;
            string title = _faker.Lorem.Sentence();
            string description = _faker.Lorem.Paragraph();
            string category = "Work";

            _mockRepository.Setup(r => r.GetAllCategories()).Returns(new List<string> { "Work" });

            // Act
            _service.AddItem(id, title, description, category);

            // Assert
            _mockRepository.Verify(r => r.Add(It.Is<TodoItem>(i =>
                i.Id == id &&
                i.Title == title &&
                i.Description == description &&
                i.Category == category)), Times.Once);
        }

        [Test]
        public void AddItem_WithInvalidCategory_ThrowsArgumentException()
        {
            // Arrange
            int id = 1;
            string title = _faker.Lorem.Sentence();
            string description = _faker.Lorem.Paragraph();
            string category = "InvalidCategory";

            _mockRepository.Setup(r => r.GetAllCategories()).Returns(new List<string> { "Work" });

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                _service.AddItem(id, title, description, category));

            Assert.That(exception.Message, Does.Contain("Category 'InvalidCategory' is not valid"));
        }

        [Test]
        public void AddItem_WithRequest_ReturnsNewId()
        {
            // Arrange
            string title = _faker.Lorem.Sentence();
            string description = _faker.Lorem.Paragraph();
            string category = "Work";
            var request = new TodoItemRequest
            {
                Title = title,
                Description = description,
                Category = category
            };

            _mockRepository.Setup(r => r.GetAllCategories()).Returns(new List<string> { "Work" });
            _mockRepository.Setup(r => r.GetNextId()).Returns(42);

            // Act
            int result = _service.AddItem(request);

            // Assert
            Assert.That(result, Is.EqualTo(42));
            _mockRepository.Verify(r => r.Add(It.Is<TodoItem>(i =>
                i.Id == 42 &&
                i.Title == title &&
                i.Description == description &&
                i.Category == category)), Times.Once);
        }

        [Test]
        public void AddItemAndReturn_WithValidData_ReturnsItemResponse()
        {
            // Arrange
            string title = _faker.Lorem.Sentence();
            string description = _faker.Lorem.Paragraph();
            string category = "Work";
            var request = new TodoItemRequest
            {
                Title = title,
                Description = description,
                Category = category
            };

            _mockRepository.Setup(r => r.GetAllCategories()).Returns(new List<string> { "Work" });
            _mockRepository.Setup(r => r.GetNextId()).Returns(42);

            // Act
            var result = _service.AddItemAndReturn(request);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(42));
            Assert.That(result.Title, Is.EqualTo(title));
            Assert.That(result.Description, Is.EqualTo(description));
            Assert.That(result.Category, Is.EqualTo(category));
            Assert.That(result.IsCompleted, Is.False);
            Assert.That(result.Progressions, Is.Empty);
        }

        [Test]
        public void AddItem_WithNullCategory_ThrowsArgumentException()
        {
            // Arrange
            string title = _faker.Lorem.Sentence();
            string description = _faker.Lorem.Paragraph();
            var request = new TodoItemRequest
            {
                Title = title,
                Description = description,
                Category = null!
            };

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => _service.AddItem(request));
            Assert.That(exception.Message, Does.Contain("Category is required"));
        }

        [Test]
        public void AddItem_WithEmptyTitle_ThrowsArgumentException()
        {
            // Arrange
            string description = _faker.Lorem.Paragraph();
            string category = "Work";
            var request = new TodoItemRequest
            {
                Title = string.Empty,
                Description = description,
                Category = category
            };

            _mockRepository.Setup(r => r.GetAllCategories()).Returns(new List<string> { "Work" });

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => _service.AddItem(request));
            Assert.That(exception.Message, Does.Contain("Title is required"));
        }

        [Test]
        public void AddItem_WithNullRequest_ThrowsArgumentNullException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => _service.AddItem(null!));
            Assert.That(exception.Message, Does.Contain("Request cannot be null"));
        }

        [Test]
        public void UpdateItem_WithValidData_UpdatesItemInRepository()
        {
            // Arrange
            int id = 1;
            string newDescription = "Updated description";
            var todoItem = new TodoItem(id, "Test", "Old description", "Work");

            _mockRepository.Setup(r => r.GetById(id)).Returns(todoItem);

            // Act
            _service.UpdateItem(id, newDescription);

            // Assert
            _mockRepository.Verify(r => r.Update(It.Is<TodoItem>(i =>
                i.Id == id &&
                i.Description == newDescription)), Times.Once);
        }

        [Test]
        public void UpdateItem_WithNonExistentId_ThrowsKeyNotFoundException()
        {
            // Arrange
            int id = 999;
            string newDescription = "Updated description";

            _mockRepository.Setup(r => r.GetById(id)).Returns((TodoItem)null!);

            // Act & Assert
            var exception = Assert.Throws<KeyNotFoundException>(() =>
                _service.UpdateItem(id, newDescription));

            Assert.That(exception.Message, Does.Contain("TodoItem with id 999 not found"));
        }

        [Test]
        public void UpdateItem_WithItemOver50PercentProgress_ThrowsInvalidOperationException()
        {
            // Arrange
            int id = 1;
            string newDescription = "Updated description";

            var todoItem = new TodoItem(id, "Test", "Old description", "Work");
            todoItem.AddProgression(DateTime.Now.AddDays(-2), 30m);
            todoItem.AddProgression(DateTime.Now.AddDays(-1), 30m);

            _mockRepository.Setup(r => r.GetById(id)).Returns(todoItem);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                _service.UpdateItem(id, newDescription));

            Assert.That(exception.Message, Does.Contain("Cannot update a TodoItem that has more than 50% progress"));
        }

        [Test]
        public void UpdateItem_WithRequest_UpdatesItemInRepository()
        {
            // Arrange
            int id = 1;
            string newDescription = "Updated description";
            var request = new TodoItemRequest
            {
                Title = "Test",
                Description = newDescription,
                Category = "Work"
            };

            var todoItem = new TodoItem(id, "Test", "Old description", "Work");

            _mockRepository.Setup(r => r.GetById(id)).Returns(todoItem);
            _mockRepository.Setup(r => r.GetAllCategories()).Returns(new List<string> { "Work" });

            // Act
            _service.UpdateItem(id, request);

            // Assert
            _mockRepository.Verify(r => r.Update(It.Is<TodoItem>(i =>
                i.Id == id &&
                i.Description == newDescription)), Times.Once);
        }

        [Test]
        public void UpdateItemAndReturn_WithValidData_ReturnsUpdatedItem()
        {
            // Arrange
            int id = 1;
            string newDescription = "Updated description";
            var request = new TodoItemRequest
            {
                Title = "Test",
                Description = newDescription,
                Category = "Work"
            };

            var todoItem = new TodoItem(id, "Test", "Old description", "Work");

            _mockRepository.Setup(r => r.GetById(id)).Returns(todoItem);
            _mockRepository.Setup(r => r.GetAllCategories()).Returns(new List<string> { "Work" });

            // Act
            var result = _service.UpdateItemAndReturn(id, request);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(id));
            Assert.That(result.Description, Is.EqualTo(newDescription));
        }

        [Test]
        public void RemoveItem_WithValidId_RemovesItemFromRepository()
        {
            // Arrange
            int id = 1;
            var todoItem = new TodoItem(id, "Test", "Description", "Work");

            _mockRepository.Setup(r => r.GetById(id)).Returns(todoItem);

            // Act
            _service.RemoveItem(id);

            // Assert
            _mockRepository.Verify(r => r.Delete(id), Times.Once);
        }

        [Test]
        public void RemoveItem_WithNonExistentId_ThrowsKeyNotFoundException()
        {
            // Arrange
            int id = 999;

            _mockRepository.Setup(r => r.GetById(id)).Returns((TodoItem)null!);

            // Act & Assert
            var exception = Assert.Throws<KeyNotFoundException>(() =>
                _service.RemoveItem(id));

            Assert.That(exception.Message, Does.Contain("TodoItem with id 999 not found"));
        }

        [Test]
        public void RemoveItem_WithItemOver50PercentProgress_ThrowsInvalidOperationException()
        {
            // Arrange
            int id = 1;

            var todoItem = new TodoItem(id, "Test", "Description", "Work");
            todoItem.AddProgression(DateTime.Now.AddDays(-2), 30m);
            todoItem.AddProgression(DateTime.Now.AddDays(-1), 30m);

            _mockRepository.Setup(r => r.GetById(id)).Returns(todoItem);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                _service.RemoveItem(id));

            Assert.That(exception.Message, Does.Contain("Cannot remove a TodoItem that has more than 50% progress"));
        }

        [Test]
        public void RegisterProgression_WithValidData_AddsProgressionToItem()
        {
            // Arrange
            int id = 1;
            DateTime date = DateTime.Now;
            decimal percent = 30m;

            var todoItem = new TodoItem(id, "Test", "Description", "Work");

            _mockRepository.Setup(r => r.GetById(id)).Returns(todoItem);

            // Act
            _service.RegisterProgression(id, date, percent);

            // Assert
            _mockRepository.Verify(r => r.Update(It.Is<TodoItem>(i =>
                i.Id == id &&
                i.GetTotalProgress() == percent)), Times.Once);
        }

        [Test]
        public void RegisterProgression_WithNonExistentId_ThrowsKeyNotFoundException()
        {
            // Arrange
            int id = 999;
            DateTime date = DateTime.Now;
            decimal percent = 30m;

            _mockRepository.Setup(r => r.GetById(id)).Returns((TodoItem)null!);

            // Act & Assert
            var exception = Assert.Throws<KeyNotFoundException>(() =>
                _service.RegisterProgression(id, date, percent));

            Assert.That(exception.Message, Does.Contain("TodoItem with id 999 not found"));
        }

        [Test]
        public void RegisterProgression_WithInvalidPercent_ThrowsArgumentException()
        {
            // Arrange
            int id = 1;
            DateTime date = DateTime.Now;
            decimal percent = 110m;

            var todoItem = new TodoItem(id, "Test", "Description", "Work");

            _mockRepository.Setup(r => r.GetById(id)).Returns(todoItem);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                _service.RegisterProgression(id, date, percent));

            Assert.That(exception.Message, Does.Contain("Percent must be greater than 0 and less than or equal to 100"));
        }

        [Test]
        public void RegisterProgression_WithTotalExceeding100Percent_ThrowsArgumentException()
        {
            // Arrange
            int id = 1;
            DateTime date = DateTime.Now;
            decimal percent = 70m;

            var todoItem = new TodoItem(id, "Test", "Description", "Work");
            todoItem.AddProgression(DateTime.Now.AddDays(-1), 40m);

            _mockRepository.Setup(r => r.GetById(id)).Returns(todoItem);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                _service.RegisterProgression(id, date, percent));

            Assert.That(exception.Message, Does.Contain("Total progress cannot exceed 100%"));
        }

        [Test]
        public void RegisterProgression_WithEarlierDate_ThrowsArgumentException()
        {
            // Arrange
            int id = 1;
            DateTime laterDate = DateTime.Now;
            DateTime earlierDate = laterDate.AddDays(-1);
            decimal percent = 30m;

            var todoItem = new TodoItem(id, "Test", "Description", "Work");
            todoItem.AddProgression(laterDate, 40m);

            _mockRepository.Setup(r => r.GetById(id)).Returns(todoItem);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                _service.RegisterProgression(id, earlierDate, percent));

            Assert.That(exception.Message, Does.Contain("Date must be greater than the date of any existing progression"));
        }

        [Test]
        public void RegisterProgression_WithRequest_AddsProgressionToItem()
        {
            // Arrange
            int id = 1;
            var request = new ProgressionRequest
            {
                Date = DateTime.Now,
                Percent = 30m
            };

            var todoItem = new TodoItem(id, "Test", "Description", "Work");

            _mockRepository.Setup(r => r.GetById(id)).Returns(todoItem);

            // Act
            _service.RegisterProgression(id, request);

            // Assert
            _mockRepository.Verify(r => r.Update(It.Is<TodoItem>(i =>
                i.Id == id &&
                i.GetTotalProgress() == request.Percent)), Times.Once);
        }

        [Test]
        public void RegisterProgressionAndReturn_WithValidData_ReturnsUpdatedItem()
        {
            // Arrange
            int id = 1;
            var request = new ProgressionRequest
            {
                Date = DateTime.Now,
                Percent = 30m
            };

            var todoItem = new TodoItem(id, "Test", "Description", "Work");

            _mockRepository.Setup(r => r.GetById(id)).Returns(todoItem);

            // Act
            var result = _service.RegisterProgressionAndReturn(id, request);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(id));
            Assert.That(result.Progressions, Has.Count.EqualTo(1));
            Assert.That(result.Progressions[0].Date, Is.EqualTo(request.Date));
            Assert.That(result.Progressions[0].Percent, Is.EqualTo(request.Percent));
            Assert.That(result.Progressions[0].AccumulatedPercent, Is.EqualTo(request.Percent));
        }

        [Test]
        public void RegisterProgression_WithNullRequest_ThrowsArgumentNullException()
        {
            // Arrange
            int id = 1;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                _service.RegisterProgression(id, null!));

            Assert.That(exception.Message, Does.Contain("Request cannot be null"));
        }

        [Test]
        public void RegisterProgression_WithInvalidPercentInRequest_ThrowsArgumentException()
        {
            // Arrange
            int id = 1;
            var request = new ProgressionRequest
            {
                Date = DateTime.Now,
                Percent = -10m
            };

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                _service.RegisterProgression(id, request));

            Assert.That(exception.Message, Does.Contain("Percent must be between 0.1 and 100.0"));
        }

        [Test]
        public void GetAllItems_ReturnsAllItemsFromRepository()
        {
            // Arrange
            var todoItems = new List<TodoItem>
            {
                new TodoItem(1, "Item 1", "Description 1", "Work"),
                new TodoItem(2, "Item 2", "Description 2", "Personal")
            };

            _mockRepository.Setup(r => r.GetAll()).Returns(todoItems);

            // Act
            var result = _service.GetAllItems();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result[0].Id, Is.EqualTo(1));
            Assert.That(result[0].Title, Is.EqualTo("Item 1"));
            Assert.That(result[1].Id, Is.EqualTo(2));
            Assert.That(result[1].Title, Is.EqualTo("Item 2"));
        }

        [Test]
        public void GetItemById_WithValidId_ReturnsItem()
        {
            // Arrange
            int id = 1;
            var todoItem = new TodoItem(id, "Test", "Description", "Work");

            _mockRepository.Setup(r => r.GetById(id)).Returns(todoItem);

            // Act
            var result = _service.GetItemById(id);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(id));
            Assert.That(result.Title, Is.EqualTo("Test"));
            Assert.That(result.Description, Is.EqualTo("Description"));
            Assert.That(result.Category, Is.EqualTo("Work"));
        }

        [Test]
        public void GetItemById_WithNonExistentId_ThrowsKeyNotFoundException()
        {
            // Arrange
            int id = 999;

            _mockRepository.Setup(r => r.GetById(id)).Returns((TodoItem)null!);

            // Act & Assert
            var exception = Assert.Throws<KeyNotFoundException>(() =>
                _service.GetItemById(id));

            Assert.That(exception.Message, Does.Contain("TodoItem with id 999 not found"));
        }

        [Test]
        public void GetAllCategories_ReturnsAllCategoriesFromRepository()
        {
            // Arrange
            var categories = new List<string> { "Work", "Personal", "Shopping" };

            _mockRepository.Setup(r => r.GetAllCategories()).Returns(categories);

            // Act
            var result = _service.GetAllCategories();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(3));
            Assert.That(result, Contains.Item("Work"));
            Assert.That(result, Contains.Item("Personal"));
            Assert.That(result, Contains.Item("Shopping"));
        }

        [Test]
        public void GetNextId_ReturnsNextIdFromRepository()
        {
            // Arrange
            int nextId = 42;

            _mockRepository.Setup(r => r.GetNextId()).Returns(nextId);

            // Act
            var result = _service.GetNextId();

            // Assert
            Assert.That(result, Is.EqualTo(nextId));
        }

        [Test]
        public void GetPrintOutput_ReturnsFormattedOutput()
        {
            // Arrange
            var todoItem = new TodoItem(1, "Complete Project Report", "Finish the final report for the project", "Work");
            todoItem.AddProgression(new DateTime(2025, 3, 18), 30m);
            todoItem.AddProgression(new DateTime(2025, 3, 19), 50m);
            todoItem.AddProgression(new DateTime(2025, 3, 20), 20m);

            _mockRepository.Setup(r => r.GetAll()).Returns(new List<TodoItem> { todoItem });

            // Act
            var result = _service.GetPrintOutput();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Does.Contain("1) Complete Project Report - Finish the final report for the project (Work) Completed:True"));
            Assert.That(result, Does.Contain("3/18/2025"));
            Assert.That(result, Does.Contain("-  30%"));
            Assert.That(result, Does.Contain("3/19/2025"));
            Assert.That(result, Does.Contain("-  80%"));
            Assert.That(result, Does.Contain("3/20/2025"));
            Assert.That(result, Does.Contain("- 100%"));
        }

        [Test]
        public void Constructor_WithNullRepository_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new TodoListService(null!, _mockLogger.Object));
        }

        [Test]
        public void Constructor_WithNullLogger_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new TodoListService(_mockRepository.Object, null!));
        }
    }
}