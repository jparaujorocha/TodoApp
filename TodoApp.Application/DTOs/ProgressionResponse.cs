using System;

namespace TodoApp.Application.DTOs
{
    public class ProgressionResponse
    {
        public DateTime Date { get; set; }
        public decimal Percent { get; set; }
        public decimal AccumulatedPercent { get; set; }
    }
}