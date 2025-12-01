using Gym.Core.Entities;

namespace Gym.Core.Interfaces;

public interface IReportRepository
{
    IQueryable<Payment> PaymentsQuery();
    IQueryable<Booking> BookingsQuery();
    IQueryable<ClassSession> SessionsQuery();
    IQueryable<GymClass> ClassesQuery();
    IQueryable<Member> MembersQuery();
    IQueryable<TrainerProfile> TrainersQuery();
    
}