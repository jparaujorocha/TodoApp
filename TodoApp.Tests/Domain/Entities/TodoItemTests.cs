using Bogus;
using NUnit.Framework;
using TodoApp.Domain.Entities;

namespace TodoApp.Tests.Domain.Entities
{
    [TestFixture]
    public class TodoItemTests
    {
        private Faker _faker;

        [SetUp]
        public void Setup()
        {
            _faker = new Faker();
        }

        [Test]
        public void Constructor_WithValidData_CreatesTodoItem()
        {
            // Arrange
            int id = _faker.Random.Int(1, 1000);
            string title = _faker.Lorem.Sentence();
            string description = _faker.Lorem.Paragraph();
            string category = _faker.Lorem.Word();

            // Act
            var todoItem = new TodoItem(id, title, description, category);

            // Assert
            Assert.That(todoItem, Is.Not.Null);
            Assert.That(todoItem.Id, Is.EqualTo(id));
            Assert.That(todoItem.Title, Is.EqualTo(title));
            Assert.That(todoItem.Description, Is.EqualTo(description));
            Assert.That(todoItem.Category, Is.EqualTo(category));
            Assert.That(todoItem.Progressions, Is.Empty);
            Assert.That(todoItem.IsCompleted, Is.False);
        }

        [Test]
        public void Constructor_WithNullDescription_UsesEmptyString()
        {
            // Arrange
            int id = _faker.Random.Int(1, 1000);
            string title = _faker.Lorem.Sentence();
            string category = _faker.Lorem.Word();

            // Act
            var todoItem = new TodoItem(id, title, null!, category);

            // Assert
            Assert.That(todoItem.Description, Is.EqualTo(string.Empty));
        }

        [Test]
        public void Constructor_WithEmptyTitle_ThrowsArgumentException()
        {
            // Arrange
            int id = _faker.Random.Int(1, 1000);
            string description = _faker.Lorem.Paragraph();
            string category = _faker.Lorem.Word();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new TodoItem(id, string.Empty, description, category));
            Assert.That(exception.Message, Does.Contain("Title is required"));
        }

