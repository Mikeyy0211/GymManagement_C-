using FluentValidation;
using Gym.Application.DTOs.Members;

namespace Gym.Application.Members;

public class UpdateMemberRequestValidator : AbstractValidator<UpdateMemberRequest>
{
    public UpdateMemberRequestValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MinimumLength(2).MaximumLength(100);
        When(x => x.DateOfBirth.HasValue, () =>
        {
            RuleFor(x => x.DateOfBirth!.Value)
                .LessThan(DateTime.Today.AddDays(1));
        });

        When(x => !string.IsNullOrWhiteSpace(x.RowVersionBase64), () =>
        {
            RuleFor(x => x.RowVersionBase64!)
                .Must(BeBase64).WithMessage("RowVersionBase64 must be base64 string.");
        });
    }
    private bool BeBase64(string s) { try { Convert.FromBase64String(s); return true; } catch { return false; } }
}