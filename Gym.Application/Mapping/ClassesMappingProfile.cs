using AutoMapper;
using Gym.Application.DTOs.Classes;
using Gym.Core.Entities;

namespace Gym.Application.Mapping;

public class ClassMappingProfile : Profile
{
    public ClassMappingProfile()
    {
        CreateMap<GymClass, ClassDto>();
        CreateMap<CreateClassRequest, GymClass>();

        CreateMap<ClassSession, SessionDto>();
        CreateMap<CreateSessionRequest, ClassSession>();
    }
}