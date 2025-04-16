using Bogus;
using NUnit.Framework;
using TodoApp.Domain.Entities;

namespace TodoApp.Tests.Domain.Entities
{
    [TestFixture]
    public class ProgressionTests
    {
        private Faker _faker;

        [SetUp]
        public void Setup()
        {
            _faker = new Faker();
        }

        [Test]
        public void Constructor_WithValidData_CreatesProgression()
        {
            // Arrange
            DateTime date = _faker.Date.Recent();
            decimal percent = _faker.Random.Decimal(0.1m, 100m);

            // Act
            var progression = new Progression(date, percent);

            // Assert
            Assert.That(progression, Is.Not.Null);
            Assert.That(progression.Date, Is.EqualTo(date));
            Assert.That(progression.Percent, Is.EqualTo(percent));
        }

        [Test]
        public void Constructor_WithNegativePercent_ThrowsArgumentException()
        {
            // Arrange
            DateTime date = _faker.Date.Recent();
            decimal percent = _faker.Random.Decimal(-100m, 0m);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Progression(date, percent));
        }

        [Test]
        public void Constructor_WithPercentGreaterThan100_ThrowsArgumentException()
        {
            // Arrange
            DateTime date = _faker.Date.Recent();
            decimal percent = _faker.Random.Decimal(100.1m, 1000m);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Progression(date, percent));
            Assert.That(exception.Message, Does.Contain("Percent must be greater than 0 and less than or equal to 100"));
        }

        [Test]
        public void Constructor_WithZeroPercent_ThrowsArgumentException()
        {
            // Arrange
            DateTime date = _faker.Date.Recent();
            decimal percent = 0m;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Progression(date, percent));
        }

        [Test]
        public void Constructor_WithExactly100Percent_CreatesProgression()
        {
            // Arrange
            DateTime date = _faker.Date.Recent();
            decimal percent = 100m;

            // Act
            var progression = new Progression(date, percent);

            // Assert
            Assert.That(progression, Is.Not.Null);
            Assert.That(progression.Date, Is.EqualTo(date));
            Assert.That(progression.Percent, Is.EqualTo(percent));
        }

        [Test]
        public void Constructor_WithExactly0Point1Percent_CreatesProgression()
        {
            // Arrange
            DateTime date = _faker.Date.Recent();
            decimal percent = 0.1m;

            // Act
            var progression = new Progression(date, percent);

            // Assert
            Assert.That(progression, Is.Not.Null);
            Assert.That(progression.Date, Is.EqualTo(date));
            Assert.That(progression.Percent, Is.EqualTo(percent));
        }
    }
}