        [Test]
        public void Constructor_WithEmptyCategory_ThrowsArgumentException()
        {
            // Arrange
            int id = _faker.Random.Int(1, 1000);
            string title = _faker.Lorem.Sentence();
            string description = _faker.Lorem.Paragraph();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new TodoItem(id, title, description, string.Empty));
            Assert.That(exception.Message, Does.Contain("Category is required"));
        }

        [Test]
        public void Constructor_WithZeroId_ThrowsArgumentException()
        {
            // Arrange
            string title = _faker.Lorem.Sentence();
            string description = _faker.Lorem.Paragraph();
            string category = _faker.Lorem.Word();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new TodoItem(0, title, description, category));
            Assert.That(exception.Message, Does.Contain("Id must be greater than zero"));
        }

        [Test]
        public void AddProgression_WithValidData_AddsProgressionToList()
        {
            // Arrange
            var todoItem = new TodoItem(1, "Test", "Description", "Category");
            DateTime date = _faker.Date.Recent();
            decimal percent = _faker.Random.Decimal(0.1m, 100m);

            // Act
            todoItem.AddProgression(date, percent);

            // Assert
            Assert.That(todoItem.Progressions, Has.Count.EqualTo(1));
            Assert.That(todoItem.GetTotalProgress(), Is.EqualTo(percent));
        }

        [Test]
        public void AddProgression_WithMultipleValidProgressions_TracksTotal()
        {
            // Arrange
            var todoItem = new TodoItem(1, "Test", "Description", "Category");

            // Act
            todoItem.AddProgression(DateTime.Now.AddDays(-3), 30m);
            todoItem.AddProgression(DateTime.Now.AddDays(-2), 40m);
            todoItem.AddProgression(DateTime.Now.AddDays(-1), 20m);

            // Assert
            Assert.That(todoItem.Progressions, Has.Count.EqualTo(3));
            Assert.That(todoItem.GetTotalProgress(), Is.EqualTo(90m));
        }

        [Test]
        public void AddProgression_WithTotalExceeding100_ThrowsArgumentException()
        {
            // Arrange
            var todoItem = new TodoItem(1, "Test", "Description", "Category");
            todoItem.AddProgression(DateTime.Now.AddDays(-2), 60m);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                todoItem.AddProgression(DateTime.Now.AddDays(-1), 41m));

            Assert.That(exception.Message, Does.Contain("Total progress cannot exceed 100%"));
        }

        [Test]
        public void AddProgression_WithEarlierDate_ThrowsArgumentException()
        {
            // Arrange
            var todoItem = new TodoItem(1, "Test", "Description", "Category");
            var laterDate = DateTime.Now;
            var earlierDate = laterDate.AddDays(-1);

            todoItem.AddProgression(laterDate, 30m);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                todoItem.AddProgression(earlierDate, 20m));

            Assert.That(exception.Message, Does.Contain("Date must be greater than the date of any existing progression"));
        }

        [Test]
        public void AddProgression_WithSameDate_ThrowsArgumentException()
        {
            // Arrange
            var todoItem = new TodoItem(1, "Test", "Description", "Category");
            var date = DateTime.Now;

            todoItem.AddProgression(date, 30m);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                todoItem.AddProgression(date, 20m));

            Assert.That(exception.Message, Does.Contain("Date must be greater than the date of any existing progression"));
        }

        [Test]
        public void IsCompleted_WithExactly100PercentProgress_ReturnsTrue()
        {
            // Arrange
            var todoItem = new TodoItem(1, "Test", "Description", "Category");
            todoItem.AddProgression(DateTime.Now.AddDays(-3), 30m);
            todoItem.AddProgression(DateTime.Now.AddDays(-2), 40m);
            todoItem.AddProgression(DateTime.Now.AddDays(-1), 30m);

            // Assert
            Assert.That(todoItem.IsCompleted, Is.True);
        }

        [Test]
        public void IsCompleted_WithLessThan100PercentProgress_ReturnsFalse()
        {
            // Arrange
            var todoItem = new TodoItem(1, "Test", "Description", "Category");
            todoItem.AddProgression(DateTime.Now.AddDays(-2), 40m);
            todoItem.AddProgression(DateTime.Now.AddDays(-1), 30m);

            // Assert
            Assert.That(todoItem.IsCompleted, Is.False);
        }

        [Test]
        public void UpdateDescription_WithValidData_UpdatesDescription()
        {
            // Arrange
            var todoItem = new TodoItem(1, "Test", "Old Description", "Category");
            string newDescription = "New Description";

            // Act
            todoItem.UpdateDescription(newDescription);

            // Assert
            Assert.That(todoItem.Description, Is.EqualTo(newDescription));
        }

        [Test]
        public void UpdateDescription_WithNullDescription_UsesEmptyString()
        {
            // Arrange
            var todoItem = new TodoItem(1, "Test", "Old Description", "Category");

            // Act
            todoItem.UpdateDescription(null!);

            // Assert
            Assert.That(todoItem.Description, Is.EqualTo(string.Empty));
        }

        [Test]
        public void UpdateDescription_WithProgressMoreThan50Percent_ThrowsInvalidOperationException()
        {
            // Arrange
            var todoItem = new TodoItem(1, "Test", "Description", "Category");
            todoItem.AddProgression(DateTime.Now.AddDays(-2), 30m);
            todoItem.AddProgression(DateTime.Now.AddDays(-1), 30m);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                todoItem.UpdateDescription("New Description"));

            Assert.That(exception.Message, Does.Contain("Cannot update a TodoItem that has more than 50% progress"));
        }

        [Test]
        public void CanBeDeleted_WithLessThan50PercentProgress_ReturnsTrue()
        {
            // Arrange
            var todoItem = new TodoItem(1, "Test", "Description", "Category");
            todoItem.AddProgression(DateTime.Now, 50m);

            // Assert
            Assert.That(todoItem.CanBeDeleted(), Is.True);
        }

        [Test]
        public void CanBeDeleted_WithMoreThan50PercentProgress_ReturnsFalse()
        {
            // Arrange
            var todoItem = new TodoItem(1, "Test", "Description", "Category");
            todoItem.AddProgression(DateTime.Now.AddDays(-2), 30m);
            todoItem.AddProgression(DateTime.Now.AddDays(-1), 30m);

            // Assert
            Assert.That(todoItem.CanBeDeleted(), Is.False);
        }

        [Test]
        public void GetTotalProgress_WithNoProgressions_ReturnsZero()
        {
            // Arrange
            var todoItem = new TodoItem(1, "Test", "Description", "Category");

            // Assert
            Assert.That(todoItem.GetTotalProgress(), Is.EqualTo(0m));
        }

        [Test]
        public void GetTotalProgress_WithProgressions_ReturnsSumOfAllPercentages()
        {
            // Arrange
            var todoItem = new TodoItem(1, "Test", "Description", "Category");
            todoItem.AddProgression(DateTime.Now.AddDays(-3), 10.5m);
            todoItem.AddProgression(DateTime.Now.AddDays(-2), 25.5m);
            todoItem.AddProgression(DateTime.Now.AddDays(-1), 14m);

            // Assert
            Assert.That(todoItem.GetTotalProgress(), Is.EqualTo(50m));
        }
    }
}