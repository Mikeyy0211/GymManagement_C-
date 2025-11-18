using AutoMapper;
using Gym.Application.DTOs.Trainers;
using Gym.Core.Entities;

namespace Gym.Application.Mapping;

public class TrainerMappingProfile : Profile
{
    public TrainerMappingProfile()
    {
        CreateMap<CreateTrainerRequest, TrainerProfile>();
        CreateMap<UpdateTrainerRequest, TrainerProfile>();

        CreateMap<TrainerProfile, TrainerDto>()
            .ForMember(d => d.FullName, opt => opt.MapFrom(s => s.User.FullName));
    }
}