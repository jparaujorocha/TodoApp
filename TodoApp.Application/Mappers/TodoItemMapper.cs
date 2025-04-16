using System.Collections.Generic;
using System.Linq;
using TodoApp.Application.DTOs;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Mappers
{
    public static class TodoItemMapper
    {
        public static TodoItemResponse ToDto(TodoItem entity)
        {
            if (entity == null)
            {
                return null!;
            }

            var progressionDtos = new List<ProgressionResponse>();
            decimal accumulatedPercent = 0;

            foreach (var progression in entity.Progressions.OrderBy(p => p.Date))
            {
                accumulatedPercent += progression.Percent;

                progressionDtos.Add(new ProgressionResponse
                {
                    Date = progression.Date,
                    Percent = progression.Percent,
                    AccumulatedPercent = accumulatedPercent
                });
            }

            return new TodoItemResponse
            {
                Id = entity.Id,
                Title = entity.Title,
                Description = entity.Description,
                Category = entity.Category,
                IsCompleted = entity.IsCompleted,
                Progressions = progressionDtos
            };
        }

        public static List<TodoItemResponse> ToDtoList(IEnumerable<TodoItem> entities)
        {
            return entities?.Select(ToDto).ToList() ?? new List<TodoItemResponse>();
        }
    }
}