using System;
using Bogus;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TodoApp.API.Controllers;
using TodoApp.Application.DTOs;
using TodoApp.Application.Interfaces;
using TodoApp.Tests.Mocks;

namespace TodoApp.Tests.API.Controllers
{
    [TestFixture]
    public class TodoItemsControllerTests
    {
        private TodoItemsController _controller;
        private Mock<ITodoListService> _mockService;
        private Faker _faker;

        [SetUp]
        public void Setup()
        {
            _mockService = MockTodoListService.GetMock();
            _controller = new TodoItemsController(_mockService.Object);
            _faker = new Faker();
        }

        [Test]
        public void GetAllItems_ReturnsOkResult()
        {
            // Arrange
            var items = new List<TodoItemResponse>
            {
                new TodoItemResponse { Id = 1, Title = "Item 1" },
                new TodoItemResponse { Id = 2, Title = "Item 2" }
            };
            _mockService.Setup(s => s.GetAllItems()).Returns(items);

            // Act
            var result = _controller.GetAllItems();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult!.Value, Is.EqualTo(items));
        }

        [Test]
        public void GetAllItems_WhenExceptionOccurs_ReturnsAppropriateErrorResult()
        {
            // Arrange
            _mockService.Setup(s => s.GetAllItems()).Throws(new Exception("Test error"));

            // Act
            var result = _controller.GetAllItems();

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var statusCodeResult = result as ObjectResult;
            Assert.That(statusCodeResult!.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public void GetItemById_WithValidId_ReturnsOkResult()
        {
            // Arrange
            int id = 1;
            var item = new TodoItemResponse { Id = id, Title = "Test Item" };
            _mockService.Setup(s => s.GetItemById(id)).Returns(item);

            // Act
            var result = _controller.GetItemById(id);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult!.Value, Is.EqualTo(item));
        }

        [Test]
        public void GetItemById_WithNonExistentId_ReturnsNotFoundResult()
        {
            // Arrange
            int id = 999;
            _mockService.Setup(s => s.GetItemById(id)).Throws(new KeyNotFoundException($"TodoItem with id {id} not found"));

            // Act
            var result = _controller.GetItemById(id);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public void GetCategories_ReturnsOkResult()
        {
            // Arrange
            var categories = new List<string> { "Work", "Personal", "Shopping" };
            _mockService.Setup(s => s.GetAllCategories()).Returns(categories);

            // Act
            var result = _controller.GetCategories();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult!.Value, Is.EqualTo(categories));
        }

        [Test]
        public void AddItem_WithValidRequest_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var request = new TodoItemRequest
            {
                Title = _faker.Lorem.Sentence(),
                Description = _faker.Lorem.Paragraph(),
                Category = "Work"
            };

            var createdItem = new TodoItemResponse
            {
                Id = 1,
                Title = request.Title,
                Description = request.Description,
                Category = request.Category
            };

            _mockService.Setup(s => s.AddItemAndReturn(request)).Returns(createdItem);

            // Act
            var result = _controller.AddItem(request);

            // Assert
            Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());
            var createdResult = result as CreatedAtActionResult;
            Assert.That(createdResult!.ActionName, Is.EqualTo("GetItemById"));
            Assert.That(createdResult.RouteValues!["id"], Is.EqualTo(createdItem.Id));
            Assert.That(createdResult.Value, Is.EqualTo(createdItem));
        }

        [Test]
        public void AddItem_WithInvalidCategory_ReturnsBadRequestResult()
        {
            // Arrange
            var request = new TodoItemRequest
            {
                Title = _faker.Lorem.Sentence(),
                Description = _faker.Lorem.Paragraph(),
                Category = "InvalidCategory"
            };

            _mockService.Setup(s => s.AddItemAndReturn(request))
                .Throws(new ArgumentException("Category 'InvalidCategory' is not valid"));

            // Act
            var result = _controller.AddItem(request);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void UpdateItem_WithValidRequest_ReturnsOkResult()
        {
            // Arrange
            int id = 1;
            var request = new TodoItemRequest
            {
                Title = _faker.Lorem.Sentence(),
                Description = _faker.Lorem.Paragraph(),
                Category = "Work"
            };

            var updatedItem = new TodoItemResponse
            {
                Id = id,
                Title = request.Title,
                Description = request.Description,
                Category = request.Category
            };

            _mockService.Setup(s => s.UpdateItemAndReturn(id, request)).Returns(updatedItem);

            // Act
            var result = _controller.UpdateItem(id, request);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult!.Value, Is.EqualTo(updatedItem));
        }

        [Test]
        public void UpdateItem_WithNonExistentId_ReturnsNotFoundResult()
        {
            // Arrange
            int id = 999;
            var request = new TodoItemRequest
            {
                Title = _faker.Lorem.Sentence(),
                Description = _faker.Lorem.Paragraph(),
                Category = "Work"
            };

            _mockService.Setup(s => s.UpdateItemAndReturn(id, request))
                .Throws(new KeyNotFoundException($"TodoItem with id {id} not found"));

            // Act
            var result = _controller.UpdateItem(id, request);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public void UpdateItem_WithItemOver50PercentProgress_ReturnsBadRequestResult()
        {
            // Arrange
            int id = 1;
            var request = new TodoItemRequest
            {
                Title = _faker.Lorem.Sentence(),
                Description = _faker.Lorem.Paragraph(),
                Category = "Work"
            };

            _mockService.Setup(s => s.UpdateItemAndReturn(id, request))
                .Throws(new InvalidOperationException("Cannot update a TodoItem that has more than 50% progress"));

            // Act
            var result = _controller.UpdateItem(id, request);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void RemoveItem_WithValidId_ReturnsNoContentResult()
        {
            // Arrange
            int id = 1;

            // Act
            var result = _controller.RemoveItem(id);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
            _mockService.Verify(s => s.RemoveItem(id), Times.Once);
        }

        [Test]
        public void RemoveItem_WithNonExistentId_ReturnsNotFoundResult()
        {
            // Arrange
            int id = 999;

            _mockService.Setup(s => s.RemoveItem(id))
                .Throws(new KeyNotFoundException($"TodoItem with id {id} not found"));

            // Act
            var result = _controller.RemoveItem(id);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public void RemoveItem_WithItemOver50PercentProgress_ReturnsBadRequestResult()
        {
            // Arrange
            int id = 1;

            _mockService.Setup(s => s.RemoveItem(id))
                .Throws(new InvalidOperationException("Cannot remove a TodoItem that has more than 50% progress"));

            // Act
            var result = _controller.RemoveItem(id);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void RegisterProgression_WithValidRequest_ReturnsOkResult()
        {
            // Arrange
            int id = 1;
            var request = new ProgressionRequest
            {
                Date = DateTime.Now,
                Percent = 30m
            };

            var updatedItem = new TodoItemResponse
            {
                Id = id,
                Title = "Test Item",
                Progressions = new List<ProgressionResponse>
                {
                    new ProgressionResponse
                    {
                        Date = request.Date,
                        Percent = request.Percent,
                        AccumulatedPercent = request.Percent
                    }
                }
            };

            _mockService.Setup(s => s.RegisterProgressionAndReturn(id, request)).Returns(updatedItem);

            // Act
            var result = _controller.RegisterProgression(id, request);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult!.Value, Is.EqualTo(updatedItem));
        }

        [Test]
        public void RegisterProgression_WithNonExistentId_ReturnsNotFoundResult()
        {
            // Arrange
            int id = 999;
            var request = new ProgressionRequest
            {
                Date = DateTime.Now,
                Percent = 30m
            };

            _mockService.Setup(s => s.RegisterProgressionAndReturn(id, request))
                .Throws(new KeyNotFoundException($"TodoItem with id {id} not found"));

            // Act
            var result = _controller.RegisterProgression(id, request);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public void RegisterProgression_WithInvalidPercent_ReturnsBadRequestResult()
        {
            // Arrange
            int id = 1;
            var request = new ProgressionRequest
            {
                Date = DateTime.Now,
                Percent = 110m
            };

            _mockService.Setup(s => s.RegisterProgressionAndReturn(id, request))
                .Throws(new ArgumentException("Percent must be between 0.1 and 100.0"));

            // Act
            var result = _controller.RegisterProgression(id, request);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void RegisterProgression_WithTotalExceeding100Percent_ReturnsBadRequestResult()
        {
            // Arrange
            int id = 1;
            var request = new ProgressionRequest
            {
                Date = DateTime.Now,
                Percent = 70m
            };

            _mockService.Setup(s => s.RegisterProgressionAndReturn(id, request))
                .Throws(new ArgumentException("Total progress cannot exceed 100%"));

            // Act
            var result = _controller.RegisterProgression(id, request);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void RegisterProgression_WithEarlierDate_ReturnsBadRequestResult()
        {
            // Arrange
            int id = 1;
            var request = new ProgressionRequest
            {
                Date = DateTime.Now.AddDays(-1),
                Percent = 30m
            };

            _mockService.Setup(s => s.RegisterProgressionAndReturn(id, request))
                .Throws(new ArgumentException("Date must be greater than the date of any existing progression"));

            // Act
            var result = _controller.RegisterProgression(id, request);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void PrintItems_ReturnsOkResultWithOutput()
        {
            // Arrange
            string expectedOutput = "1) Item 1 - Description (Work) Completed:False";
            _mockService.Setup(s => s.GetPrintOutput()).Returns(expectedOutput);

            // Act
            var result = _controller.PrintItems();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            var outputObj = okResult!.Value!.ToString();
            Assert.That(outputObj, Does.Contain("1) Item 1 - Description (Work) Completed:False"));

        }

        [Test]
        public void PrintItems_WhenExceptionOccurs_ReturnsAppropriateErrorResult()
        {
            // Arrange
            _mockService.Setup(s => s.GetPrintOutput()).Throws(new Exception("Test error"));

            // Act
            var result = _controller.PrintItems();

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var statusCodeResult = result as ObjectResult;
            Assert.That(statusCodeResult!.StatusCode, Is.EqualTo(500));
        }
    }
}