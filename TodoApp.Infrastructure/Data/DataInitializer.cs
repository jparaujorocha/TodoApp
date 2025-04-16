using System;
using Microsoft.Extensions.Logging;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Interfaces.Repositories;

namespace TodoApp.Infrastructure.Data
{
    public class DataInitializer
    {
        private readonly ITodoListRepository _repository;
        private readonly ILogger<DataInitializer> _logger;

        public DataInitializer(ITodoListRepository repository, ILogger<DataInitializer> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public void SeedData()
        {
            try
            {
                _logger.LogInformation("Initializing sample data");

                // Add example TodoItem as in the exercise
                var todoItem = new TodoItem(1, "Complete Project Report", "Finish the final report for the project", "Work");

                // Add progressions to the TodoItem with properly formatted dates
                var date1 = new DateTime(2025, 3, 18);
                var date2 = new DateTime(2025, 3, 19);
                var date3 = new DateTime(2025, 3, 20);

                todoItem.AddProgression(date1, 30m);
                todoItem.AddProgression(date2, 50m);
                todoItem.AddProgression(date3, 20m);

                _repository.Add(todoItem);

                _logger.LogInformation("Successfully initialized sample data");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error initializing sample data: {ex.Message}", ex);
            }
        }
    }
}