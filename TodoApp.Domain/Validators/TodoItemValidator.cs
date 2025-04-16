using FluentValidation;
using TodoApp.Domain.Entities;

namespace TodoApp.Domain.Validators
{
    public class TodoItemValidator : AbstractValidator<TodoItem>
    {
        public TodoItemValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Id must be greater than zero");

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required");

            RuleFor(x => x.Category)
                .NotEmpty()
                .WithMessage("Category is required");
        }
    }
}