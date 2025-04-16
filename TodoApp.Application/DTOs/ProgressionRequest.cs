using System;
using System.ComponentModel.DataAnnotations;

namespace TodoApp.Application.DTOs
{
    public class ProgressionRequest
    {
        [Required]
        public DateTime Date { get; set; }

        [Required]
        [Range(0.1, 100.0, ErrorMessage = "Percent must be between 0.1 and 100.0")]
        public decimal Percent { get; set; }
    }
}