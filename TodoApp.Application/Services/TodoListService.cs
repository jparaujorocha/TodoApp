using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using TodoApp.Application.DTOs;
using TodoApp.Application.Interfaces;
using TodoApp.Application.Mappers;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Interfaces.Repositories;

namespace TodoApp.Application.Services
{
    public class TodoListService : ITodoListService
    {
        private readonly ITodoListRepository _repository;
        private readonly ILogger<TodoListService> _logger;

        public TodoListService(ITodoListRepository repository, ILogger<TodoListService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void AddItem(int id, string title, string description, string category)
        {
            try
            {
                _logger.LogInformation($"Adding TodoItem with id {id}, title: '{title}', category: '{category}'");

                ValidateCategory(category);

                TodoItem item = new TodoItem(id, title, description, category);
                _repository.Add(item);

                _logger.LogInformation($"Successfully added TodoItem with id {id}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding TodoItem with id {id}: {ex.Message}", ex);
                throw;
            }
        }

        public int AddItem(TodoItemRequest request)
        {
            try
            {
                ValidateRequest(request);

                _logger.LogInformation($"Adding TodoItem with title: '{request.Title}', category: '{request.Category}'");


                int id = GetNextId();
                AddItem(id, request.Title, request.Description, request.Category);

                _logger.LogInformation($"Successfully added TodoItem with id {id}");
                return id;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding TodoItem: {ex.Message}", ex);
                throw;
            }
        }

        public TodoItemResponse RegisterProgressionAndReturn(int id, ProgressionRequest request)
        {
            try
            {
                _logger.LogInformation($"Registering progression and returning TodoItem with id {id}");

                RegisterProgression(id, request);
                TodoItemResponse result = GetItemById(id);

                _logger.LogInformation($"Successfully registered progression and returned TodoItem with id {id}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error registering progression and returning TodoItem with id {id}: {ex.Message}", ex);
                throw;
            }
        }
        public TodoItemResponse AddItemAndReturn(TodoItemRequest request)
        {
            try
            {
                _logger.LogInformation($"Adding TodoItem and returning DTO with title: '{request.Title}', category: '{request.Category}'");

                int id = AddItem(request);
                TodoItemResponse result = GetItemById(id);

                _logger.LogInformation($"Successfully added and returned TodoItem with id {id}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding and returning TodoItem: {ex.Message}", ex);
                throw;
            }
        }

        public void UpdateItem(int id, string description)
        {
            try
            {
                _logger.LogInformation($"Updating description for TodoItem with id {id}");

                TodoItem item = GetTodoItemById(id);
                item.UpdateDescription(description);
                _repository.Update(item);

                _logger.LogInformation($"Successfully updated description for TodoItem with id {id}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating description for TodoItem with id {id}: {ex.Message}", ex);
                throw;
            }
        }

        public void UpdateItem(int id, TodoItemRequest request)
        {
            try
            {
                _logger.LogInformation($"Updating TodoItem with id {id}");

                ValidateRequest(request);
                UpdateItem(id, request.Description);

                _logger.LogInformation($"Successfully updated TodoItem with id {id}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating TodoItem with id {id}: {ex.Message}", ex);
                throw;
            }
        }

        public TodoItemResponse UpdateItemAndReturn(int id, TodoItemRequest request)
        {
            try
            {
                _logger.LogInformation($"Updating and returning TodoItem with id {id}");

                UpdateItem(id, request);
                TodoItemResponse result = GetItemById(id);

                _logger.LogInformation($"Successfully updated and returned TodoItem with id {id}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating and returning TodoItem with id {id}: {ex.Message}", ex);
                throw;
            }
        }

        public void RemoveItem(int id)
        {
            try
            {
                _logger.LogInformation($"Removing TodoItem with id {id}");

                TodoItem item = GetTodoItemById(id);

                if (!item.CanBeDeleted())
                {
                    string errorMessage = "Cannot remove a TodoItem that has more than 50% progress";
                    _logger.LogWarning($"Failed to remove TodoItem with id {id}: {errorMessage}");
                    throw new InvalidOperationException(errorMessage);
                }

                _repository.Delete(id);
                _logger.LogInformation($"Successfully removed TodoItem with id {id}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error removing TodoItem with id {id}: {ex.Message}", ex);
                throw;
            }
        }

        public void RegisterProgression(int id, DateTime dateTime, decimal percent)
        {
            try
            {
                _logger.LogInformation($"Registering progression for TodoItem with id {id}: {percent}% on {dateTime}");

                TodoItem item = GetTodoItemById(id);
                item.AddProgression(dateTime, percent);
                _repository.Update(item);

                _logger.LogInformation($"Successfully registered progression for TodoItem with id {id}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error registering progression for TodoItem with id {id}: {ex.Message}", ex);
                throw;
            }
        }

        public void RegisterProgression(int id, ProgressionRequest request)
        {
            try
            {
                ValidateRequest(request);
                _logger.LogInformation($"Registering progression for TodoItem with id {id}: {request.Percent}% on {request.Date}");

                RegisterProgression(id, request.Date, request.Percent);

                _logger.LogInformation($"Successfully registered progression for TodoItem with id {id}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error registering progression for TodoItem with id {id}: {ex.Message}", ex);
                throw;
            }
        }

        public List<TodoItemResponse> GetAllItems()
        {
            try
            {
                _logger.LogInformation("Getting all TodoItems");

                var items = _repository.GetAll();
                var result = TodoItemMapper.ToDtoList(items);

                _logger.LogInformation($"Successfully retrieved {result.Count} TodoItems");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting all TodoItems: {ex.Message}", ex);
                throw;
            }
        }

        public TodoItemResponse GetItemById(int id)
        {
            try
            {
                _logger.LogInformation($"Getting TodoItem with id {id}");

                var item = GetTodoItemById(id);
                var result = TodoItemMapper.ToDto(item);

                _logger.LogInformation($"Successfully retrieved TodoItem with id {id}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting TodoItem with id {id}: {ex.Message}", ex);
                throw;
            }
        }

        public List<string> GetAllCategories()
        {
            try
            {
                _logger.LogInformation("Getting all categories");

                var result = _repository.GetAllCategories();

                _logger.LogInformation($"Successfully retrieved {result.Count} categories");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting all categories: {ex.Message}", ex);
                throw;
            }
        }

        public int GetNextId()
        {
            try
            {
                _logger.LogInformation("Getting next available id");

                int result = _repository.GetNextId();

                _logger.LogInformation($"Next available id is {result}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting next available id: {ex.Message}", ex);
                throw;
            }
        }

        public string GetPrintOutput()
        {
            try
            {
                _logger.LogInformation("Getting formatted print output for all TodoItems");

                StringBuilder output = new StringBuilder();
                var items = _repository.GetAll().OrderBy(i => i.Id).ToList();

                for (int i = 0; i < items.Count; i++)
                {
                    PrintItemToStringBuilder(items[i], output);

                    // Add a separator between items, but not after the last one
                    if (i < items.Count - 1)
                    {
                        output.AppendLine();
                    }
                }

                _logger.LogInformation("Successfully generated formatted print output");
                return output.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting print output: {ex.Message}", ex);
                throw;
            }
        }

        private void PrintItemToStringBuilder(TodoItem item, StringBuilder sb)
        {
            var dto = TodoItemMapper.ToDto(item);

            // Header line with title, description, category and completion status
            sb.AppendLine($"{dto.Id}) {dto.Title} - {dto.Description} ({dto.Category}) Completed:{dto.IsCompleted}");

            // Print each progression with proper formatting
            foreach (var progression in dto.Progressions.OrderBy(p => p.Date))
            {
                // Format the date properly
                string formattedDate = progression.Date.ToString("MM/dd/yyyy HH:mm:ss");

                // Create progress bar - ensure it has exact length with no extra characters
                int progressBarLength = 50;
                int filledLength = (int)(progressBarLength * progression.AccumulatedPercent / 100);
                string progressBar = "|" + new string('O', filledLength) + new string(' ', progressBarLength - filledLength) + "|";

                // Print progression line with precise alignment
                sb.AppendLine($"{formattedDate} - {progression.AccumulatedPercent,3}% {progressBar}");
            }
        }

        private TodoItem GetTodoItemById(int id)
        {
            TodoItem item = _repository.GetById(id);

            if (item == null)
            {
                throw new KeyNotFoundException($"TodoItem with id {id} not found");
            }

            return item;
        }

        private void ValidateCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                throw new ArgumentException("Category is required", nameof(category));
            }

            List<string> validCategories = _repository.GetAllCategories();
            if (!validCategories.Contains(category))
            {
                throw new ArgumentException(
                    $"Category '{category}' is not valid. Valid categories are: {string.Join(", ", validCategories)}",
                    nameof(category));
            }
        }

        private void ValidateRequest(TodoItemRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Request cannot be null");
            }

            if (string.IsNullOrWhiteSpace(request.Title))
            {
                throw new ArgumentException("Title is required", nameof(request.Title));
            }

            ValidateCategory(request.Category);
        }

        private void ValidateRequest(ProgressionRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Request cannot be null");
            }

            if (request.Percent <= 0 || request.Percent > 100)
            {
                throw new ArgumentException("Percent must be between 0.1 and 100.0", nameof(request.Percent));
            }
        }

    }
}