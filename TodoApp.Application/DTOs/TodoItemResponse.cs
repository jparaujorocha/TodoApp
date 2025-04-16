using System.Collections.Generic;

namespace TodoApp.Application.DTOs
{
    public class TodoItemResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public List<ProgressionResponse> Progressions { get; set; } = new List<ProgressionResponse>();
    }
}