using FluentValidation;
using TodoApp.Domain.Entities;

namespace TodoApp.Domain.Validators
{
    public class ProgressionValidator : AbstractValidator<Progression>
    {
        private const decimal MinPercentValue = 0m;
        private const decimal MaxPercentValue = 100m;

        public ProgressionValidator()
        {
            RuleFor(x => x.Percent)
                .GreaterThan(MinPercentValue)
                .LessThanOrEqualTo(MaxPercentValue)
                .WithMessage($"Percent must be greater than {MinPercentValue} and less than or equal to {MaxPercentValue}");
        }
    }
}