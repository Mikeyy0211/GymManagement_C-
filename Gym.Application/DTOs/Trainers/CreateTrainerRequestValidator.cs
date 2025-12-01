using FluentValidation;

namespace Gym.Application.DTOs.Trainers;

public class CreateTrainerRequestValidator : AbstractValidator<CreateTrainerRequest>
{
    public CreateTrainerRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Specialty).NotEmpty().MaximumLength(100);
        RuleFor(x=> x.ExperienceYears).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(20);
    }
}