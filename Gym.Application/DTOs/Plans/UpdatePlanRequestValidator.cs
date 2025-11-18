using FluentValidation;

namespace Gym.Application.DTOs.Plans;

public class UpdatePlanRequestValidator : AbstractValidator<UpdatePlanRequest>
{
    public UpdatePlanRequestValidator()
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


        When(x => !string.IsNullOrWhiteSpace(x.RowVersionBase64), () =>
        {
            RuleFor(x => x.RowVersionBase64!)
                .Must(BeBase64)
                .WithMessage("RowVersionBase64 must be a valid base64 string.");
        });
    }

    private bool BeBase64(string s)
    {
        try { Convert.FromBase64String(s); return true; }
        catch { return false; }
    }
}
