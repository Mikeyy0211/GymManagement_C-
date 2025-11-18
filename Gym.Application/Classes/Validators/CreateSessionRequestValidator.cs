using FluentValidation;
using Gym.Application.DTOs.Classes;

namespace Gym.Application.Classes.Validators;

public class CreateSessionRequestValidator : AbstractValidator<CreateSessionRequest>
{
    public CreateSessionRequestValidator()
    {
        RuleFor(x => x.ClassId).NotEmpty();
        RuleFor(x => x.StartAt).NotEmpty();
        RuleFor(x => x.EndAt)
            .GreaterThan(x => x.StartAt)
            .When(x => x.EndAt.HasValue)
            .WithMessage("End time must be after start time");
    }
}