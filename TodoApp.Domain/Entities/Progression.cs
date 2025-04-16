using System;
using System.Linq;
using FluentValidation;
using TodoApp.Domain.Validators;

namespace TodoApp.Domain.Entities
{
    public class Progression
    {
        public DateTime Date { get; private set; }
        public decimal Percent { get; private set; }

        protected Progression()
        {
        }

        public Progression(DateTime date, decimal percent)
        {
            Date = date;
            Percent = percent;

            ValidateEntity();
        }

        private void ValidateEntity()
        {
            var validator = new ProgressionValidator();
            var result = validator.Validate(this);

            if (!result.IsValid)
            {
                var error = result.Errors.First();
                throw new ArgumentException(error.ErrorMessage, error.PropertyName);
            }
        }
    }
}