using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using TodoApp.Domain.Validators;

namespace TodoApp.Domain.Entities
{
    public class TodoItem : BaseEntity
    {
        private const decimal MaxProgressPercentage = 100m;
        private const decimal MaxUpdateableProgressPercentage = 50m;

        public string Title { get; private set; }
        public string Description { get; private set; }
        public string Category { get; private set; }
        public List<Progression> Progressions { get; private set; }
        public bool IsCompleted => GetTotalProgress() >= MaxProgressPercentage;

        protected TodoItem()
        {
            Progressions = new List<Progression>();
        }

        public TodoItem(int id, string title, string description, string category)
        {
            Id = id;
            Title = title;
            Description = description ?? string.Empty;
            Category = category;
            Progressions = new List<Progression>();

            ValidateEntity();
        }

        public void UpdateDescription(string description)
        {
            ValidateUpdateEligibility();

            Description = description ?? string.Empty;
        }

        public void AddProgression(DateTime date, decimal percent)
        {
            ValidateProgressionParameters(date, percent);

            Progressions.Add(new Progression(date, percent));
        }

        public decimal GetTotalProgress()
        {
            return Progressions.Sum(p => p.Percent);
        }

        public bool CanBeDeleted()
        {
            return GetTotalProgress() <= MaxUpdateableProgressPercentage;
        }

        private void ValidateEntity()
        {
            var validator = new TodoItemValidator();
            var result = validator.Validate(this);

            if (!result.IsValid)
            {
                var error = result.Errors.First();
                throw new ArgumentException(error.ErrorMessage, error.PropertyName);
            }
        }

        private void ValidateUpdateEligibility()
        {
            if (GetTotalProgress() > MaxUpdateableProgressPercentage)
            {
                throw new InvalidOperationException("Cannot update a TodoItem that has more than 50% progress");
            }
        }

        private void ValidateProgressionParameters(DateTime date, decimal percent)
        {
            ValidateProgressionPercent(percent);
            ValidateProgressionDate(date);
            ValidateTotalProgressWouldNotExceedMaximum(percent);
        }

        private void ValidateProgressionPercent(decimal percent)
        {
            var progressionValidator = new ProgressionValidator();
            var progression = new Progression(DateTime.Now, percent); // Date doesn't matter for percent validation
            var result = progressionValidator.Validate(progression);

            if (!result.IsValid)
            {
                var error = result.Errors.First();
                throw new ArgumentException(error.ErrorMessage, nameof(percent));
            }
        }

        private void ValidateProgressionDate(DateTime date)
        {
            if (Progressions.Any() && date <= Progressions.Max(p => p.Date))
            {
                throw new ArgumentException(
                    "Date must be greater than the date of any existing progression",
                    nameof(date));
            }
        }

        private void ValidateTotalProgressWouldNotExceedMaximum(decimal newPercent)
        {
            decimal totalPercent = GetTotalProgress() + newPercent;
            if (totalPercent > MaxProgressPercentage)
            {
                throw new ArgumentException(
                    $"Total progress cannot exceed {MaxProgressPercentage}%. Current: {GetTotalProgress()}%, Attempted to add: {newPercent}%",
                    nameof(newPercent));
            }
        }
    }
}