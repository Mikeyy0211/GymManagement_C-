using FluentValidation;

namespace Gym.Application.DTOs.Trainers;

public class UpdateTrainerRequestValidator : AbstractValidator<UpdateTrainerRequest>
{
    public UpdateTrainerRequestValidator()
    {
        RuleFor(x => x.Specialty).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ExperienceYears).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(20);
    }
}