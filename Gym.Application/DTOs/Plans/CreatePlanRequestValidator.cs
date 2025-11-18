using FluentValidation;

namespace Gym.Application.DTOs.Plans;

public class CreatePlanRequestValidator : AbstractValidator<CreatePlanRequest>
{
    public CreatePlanRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(100);

        RuleFor(x => x.Price)
            .GreaterThan(0);

        RuleFor(x => x.DurationDays)
            .GreaterThan(0);

        RuleFor(x => x.MaxSessionsPerWeek)
            .InclusiveBetween(1, 14);
    }
}