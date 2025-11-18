using FluentValidation;

namespace Gym.Application.DTOs.Members;

public class AssignPlanRequestValidator : AbstractValidator<AssignPlanRequest>
{
    public AssignPlanRequestValidator()
    {
        RuleFor(x => x.MembershipPlanId).NotEmpty();
        When(x => !string.IsNullOrWhiteSpace(x.RowVersionBase64), () =>
        {
            RuleFor(x => x.RowVersionBase64!)
                .Must(BeBase64).WithMessage("RowVersionBase64 must be base64 string.");
        });
    }
    private bool BeBase64(string s) { try { Convert.FromBase64String(s); return true; } catch { return false; } }
}
