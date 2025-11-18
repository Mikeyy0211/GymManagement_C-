using FluentValidation;

namespace Gym.Application.DTOs.Members;

public class CreateMemberRequestValidator : AbstractValidator<CreateMemberRequest>
{
    public CreateMemberRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(100);

        // B1: DateOfBirth phải có giá trị
        RuleFor(x => x.DateOfBirth)
            .NotNull()
            .WithMessage("Date of birth is required");

        // B2: Khi chắc chắn không null → dùng !.Value
        RuleFor(x => x.DateOfBirth!.Value)
            .LessThan(DateTime.Today)
            .WithMessage("Date of birth must be in the past");
    }
}