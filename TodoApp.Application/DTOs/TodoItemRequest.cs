using System.ComponentModel.DataAnnotations;

namespace TodoApp.Application.DTOs
{
    public class TodoItemRequest
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        public string Category { get; set; } = string.Empty;
    }
}