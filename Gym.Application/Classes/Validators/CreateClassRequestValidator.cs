using FluentValidation;
using Gym.Application.DTOs.Classes;

namespace Gym.Application.Classes.Validators;

public class CreateClassRequestValidator : AbstractValidator<CreateClassRequest>
{
    public CreateClassRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Capacity).GreaterThan(0);
    }
}